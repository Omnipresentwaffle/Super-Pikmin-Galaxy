using Godot;
using System;
using System.Drawing;
using System.Numerics;

public partial class Passive : Entity
{
	//class that contains data and scripts that apply to both pikmin and captains
	//pikmin and captain inherit from this class
	public FollowPath followPath = null;
	public Follower follower = null;
	public UInt16 team = 0;


	public UInt16 followerId = 0;
	//the number of the follower
	public uint targetIndex = 0;
	//the point the follower prioritizes

	public Godot.Vector2 targetPos =  Godot.Vector2.Zero;


	public UInt16 order = 0;

	public UInt16 nextPathIdx = 0;

	public JumpPath nextPath = null;

	public float speedConst = 800f;

	public Captain leader = null;

	public FollowState followState = FollowState.walk;

	public Line2D line = null;


	public enum FollowState
	{
		join,
		walk,
		jump,
		fall,
		held
	}

	public bool whistleLocked = false;

	public bool joinFollow = false;
	public override void _Ready()
	{
		line = GetNode<Line2D>("NormalDirection");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public Godot.Vector2 follow(float delta)
	{
		FollowPath followPath = leader.followPath;
		Line2D squadLine = followPath.squadLine;
		Godot.Vector2 dirVector = targetPos - GlobalPosition;
		float speed = speedConst;
		switch (followState)
		{
			case FollowState.join:

				return Velocity;


		
		}
			return Velocity;

	}


	public Godot.Vector2 stateJoin()
	{
		return Godot.Vector2.Zero;
	}
}
