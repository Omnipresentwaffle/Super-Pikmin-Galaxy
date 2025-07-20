using Godot;
using System;

public partial class AutoRotate : Node
{
	// Called when the node enters the scene tree for the first time.

	public Node2D parent = null;

	[Export]
	public float speed = 15;
	public override void _Ready()
	{
		parent = (Node2D)GetParent();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		parent.RotationDegrees += (float)(speed * delta);
		if (parent.RotationDegrees >= 360)
		{
			parent.RotationDegrees -= 360;
		}
		else if (parent.RotationDegrees < 0)
		{
			parent.RotationDegrees += 360;
		}
	}
}
