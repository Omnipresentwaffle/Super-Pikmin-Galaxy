using Godot;
using System;

public partial class Follower : Node
{
	// Called when the node enters the scene tree for the first time.

	public UInt16 id = 0;
	//the number of the follower
	public uint targetIndex = 0;
	//the point the follower prioritizes

	public Captain leader = null;

	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
