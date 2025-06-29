using Godot;
using System;
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

	public float speedConst = 800f;

	public Captain leader = null;

	public State followState = State.walk;

	public Line2D line = null;

	public enum State
	{
		walk,
		jump
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
				GD.Print("id: ", id);

				targetPos = squadLine.GetPointPosition((int)(id)+1);
				dirVector = targetPos - GlobalPosition;

				(normalDir, tangentDir, angle) = mainGravity.getDirections(GlobalPosition);
				tangentDir = getPerp(normalDir);
				GlobalRotationDegrees = angle;

				float t = getProjection(dirVector, tangentDir);
				dirVector = tangentDir * t;
				if (speed > dirVector.Length())
				{
					speed = dirVector.Length();
				}
				dirVector = dirVector.Normalized();
				

				return dirVector * speed;


		}

		return Velocity;
		
	}
}
