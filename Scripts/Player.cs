using Godot;
using System;

public partial class Player : Camera2D
{
	// Called when the node enters the scene tree for the first time.
	public int id = 0;
	public uint captainIndex = 0;

	public bool swapPressed = false;

	public Captain currentCaptain = null;
	public Node currentScene = null;
	public Godot.Collections.Array<Node> captains = new Godot.Collections.Array<Node>();
	public override void _Ready()
	{
		currentScene = GetTree().CurrentScene;
		captains = GetTree().GetNodesInGroup("captain");
		currentCaptain = (Captain)captains[0];
		currentCaptain.active = true;



	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("swap_captain") && !swapPressed)
		{
			swapCaptain();

		}
		else
		{
			swapPressed = false;
		}
		this.Reparent(currentCaptain, false);
		Vector2 mouseDistance = GetGlobalMousePosition() - currentCaptain.GlobalPosition;
		Position = Vector2.Zero;
		GlobalPosition += mouseDistance / 10;


	}

	public void swapCaptain()
	{
		swapPressed = true;
		if (currentCaptain.state != 0)
		{
			return;
		}
		if (captains.Count <= 1)
		{
			return;
		}
		currentCaptain.active = false;
		captainIndex += 1;
		captainIndex %= (uint)captains.Count;
		currentCaptain = (Captain)captains[(int)captainIndex];
		currentCaptain.active = true;
		GlobalRotationDegrees = currentCaptain.GlobalRotationDegrees;
	}

	public void cameraLock()
	{
		IgnoreRotation = true;
	}


	public void _on_pikmin_whistled(Area2D area)
	{
		Pikmin pikmin = GetNode<Area2D>(GetPathTo(area)).GetParent<Pikmin>();
		if (pikmin.state != 0)
		{
			return;
		}
		GD.Print("pikmin");

		FollowPath followPath = currentCaptain.GetNode<FollowPath>("Follow");

		pikmin.follower.leader = currentCaptain;

		//increase followers and set the follower id
		followPath.followers += 1;
		pikmin.follower.id = followPath.followers;
		pikmin.follower.targetIndex = (ushort)(5 * pikmin.follower.id);
		pikmin.Reparent(this);
		pikmin.joinFollow = true;
		
		

	}

}
