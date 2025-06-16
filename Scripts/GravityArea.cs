using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

public partial class GravityArea : Area2D
{
	//This is the base gravityArea class
	//it will have subclasses and use polymorphism

	[Export]
	public float gravityStrength = 1800f;
	[Export]
	public Behaviour behaviour = Behaviour.oneDirection;

	[Export]
	public Godot.Vector2 pointSource = Godot.Vector2.Zero;

	[Export]
	bool defaultValues = true;
	[Export]
	//must be supplied in degrees
	public float angle = 0f;


	public Line2D curve = null;
	public Godot.Vector2 norDir = Godot.Vector2.Zero;

	public enum Behaviour{
		oneDirection,
		pointSource,
		lineSource
	}
	
	public int priority = 0;


	public override void _Ready()
	{
		if(defaultValues){
			switch(behaviour){
				case Behaviour.oneDirection:
					float radAngle = angle * (float)Math.PI/180;
					norDir.X = (float)Math.Cos(radAngle);
					norDir.Y = -(float)Math.Sin(radAngle);
					GD.Print("angle: ", angle);
					GD.Print("norDir: ", norDir);
					break;
				case Behaviour.pointSource:
					pointSource = GlobalPosition;
					GD.Print("Default bin");
					break;
				case Behaviour.lineSource:
					curve = GetNode<Line2D>("curve");
					break;


			}

		}
	}

	public override void _Process(double delta)
	{
		return;
	}


	public Godot.Vector2 getPerp(Godot.Vector2 dir){

		Godot.Vector2 perp = Godot.Vector2.Zero;
		perp.X = dir.Y;
		perp.Y = -dir.Y;

		return Godot.Vector2.Zero;
		
	}

	public (Godot.Vector2, float)getPointSourceValues(Godot.Vector2 pointSource, Godot.Vector2 entityPos){
		Godot.Vector2 distance = Godot.Vector2.Zero;
		Godot.Vector2 normal = Godot.Vector2.Zero;
		float setAngle = 0f;
				
				distance.X = pointSource.X - entityPos.X;
				distance.Y = pointSource.Y - entityPos.Y;
				normal = distance.Normalized();
				setAngle = entityPos.AngleToPoint(pointSource);
				setAngle *= (float)(180/Math.PI);
				setAngle -= 90f;
		return (normal, setAngle);

	}


	public (Godot.Vector2, Godot.Vector2, float)getDirections(Godot.Vector2 entityPos){

		Godot.Vector2 normal = Godot.Vector2.Zero;
		float setAngle = 0f;
		
				
		switch(behaviour){
			case Behaviour.oneDirection:
				normal = norDir;
				setAngle = angle;
				setAngle = -angle -90f;
				break;
			case Behaviour.pointSource:
				Godot.Vector2 distance = Godot.Vector2.Zero;
				
				distance.X = pointSource.X - entityPos.X;
				distance.Y = pointSource.Y - entityPos.Y;
				normal = distance.Normalized();
				setAngle = entityPos.AngleToPoint(pointSource);
				setAngle *= (float)(180/Math.PI);
				setAngle -= 90f;

				break;

			case Behaviour.lineSource:
				List<Godot.Vector2> pointSources = [];
				//get the closest point of the predefined points
				int index = getClosestDefinedPoint(entityPos, curve.Points);
				//check index +1 of that point
				bool proceed = true;
				if(index == curve.Points.Count()-1){
					if(!curve.Closed){
						//we will not check for a line that does not exist
						proceed = false;
					}
				}
				if (proceed){
					Godot.Vector2[] pointList = [Godot.Vector2.Zero, Godot.Vector2.Zero];
					pointList[0] = curve.GetPointPosition(index);
					pointList[1] = curve.GetPointPosition((index+1)%curve.Points.Count());
					pointSources.Add(castToLineSegment(pointList,entityPos));

				}
				
				proceed = true;
				//check index -1
				if(index == 0){
					if(!curve.Closed){
						proceed = false;
					}
				}
				if (proceed){
					Godot.Vector2[] pointList = [Godot.Vector2.Zero, Godot.Vector2.Zero];
					pointList[0] = curve.GetPointPosition(index);
					int pIndex = (index-1);
					if (pIndex < 0){
						pIndex = curve.Points.Count()-1;
					}
					pointList[1] = curve.GetPointPosition(pIndex);
					pointSources.Add(castToLineSegment(pointList,entityPos,1));

				}
				index = 0;
				if(pointSources.Count() >= 2){
					if(entityPos.DistanceSquaredTo(pointSources[0]) > entityPos.DistanceSquaredTo(pointSources[1])){
					//second point is closer
					index = 1;
				}
				}
				
				(normal,setAngle) = getPointSourceGravity(pointSources[index], entityPos);



				//normal = castToLineSegment(curve.Points, entityPos);
				break;


		}
		return (normal, getPerp(normal),  setAngle);

	}


	public int getClosestDefinedPoint(Godot.Vector2 entityPos, Godot.Vector2[] points){
		//loop through the points and get the index of the closest one
		int index = 0;
		float distance = entityPos.DistanceSquaredTo(ToGlobal(points[0]));
		for (int i = 1; i < points.Length; i++){
			float checkDis = entityPos.DistanceSquaredTo(ToGlobal(points[i]));
			if (checkDis < distance){
				distance = checkDis;
				index = i;
			}
			
			
		}
		return index;
	}

		public float getProjection(Godot.Vector2 projected, Godot.Vector2 projDir){
		//projected is the vector we want to project onto projDir

		return (projected.Dot(projDir))/(projDir.Dot(projDir));
	}

	public Godot.Vector2 castToLineSegment(Godot.Vector2[] points, Godot.Vector2 entityPos, UInt16 start = 0){
		//we want to convert this into the form
		//p(t) = a + t - (b-a)
		Godot.Vector2 lineVector = Godot.Vector2.Zero;

		Godot.Vector2 startPos = ToGlobal(points[0]);
		
		
		
		lineVector = points[1]-points[0];

		Godot.Vector2 entityVector = entityPos - startPos;
	

		float t = getProjection(entityVector, lineVector);

		t = Math.Clamp(t,0,1);


		Godot.Vector2 pointSource = startPos + (t*lineVector);


		return pointSource;
	}


	public (Godot.Vector2, float) getPointSourceGravity(Godot.Vector2 pointSource, Godot.Vector2 entityPos){
		Godot.Vector2 retNormal = Godot.Vector2.Zero;
		float retAngle = 0f;
		retNormal.X = pointSource.X - entityPos.X;
		retNormal.Y = pointSource.Y - entityPos.Y;
		retNormal = retNormal.Normalized();
		retAngle = retNormal.Angle();
		retAngle *= (float)(180/Math.PI);
		
		retAngle -= 90f;

		return (retNormal, retAngle);
	}

}
