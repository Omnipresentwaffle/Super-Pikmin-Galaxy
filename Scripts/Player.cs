using Godot;
using System;
using System.Linq;

public partial class Player : Camera2D
{
	// Called when the node enters the scene tree for the first time.
	public int id = 0;
	public uint captainIndex = 0;

	public bool swapPressed = false;

	public Captain curCaptain = null;
	//current captain being controlled
	public Node currentScene = null;
	public Godot.Collections.Array<Node> captains = new Godot.Collections.Array<Node>();
	public override void _Ready()
	{
		currentScene = GetTree().CurrentScene;
		captains = GetTree().GetNodesInGroup("captain");
		curCaptain = (Captain)captains[0];
		curCaptain.active = true;
		curCaptain.whistleLocked = true;



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
		this.Reparent(curCaptain, false);
		Vector2 mouseDistance = GetGlobalMousePosition() - curCaptain.GlobalPosition;
		Position = Vector2.Zero;
		GlobalPosition += mouseDistance / 10;

		squadShow();

		if (curCaptain.throwHold)
		{
			//if the captain is holding a pikmin
			if (Input.IsActionPressed("whistle"))
			{
				curCaptain.throwHold = false;
			}
		}

		if (Input.IsActionPressed("throw"))
		{
			curCaptain.grabFollower(curCaptain.followPath.followerList[0]);
		}
		else
		{
			curCaptain.throwHold = false;
		}
		

	}

	public void squadShow()
	{
		Line2D squadLine = curCaptain.followPath.squadLine;
		if (squadLine.Points.Length == 0)
		{
			return;
		}
		Godot.Vector2 squadVector = squadLine.GetPointPosition(squadLine.Points.Length - 1) - squadLine.GetPointPosition(0);
		//GlobalPosition += squadVector / 5;
		
	}

	public void swapCaptain()
	{
		swapPressed = true;
		if (curCaptain.state != 0)
		{
			return;
		}
		if (captains.Count <= 1)
		{
			return;
		}
		curCaptain.active = false;
		curCaptain.whistleLocked = false;
		captainIndex += 1;
		captainIndex %= (uint)captains.Count;
		curCaptain = (Captain)captains[(int)captainIndex];
		curCaptain.active = true;
		curCaptain.whistleLocked = true;
		//GlobalRotationDegrees = curCaptain.GlobalRotationDegrees;
	}

	public void cameraLock()
	{
		IgnoreRotation = true;
	}


	public void _on_follower_whistled(Area2D area)
	{
		//********************IMPORTANT******************************
		//here we add the pikmin/captain to follow the captain
		Passive follower = GetNode<Area2D>(GetPathTo(area)).GetParent<Passive>();

		if (follower.whistleLocked)
		{
			return;
		}

		if (follower.state != 0)
		{
			return;
		}
		GD.Print("pikmin");

		FollowPath followPath = curCaptain.GetNode<FollowPath>("FollowPath");

		follower.leader = curCaptain;

		//increase followers and set the follower id
		follower.targetIndex = followPath.followers;
		follower.id = followPath.followers;
		followPath.followers += 1;
		followPath.followerList.Add(follower);


		follower.joinFollow = true;

		followPath.setSquadLine();

		GD.Print("followerList: ", curCaptain.followPath.followerList);
		
		

	}

}
