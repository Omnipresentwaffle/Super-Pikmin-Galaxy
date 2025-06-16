using Godot;
using System;
using System.Text.RegularExpressions;

public partial class FakePlayer : Entity
{
	public const float JumpVelocity = -600.0f;

	public const float topSpeed = 800f;

	public const float tanAccel = 20f;
	Line2D norLine = null;
	Line2D tanLine = null;
	UInt16 state = 1;

	public override void _Ready()
	{
		norLine = GetNode<Line2D>("NormalDirection");
		tanLine = GetNode<Line2D>("TangentDirection");


		base._Ready();
	}


	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;
		Vector2 addVelocity = Vector2.Zero;

		
		Vector2 keyPress = Vector2.Zero;
		keyPress.X = 0;
		if (Input.IsActionPressed("right")){
			keyPress.X -= 1;
		}
		if (Input.IsActionPressed("left")){
			keyPress.X += 1;
		}
		
		//(normalDir, angle) = gravityArea.getNormal(Position);
		tangentDir = getPerp(normalDir);

		UpDirection = -normalDir;
		RotationDegrees = (float)(angle * 180/Math.PI) - 90f;
		norLine.GlobalRotationDegrees = 0;
		tanLine.GlobalRotationDegrees = 0;



		switch (state){
			case 0:
			//walking
			normalVelocity = 0;
				if (Input.IsActionPressed("jump")){
					normalVelocity = -1200f;
					//enterJump
					state = 1;
					break;
				}
				if (keyPress.X != 0)
				{
					tangentVelocity = (float)Mathf.MoveToward(tangentVelocity, topSpeed*keyPress.X, 700*delta);
				}
				else
				{

					tangentVelocity = (float)Mathf.MoveToward(tangentVelocity, 0, 500*delta);
				}

				if(!IsOnFloor()){
					//enter jump
					state=1;
					break;
				}
				if(IsOnCeiling()){
					normalVelocity = +200f;
				}

			
				break;

			case 1:
			//jumping

			normalVelocity += 1200f * (float)delta;

			tangentVelocity += keyPress.X * 20f;
			tangentVelocity = Mathf.Clamp(tangentVelocity, -800, 800);

			if (IsOnFloor()){
				//enter walk
				state = 0;
				break;
			}


			break;


		}


		
		
		norLine.ClearPoints();
		norLine.AddPoint(Vector2.Zero);
		norLine.AddPoint(normalDir*150);
		tanLine.ClearPoints();
		tanLine.AddPoint(Vector2.Zero);
		tanLine.AddPoint(tangentDir*150);

		velocity = normalDir*normalVelocity;
		velocity += tangentDir*tangentVelocity;


		


		
		
		//
		//GD.Print("velocity: ", velocity);
		GD.Print("tanV: ", tangentVelocity);
		
		Velocity = velocity;
				

		MoveAndSlide();
		ApplyFloorSnap();
		
	}


}
