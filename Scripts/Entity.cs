using Godot;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text.RegularExpressions;

public partial class Entity : CharacterBody2D
{
	//entites are moving bodies that are affected by gravity

	public GravityArea mainGravity = null;
	public bool newGravPriority = false;
	public bool gravPriorityLocked = false;

	public UInt16 state = 0;

	public UInt16 subState = 0;






	public Godot.Vector2 normalDir = Godot.Vector2.Zero;
	public Godot.Vector2 tangentDir = Godot.Vector2.Zero;

	public bool gravEmpty = true;


	public float normalVelocity = 0f;
	public float tangentVelocity = 0f;
	public float angle = 0f;

	public List<GravityArea> gravityAreas = new List<GravityArea>();




	public override void _PhysicsProcess(double delta)
	{

	}



	public Godot.Vector2 getPerp(Godot.Vector2 dir)
	{

		Godot.Vector2 perp = Godot.Vector2.Zero;
		perp.X = dir.Y;
		perp.Y = -dir.X;
		return perp;

	}

	public Godot.Vector2 getTangent(Godot.Vector2 normal)
	{
		Godot.Vector2 tangent = Godot.Vector2.Zero;
		tangent = getPerp(normal);
		//tangent.X *= -1;
		return tangent;

	}
	public float getProjection(Godot.Vector2 projected, Godot.Vector2 projDir)
	{
		//projected is the vector we want to project onto projDir
		float projection = 0;
		float dotProd = projected.Dot(projDir);
		float magnitude = projDir.Length();

		projection = dotProd / magnitude;

		return projection;
	}

	public (float, float) getMagnitudes(Godot.Vector2 velocity, Godot.Vector2 normalDir)
	{
		float norMag = getProjection(velocity, normalDir);
		float tanMag = getProjection(velocity, getPerp(normalDir));

		return (norMag, tanMag);
	}

	public void _on_gravity_area_entered(Area2D area)
	{
		//get the path to the gravity and 
		NodePath path = GetPathTo(area);
		GravityArea gZone = GetNode<GravityArea>(path);

		if (gravEmpty)
		{
			gravityAreas.Clear();
			gravEmpty = false;

		}

		//add the gravity area to the zones

		prioritizeGravityArea(gZone);


		//GD.Print("gravCount: ", gravityAreas.Count);


		return;
	}

	public void prioritizeGravityArea(GravityArea gZone)
	{

		//loop through the gravity zones and check their priority levels
		//start at index 0

		int i = 0;
		if (gravityAreas.Count == 0)
		{
			gravityAreas.Add(gZone);
			if (mainGravity != gZone)
			{
				mainGravity = gZone;
				newGravPriority = true;

			}

			return;

		}


		for (i = 0; i < gravityAreas.Count; i += 1)
		{
			//if the priority of the new gZone is >= the one in the list
			if (gZone.priority >= gravityAreas[i].priority)
			{
				gravityAreas.Insert(i, gZone);

				break;
			}
		}
		if (gravPriorityLocked)
		{
			return;
		}

		if (i == 0)
		{
			mainGravity = gZone;
			newGravPriority = true;
		}


	}
	
	public void _on_gravity_area_exited(Area2D area)
	{
		NodePath path = GetPathTo(area);
		GravityArea gZone = GetNode<GravityArea>(path);

		int gIndex = 0;


		if (gravityAreas.Count >= 2)
		{
			gravityAreas.Remove(gZone);
			if (!gravPriorityLocked)
			{
				mainGravity = gravityAreas[0];

			}
		}
		else if (gravityAreas.Count == 1)
		{
			gravEmpty = true;
		}



	}


}
