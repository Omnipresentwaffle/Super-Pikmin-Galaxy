using Godot;
using System;


[Tool]

public partial class DataCopier : CollisionPolygon2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}



	public void copyData()
	{

		Line2D surface = GetParent().GetParent().GetNode<Line2D>("Surface");

		surface.Points = Polygon;

	}

}
