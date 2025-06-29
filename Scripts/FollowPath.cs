using Godot;
using System;
using System.Collections.Generic;

public partial class FollowPath : Line2D
{
	// Called when the node enters the scene tree for the first time.
	Godot.Vector2 prevPointPos = new Godot.Vector2(0, 0);
	public const float pointDistance = 5f;

	public List<Passive.State> stateList = new List<Passive.State>();


	public Line2D squadLine = null;
	public const float squadLineLength = 40f;

	public float spacing = 5f;
	public const float maxSpacing = 50f;



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
		Godot.Vector2 squadVector = squadLine.GetPointPosition(Points.Length - 1) - squadLine.GetPointPosition(0);
		Godot.Vector2 posToFront = parent.GlobalPosition - squadLine.GetPointPosition(0);
		float t = parent.getProjection(posToFront, squadVector);
		if (t > 1)
		{
			//addSquadPoint(parent.GlobalPosition, -1);
		}

	}

	public void addFollowPoint(Vector2 pos, int idx = 0)
	{
		Captain parent = GetParent<Captain>();

		stateList.Add((Passive.State)parent.state);
		AddPoint(pos, idx);


		if (Points.Length >= 1500)
		{
			RemovePoint(0);
			stateList.RemoveAt(0);
		}

	}

	public void addSquadPoint(Vector2 pos, int idx = 0)
	{
		
	}

	public void setSquadLine()
	{
		if (followers == 0)
		{
			return;
		}
		Captain parent = GetParent<Captain>();
		Godot.Vector2 pos = parent.GlobalPosition;
		spacing = squadLineLength / followers;
		uint repeat = followers;
		spacing = maxSpacing;

		GD.Print("followers: ", followers);
		Vector2 dir = parent.tangentDir;
		dir = dir.Normalized();
		GD.Print("dir: ", dir);
		Vector2 prevDir = dir;

		Vector2 posAdd = (dir * spacing);

		squadLine.ClearPoints();
		for (int i = 0; i < repeat + 1; i += 1)
		{


			squadLine.AddPoint(pos);
			if (prevDir != dir)
			{
				prevDir = dir;
				GD.Print("uhOh");
			}
			pos += posAdd;
			


		}
		GlobalPosition = Vector2.Zero;
		GlobalRotationDegrees = 0f;
	}
	
}
