using Godot;
using System;

public partial class ChainAttach : Node2D
{

	public float progress = 0f;
	public Vector2 chainVector = Vector2.Up;
	public Vector2 start = Vector2.Zero;

	public UInt16 state = 0;

	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
	}

	public void chainAttach(Chain chain)
	{
		start = chain.GetPointPosition(0) + chain.GlobalPosition;
		progress = getProjection(GlobalPosition - start, chain.chainVectorFull);
		chainVector = chain.chainVectorFull.Normalized();


	}

	public float getProjection(Godot.Vector2 projected, Godot.Vector2 projDir){
		//projected is the vector we want to project onto projDir
		float projection = 0;
		float dotProd = projected.Dot(projDir);
		float magnitude = projDir.Length();

		projection = dotProd / magnitude;

		return projection;
	}


}
