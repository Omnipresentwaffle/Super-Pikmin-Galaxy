using Godot;
using System;

public partial class Cursor : Area2D
{
	// Called when the node enters the scene tree for the first time.

	public const double whistleTime = 3f;
	public const double timeToFull = 1f;

	public bool timeOut = false;

	public const float minRad = 64f;
	private const float maxRad = 128f;

	public float slope = 0f;

	public double timer = 0f;
	public bool whistling = false;

	public Area2D whistleArea = null;
	public CollisionShape2D whistleHbox = null;
	public override void _Ready()
	{
		whistleArea = GetNode<Area2D>("WhistleHitbox");
		whistleHbox = whistleArea.GetNode<CollisionShape2D>("CollisionShape2D");
		slope = maxRad - minRad;


	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		GlobalPosition = GetGlobalMousePosition();
		whistleHandle(delta);
	

	}

	public void whistleHandle(double delta) {
		CircleShape2D shape = (CircleShape2D)whistleHbox.Shape;
		if (!whistling)
		{
			timer = 0;
		}
		if (Input.IsActionPressed("whistle"))
		{
			timer += delta;
			if (timer > whistleTime)
			{
				timeOut = true
			}
			if (timer >= timeToFull)
			{
				shape.Radius = maxRad;
			}
			else
			{
				shape.Radius = minRad + (slope * (float)timer);
			}
		}
	}
}
