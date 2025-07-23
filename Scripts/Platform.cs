using Godot;
using System;

public partial class Platform : Node2D
{
	// Called when the node enters the scene tree for the first time.

	[Export]
	Line2D surface = new Line2D();

	[Export]
	Script executeSurface = null;
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
