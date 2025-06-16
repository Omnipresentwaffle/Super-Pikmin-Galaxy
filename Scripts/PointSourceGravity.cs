using Godot;
using System;

public partial class PointSourceGravity : GravityArea
{
	//This is for circular point source gravity

	[Export]
	Vector2 pointSource = Vector2.Zero;

	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
	}


	public (Vector2, float)getNormal(Vector2 entityPos){

		//find the distance between the entity and the gravity source
		Vector2 distance = Vector2.Zero;
		float angle = 0f;
		
		distance.X = pointSource.X - entityPos.X;
		distance.Y = pointSource.Y - entityPos.Y;
		distance = distance.Normalized();
		angle = entityPos.AngleToPoint(pointSource);
		return (distance, angle);


	}



}
