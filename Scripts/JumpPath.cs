using Godot;
using System;

public partial class JumpPath : Line2D
{
	public bool complete = false;

	public UInt16 followers = 0;
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
