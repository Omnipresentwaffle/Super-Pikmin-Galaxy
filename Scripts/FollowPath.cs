using Godot;
using System;

public partial class FollowPath : Line2D
{
	// Called when the node enters the scene tree for the first time.

	public const float pointDistance = 1f;


	public UInt16 followers = 0;
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}


	public void addFollowPoint(bool reset = true, int index = -1)
	{
		
		Captain parent = GetParent<Captain>();
		Vector2 displacement = parent.displacement;

		if (parent.landedAdd > 0)
		{
			parent.landedAdd -= 1;
		}
		if (reset)
		{
			parent.displacement = Vector2.Zero;
		}
		else
		{
			parent.displacement.X += -Math.Sign(displacement.X) * pointDistance;
			parent.displacement.Y += -Math.Sign(displacement.Y) * pointDistance;
		}
		
		AddPoint(parent.GlobalPosition, index);
		
		
		if (Points.Length >= 1500)
		{
			RemovePoint(0);
		}
	}
	
}
