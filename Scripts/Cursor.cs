using Godot;
using System;

public partial class Cursor : Area2D
{
	// Called when the node enters the scene tree for the first time.

	public const float whistleTime = 2f;
	public const float timeToFull = 0.5f;

	public bool timeOut = false;

	public const float minRad = 64f;
	private const float maxRad = 450f;

	public float slope = 0f;

	public double timer = 0f;

	public bool whistlePress = false;
	public bool whistling = false;

	public Area2D whistleArea = null;
	public CollisionShape2D whistleHbox = null;
	public override void _Ready()
	{
		whistleArea = GetNode<Area2D>("WhistleHitbox");
		whistleHbox = whistleArea.GetNode<CollisionShape2D>("CollisionShape2D");
		slope = maxRad - minRad;
		slope /= (float)timeToFull;


	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		GlobalPosition = GetGlobalMousePosition();
		whistleHandle(delta);


	}

	public void whistleHandle(double delta)
	{
		CircleShape2D shape = (CircleShape2D)whistleHbox.Shape;
		if (!Input.IsActionPressed("whistle"))
		{
			shape.Radius = minRad;
			whistlePress = false;
			timeOut = false;
			return;
		}
		if (timeOut)
		{
			return;
		}
		if (!whistlePress)
		{
			shape.Radius = minRad;
			timer = 0f;
			whistlePress = true;
			
		}

		timer += delta;

		if (timer > whistleTime)
		{
			//GD.Print("TIMEOUT");
			timeOut = true;
			shape.Radius = minRad;
			timer = 0f;
		}
		else if (timer >= timeToFull)
		{
			shape.Radius = maxRad;
		}
		else
		{
			shape.Radius = (float)(minRad + (timer * slope));

		}




	}
	
}
