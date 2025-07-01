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


	public UInt16 id = 0;
	//the number of the follower
	public uint targetIndex = 0;
	//the point the follower prioritizes

	public Godot.Vector2 targetPos =  Godot.Vector2.Zero;


	public UInt16 order = 0;

	public UInt16 nextPathIdx = 0;

	public JumpPath nextPath = null;

	public float speedConst = 800f;

	public Captain leader = null;

	public State followState = State.walk;

	public Line2D line = null;


	public enum State
	{
		walk,
		jump,
		fall
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
		//Passive entity = this;
		FollowPath followPath = leader.followPath;
		Line2D squadLine = followPath.squadLine;
		//targetPos = leader.followPath.GetPointPosition((int)(followPath.Points.Length - targetIndex));
		Godot.Vector2 dirVector = targetPos - GlobalPosition;
		float speed = speedConst;
		switch (followState)
		{
			case State.walk:
				line = GetNode<Line2D>("NormalDirection");
				//distance = v * t = m/s * s = m

				targetPos = squadLine.GetPointPosition((int)(id) + 1) + squadLine.GlobalPosition;
				dirVector = targetPos - GlobalPosition;

				(normalDir, tangentDir, angle) = mainGravity.getDirections(GlobalPosition);
				tangentDir = getPerp(normalDir);
				GlobalRotationDegrees = angle;
				float t = getProjection(dirVector, tangentDir);
				Godot.Vector2 targetVector = tangentDir * t;
				Godot.Vector2 velocityVector = tangentDir * Math.Sign(t) * speed;


				Godot.Vector2 moveVector = velocityVector * delta;


				if (moveVector.Length() > targetVector.Length())
				{
					speed = targetVector.Length() / delta;
					velocityVector = tangentDir * Math.Sign(t) * speed;

				}

				if (leader.state == 1)
				{
					nextPathIdx = (ushort)((ushort)followPath.jumpPaths.Count - 1);
					nextPath = followPath.jumpPaths[nextPathIdx];

				}
				if (nextPath != null)
				{
					targetIndex = (uint)(5 * ((int)(id) + 1));

					if (nextPath.Points.Length >= targetIndex)
					{
						followState = State.jump;
						break;
					}
					if (nextPath.complete)
					{
						followState = State.fall;
						break;

					}
					else
					{
					}
						
				}
				velocityVector += normalDir * 500f;
				return velocityVector;

			case State.jump:
				targetPos = nextPath.GetPointPosition((int)targetIndex);

				GlobalPosition = targetPos;

				if (leader.state == 0)
				{
					followState = State.fall;

				}


				return Godot.Vector2.Zero;

			case State.fall:
				targetIndex -= 1;
				if (targetIndex > nextPath.Points.Length)
				{
					return Godot.Vector2.Zero;
				}

				if (targetIndex <= 0)
				{
					followState = State.walk;
					nextPath = null;
					return Godot.Vector2.Zero;
				}
				targetPos = nextPath.GetPointPosition((int)targetIndex);
				GlobalPosition = targetPos;
				
				return Godot.Vector2.Zero;



		}

		return Velocity;
		
	}
}
