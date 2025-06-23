using Godot;
using System;

public partial class FollowPath : Line2D
{
	// Called when the node enters the scene tree for the first time.

	public const float pointDistance = 50f;
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}


	public void addFollowPoint(int index = -1)
	{
		Captain parent = GetParent<Captain>();
		AddPoint(parent.GlobalPosition, index);
		parent.displacement = Vector2.Zero;
		
		if (Points.Length >= 200)
		{
			RemovePoint(0);
		}
	}
	
}
