using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
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

			connection.AddPoint(Vector2.Zero);
			connection.AddPoint(next.GlobalPosition-connection.GlobalPosition);
			connection.destination = next;
			navNodeList[i].AddChild(connection);
			connection.Position = Vector2.Zero;
			connection.Owner = GetTree().EditedSceneRoot;


			connection = connScene.Instantiate<NodeConnection>();
			connection.Name = "ConnectionRev";

			connection.GlobalPosition = navNodeList[i].GlobalPosition;

			connection.AddPoint(Vector2.Zero);
			connection.AddPoint(prev.GlobalPosition-connection.GlobalPosition);
			connection.destination = prev;
			navNodeList[i].AddChild(connection);
			connection.Position = Vector2.Zero;
			connection.Owner = GetTree().EditedSceneRoot;



	
			

		}	

	}
}
