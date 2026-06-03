using Godot;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Numerics;
[Tool]
public partial class Planet : Node2D
{
	//this is the base class for all walkable collision bodies in this game
	//the goal of this approach is to have a surface map that can be accessed
	//by the pikmin which can tell them where to stand
	[Export]
	Line2D surface = new Line2D();

	public override void _Ready()
	{
		surface = GetNode<Line2D>("Surface");
	}



	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Engine.IsEditorHint())
		return;

	}



	[ExportToolButton("SetGrav")]
	public Callable DoThingButton => Callable.From(SetGrav);

	private void SetGrav()
	{
		Area2D gravity = GetNode<Area2D>("complexGravity");
		CollisionPolygon2D collisionPolygon = GetNode<StaticBody2D>("StaticBody2D").GetNode<CollisionPolygon2D>("CollisionPolygon2D");
		uint polyLen = (uint)collisionPolygon.Polygon.Length;

		Line2D gravityLine = gravity.GetNode<Line2D>("curve");
		gravityLine.ClearPoints();

		for(uint j = 0; j < polyLen; ++j)
		{
			gravityLine.AddPoint(collisionPolygon.Polygon[j] - gravity.Position);
			
		}

	}

	[ExportToolButton("SetNavigation")]
	public Callable SetNavButton => Callable.From(SetNavigation);

	private void SetNavigation()
	{
		
		//this function automatically sets the navigation nodes to just 
		//be a copy of the gravity line
		Line2D gravityLine = GetNode<Area2D>("complexGravity").GetNode<Line2D>("curve");
		foreach (Node child in GetChildren()) {
			if(child.GetType() == typeof(NavNode))
			{
				child.Free();
			}


		}


		PackedScene scene = GD.Load<PackedScene>("res://Scenes/Navigation/NavNode.tscn");
		PackedScene connScene = GD.Load<PackedScene>("res://Scenes/Navigation/NodeConnection.tscn");


		List<NavNode> navNodeList = new();

		for(int i = 0; i < gravityLine.Points.Count(); ++i)
		{
			NavNode navNode = scene.Instantiate<NavNode>();
			navNode.Position = gravityLine.GetPointPosition(i);
			navNode.Name = $"navNode{i}";
			AddChild(navNode);
			navNodeList.Add(navNode);
			navNode.Owner = GetTree().EditedSceneRoot;


		};

		for(int i = 0; i < navNodeList.Count(); ++i)
		{
			GD.Print($"NavNodeList: ", navNodeList[i].Name);

		}


		GD.Print("connScene.ResourcePath: ", connScene.ResourcePath);
		GD.Print("connScene.Instantiate().GetType(): ", connScene.Instantiate().GetType());


		

		GD.Print("Great success!");

		for(int i = 0; i < gravityLine.Points.Count(); ++i)
		{
			NodeConnection connection = connScene.Instantiate<NodeConnection>();
			foreach (Node child in navNodeList[i].GetChildren()) {
			if(child.GetType() == typeof(NodeConnection))
			{
				child.Free();
			}


			}



			NavNode next = navNodeList[(i+1)%navNodeList.Count];
			NavNode prev = navNodeList[Math.Abs((i-1)%navNodeList.Count)];



			connection.Name = "ConnectionFwd";
			connection.GlobalPosition = navNodeList[i].GlobalPosition;

			connection.AddPoint(Godot.Vector2.Zero);
			connection.AddPoint(next.GlobalPosition-connection.GlobalPosition);
			connection.destination = next;
			navNodeList[i].AddChild(connection);
			connection.Position = Godot.Vector2.Zero;
			connection.Owner = GetTree().EditedSceneRoot;


			connection = connScene.Instantiate<NodeConnection>();
			connection.Name = "ConnectionRev";

			connection.GlobalPosition = navNodeList[i].GlobalPosition;

			connection.AddPoint(Godot.Vector2.Zero);
			connection.AddPoint(prev.GlobalPosition-connection.GlobalPosition);
			connection.destination = prev;
			navNodeList[i].AddChild(connection);
			connection.Position = Godot.Vector2.Zero;
			connection.Owner = GetTree().EditedSceneRoot;



	
			

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

	public Godot.Vector2 getGroundCast(Godot.Vector2 capPos){
		/*
		Accepts the global position of the captain
		Loops thru all the nodes under the planet and then checks if they have valid
		NodeConnections, at which point it does a cast to get the closest point on the line
		This is seperate from gravity since a point that is a gravity source may not necessarily be
		the surface of the planet or it could be a hazard
		Returns the closest walkable position

		currently this prefers that the Fwd connections all go in the same direction
		and the Rev directions all go the same
		so uhhhh... for now assume that Fwd is clockwise and Rev is CCW
		and that ConnectionFwd should be the first child of a NavNode
		*/

		Godot.Vector2 closestPos = Godot.Vector2.Inf;
		float closestDist = Mathf.Inf;
		foreach (Node child in GetChildren())
		{
			if(child.GetType() != typeof(NavNode))
			{
				continue;
				//go to next node
			}
			NodeConnection connection = getValidConnection((NavNode)child);
			if(connection == null)
			{
				continue;
			}
			Godot.Vector2 projecting = capPos - ((NavNode)child).GlobalPosition;
			Godot.Vector2 projDir = connection.GetPointPosition(1) - connection.GetPointPosition(0);					
			float projProgress = getProjection(projecting, projDir);
			projDir *= projProgress;
			Godot.Vector2 groundCast = ((NavNode)child).GlobalPosition + projDir;
			if(capPos.DistanceSquaredTo(groundCast) < closestDist)
			{
				closestPos = groundCast;
				closestDist = capPos.DistanceSquaredTo(groundCast);
			}

		}

		return closestPos;
		


	}

	public float getProjection(Godot.Vector2 projecting, Godot.Vector2 projDir)
	{
		//projecting is the vector we want to project onto projDir
		//this works
		//trust me bro
		return (projecting.Dot(projDir)) / (projDir.Dot(projDir));
	}
}
