using Godot;
using System;
[Tool]
public partial class Platform : Node2D
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
		pasteSurface();
	}

	public void pasteSurface()
	{
		StaticBody2D body = GetNode<StaticBody2D>("StaticBody2D");
		CollisionPolygon2D shape = body.GetNode<CollisionPolygon2D>("CollisionPolygon2D");

		Line2D surface = GetNode<Line2D>("Surface");
		surface.Points = shape.Polygon;

	}
}
