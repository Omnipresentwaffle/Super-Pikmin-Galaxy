using Godot;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text.RegularExpressions;


public partial class Entity : CharacterBody2D
{
	//entites are moving bodies that are affected by gravity
	//this is the base class for enemies, pikmin and captains

	public int gravityIndex = 0;

	public Planet residentPlanet = null;
	public GravityArea mainGravity = null;
	public bool newGravPriority = false;
	public bool gravPriorityLocked = false;

	public UInt16 state = 0;

	public UInt16 subState = 0;

	public AnimatedSprite2D anim = null;



	public Godot.Vector2 normalDir = Godot.Vector2.Zero;
	public Godot.Vector2 tangentDir = Godot.Vector2.Zero;

	public bool gravEmpty = true;


	public float normalVelocity = 0f;
	public float tangentVelocity = 0f;
	public float angle = 0f;

	public List<GravityArea> gravityAreas = new List<GravityArea>();


public override void _Ready()

	{
	}

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
	public float getProjection(Godot.Vector2 projecting, Godot.Vector2 projDir)
	{
		//projecting is the vector we want to project onto projDir
		//this works
		//trust me bro
		return (projecting.Dot(projDir)) / (projDir.Dot(projDir));
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

		public NodeConnection getValidConnection(NavNode navNode)
	{
		/*
		Accepts a NavNode and return the first NodeConnection
		that is of the walk type, otherwise returns null
		*/

		foreach(Node child in navNode.GetChildren())
		{
			if(child.GetType() != typeof(NodeConnection))
			{
				continue;
			}
			NodeConnection connection = (NodeConnection)child;
			if(connection.type != NodeConnection.ConnectionType.walk)
			{
				continue;
			}
			return connection;
			
		}

		return null;
	}

	public List<NodeConnection> getNextDestinations(NavNode prev, NavNode navNode)
	{
		/*
		Accepts two navNodes, prev and current
		prev is the navNode that was just iterated on
		we include it so that this doesn't return a path back to itself
		FIX THIS
		*/

		List<NodeConnection> connections = new List<NodeConnection>();
		foreach(Node child in navNode.GetChildren())
		{
			if(child.GetType() != typeof(NodeConnection))
			{
				continue;
			}
			NodeConnection connection = (NodeConnection)child;
			if(connection.type != NodeConnection.ConnectionType.walk)
			{
				continue;
			}
			return null;
			
		}

		return null;
	}

	public NavNode getClosestNavNode(Godot.Vector2 pos)
	{
		residentPlanet = (Planet)((GravityArea)gravityAreas[gravityIndex]).GetParent();

		NavNode closestNode = null;
		float closestDist = Mathf.Inf;
		foreach (Node child in residentPlanet.GetChildren())
		{
			if(child.GetType() != typeof(NavNode))
			{
				continue;
				//go to next node
			}
			if(pos.DistanceSquaredTo(((NavNode)child).GlobalPosition) < closestDist)
			{
				
			}
			
		

		}



		return null;
	}


	public NavPath getPathTo(NavNode startNode, NavNode destNode)
	{
		List<NavPath> paths = new List<NavPath>();


		return null;
	}
	public List<NavPath> getPathHelper(List<NavPath> navPaths)
	{
		

	}


}
