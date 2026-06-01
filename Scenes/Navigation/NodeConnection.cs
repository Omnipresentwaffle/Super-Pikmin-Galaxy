using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
[Tool]
public partial class NodeConnection : Line2D
{
	// Called when the node enters the scene tree for the first time.


	[Export]
	public NavNode destination = null;

	[Export]
	ConnectionType type = ConnectionType.walk;
	public enum ConnectionType
	{
		walk,
		jump,

		fall,
		chain
	}
	public override void _Ready()
	{

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void _EnterTree()
{
	GD.Print("SCRIPT ENTERED TREE:", GetType());
}
}
