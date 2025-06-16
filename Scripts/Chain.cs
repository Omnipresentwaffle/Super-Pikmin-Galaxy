using Godot;
using System;
using System.Drawing;

public partial class Chain : Line2D
{
	// Called when the node enters the scene tree for the first time.

	public const int hboxWidth = 20;
	public Vector2 chainVectorFull = Vector2.Zero;

	public Vector2 start = Vector2.Zero;


	public Path2D path = null;



	public override void _Ready()
	{
		setHitbox();

	}

	public void setHitbox()
	{
		start = GetPointPosition(0);
		Godot.Vector2 end = GetPointPosition(1);
		chainVectorFull = end - start;
		float length = Math.Abs(start.DistanceTo(end));
		GD.Print("len: ", length);

		Area2D body = GetNode<Area2D>("ChainArea");
		CollisionShape2D hitbox = body.GetNode<CollisionShape2D>("CollisionShape2D");
		RectangleShape2D shape = (RectangleShape2D)hitbox.Shape;
		shape.Size = new Godot.Vector2(hboxWidth, length);



		Godot.Vector2 dist = new Godot.Vector2(end.X - start.X, end.Y - start.Y);

		body.GlobalPosition = ToGlobal(start);
		body.Position += dist / 2;
		body.GlobalRotation = start.AngleToPoint(end);
		body.GlobalRotationDegrees += 90f;

		start = GlobalPosition + GetPointPosition(0);

		GD.Print("pntCount: ", GetPointCount());

	}


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}


	}
