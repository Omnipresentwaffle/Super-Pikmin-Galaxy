using Godot;
using System;
using System.Collections.Generic;

public partial class FollowPath : Line2D
{
	// Called when the node enters the scene tree for the first time.
	Godot.Vector2 prevPointPos = new Godot.Vector2(0, 0);

	public List<Passive> followerList = new List<Passive>();

	public bool squadLineLocked = false;

	public Godot.Vector2 squadLockedPos = new Godot.Vector2(0, 0);
	public const float pointDistance = 10f;

	public List<byte> stateList = new List<byte>();

	public List<JumpPath> jumpPaths = new List<JumpPath>();


	public Line2D squadLine = null;
	public const float squadLineMaxLength = 1000f;

	

	public float spacing = 5f;
	public const float maxSpacing = 25f;



	public UInt16 followers = 0;
	public override void _Ready()

	{
		Captain parent = GetParent<Captain>();
		prevPointPos = parent.GlobalPosition;
		squadLine = GetNode<Line2D>("SquadLine");
		setSquadLine();

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}


	public void followPathUpdate(bool reset = true, int index = -1)
	{
		if (followers <= 0)
		{
			return;
		}
		Captain parent = GetParent<Captain>();
		//this is a vector that represents the vector from start to end
		//(starts at front and ends at index 0)
		Godot.Vector2 squadVector = squadLine.GetPointPosition(squadLine.Points.Length - 1) - squadLine.GetPointPosition(0);
		

		Godot.Vector2 posToFront = parent.GlobalPosition - (squadLine.GetPointPosition(0) + squadLine.GlobalPosition);
		float t = parent.getProjection(posToFront, squadVector);
		Line2D norline = parent.GetNode<Line2D>("NormalDirection");
		norline.ClearPoints();
		norline.RotationDegrees = 0;
		norline.GlobalPosition = squadLine.GetPointPosition(0) + squadLine.GlobalPosition;
		norline.AddPoint(Vector2.Zero);
		norline.AddPoint(posToFront);
		if (t < 0 || t > 1)
		{
			squadLockedPos = parent.GlobalPosition;
			if (t >= 1)
			{
				squadLockedPos -= squadVector;
			}
			squadLine.GlobalPosition = squadLockedPos;
		}

	}

	public void addFollowPoint(Vector2 pos, int idx = 0)
	{
		Captain parent = GetParent<Captain>();
		if (parent.state == 0)
		{
			return;
		}
		if (jumpPaths.Count == 0)
		{
			return;
		}


		Line2D curPath = jumpPaths[jumpPaths.Count - 1];

		if (parent.timeOut)
		{
			stateList.Add(0);
		}
		else
		{
			stateList.Add(1);

		}
		curPath.AddPoint(pos, idx);

		int len = Points.Length;
		return;
		if (len >= 2500)
		{
			jumpPaths[0].RemovePoint(len - 1);
			stateList.RemoveAt(len - 1);
		}

	}

	public void addSquadPoint(Vector2 pos, int idx = 0)
	{
		squadLine.AddPoint(pos, idx);
	}

	public void setSquadLine(bool flip = false)
	{
		//here is the code where we set the squad line
		if (followers == 0)
		{
			return;
		}
		Captain parent = GetParent<Captain>();
		Godot.Vector2 pos = Vector2.Zero;

		uint repeat = (uint)(followers + 2);

		spacing = maxSpacing;
		if (spacing * followers >= squadLineMaxLength)
		{
			spacing = squadLineMaxLength / followers;
		}

		GD.Print("followers: ", followers);
		Vector2 dir = parent.tangentDir;
		dir = dir.Normalized();
		GD.Print("dir: ", dir);

		Vector2 posAdd = (dir * spacing);
		if (flip)
		{
			GD.Print("flipSquadLine");
			posAdd = -posAdd;
		}
		GD.Print("posAdd: ", posAdd);

		squadLine.ClearPoints();
		for (int i = 0; i < repeat; i += 1)
		{
			squadLine.AddPoint(pos);
			pos += posAdd;
		}

		squadLineLocked = true;
		squadLockedPos = parent.GlobalPosition;
		GlobalPosition = parent.GlobalPosition;

	}
	public void landed(bool flip = false)
	{
		if (followers <= 0)
		{
			return;
		}
		Captain parent = GetParent<Captain>();
		//we want it to be that index 0 is right behind the captain
		//so the larger the index the further away from the captain
		Godot.Vector2 squadVector = ToGlobal(squadLine.GetPointPosition(squadLine.Points.Length - 1)) - ToGlobal(squadLine.GetPointPosition(0));

		Godot.Vector2 posToFront = parent.GlobalPosition - ToGlobal((squadLine.GetPointPosition(0)));
		float t = parent.getProjection(posToFront, squadVector);
		setSquadLine();
		jumpPaths[jumpPaths.Count - 1].complete = true;
		return;
		if (t < 0 || t > 1)
		{
			setSquadLine();
		}
	}

	public void newPath()
	{
		jumpPaths.Add(new JumpPath());
		GD.Print("jumpPathsLength: ", jumpPaths.Count);
	}
	
}
