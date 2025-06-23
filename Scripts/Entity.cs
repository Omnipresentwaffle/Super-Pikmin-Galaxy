using Godot;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text.RegularExpressions;

public partial class Entity : CharacterBody2D
{
	//entites are moving bodies that are affected by gravity

	public Godot.Vector2 normalDir = Godot.Vector2.Zero;
	public Godot.Vector2 tangentDir = Godot.Vector2.Zero;

	public float normalVelocity = 0f;
	public float tangentVelocity = 0f;
	public float angle = 0f;

	public List<GravityArea> gravityAreas = new List<GravityArea>();

	


	public override void _PhysicsProcess(double delta)
	{

	}



	public Godot.Vector2 getPerp(Godot.Vector2 dir){

	Godot.Vector2 perp =Godot.Vector2.Zero;		
	perp.X = dir.Y;
	perp.Y = -dir.X;
	return perp;	
		
	}

	public Godot.Vector2 getTangent(Godot.Vector2 normal){
		Godot.Vector2 tangent = Godot.Vector2.Zero;
		tangent = getPerp(normal);
		//tangent.X *= -1;
		return tangent;

	}
	public float getProjection(Godot.Vector2 projected, Godot.Vector2 projDir){
		//projected is the vector we want to project onto projDir
		float projection = 0;
		float dotProd = projected.Dot(projDir);
		float magnitude = projDir.Length();

		projection = dotProd / magnitude;

		return projection;
	}

	public (float, float)getMagnitudes(Godot.Vector2 velocity, Godot.Vector2 normalDir){
		float norMag = getProjection(velocity, normalDir);
		float tanMag = getProjection(velocity, getPerp(normalDir));

		return (norMag, tanMag);
	}


}
