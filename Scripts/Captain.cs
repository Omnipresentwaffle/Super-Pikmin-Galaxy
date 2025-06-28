using Godot;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

public partial class Captain : Entity
{
	[Export]
	public int id = 0;
	//0 = olimar
	//1 = louie
	//2 = prez... etc
	public int team = 0;



	public bool active = false;
	public const float JumpVelocity = -700.0f;

	public Area2D chainDetector = null;



	public int gravityIndex = 0;

	public const float topSpeed = 800f;

	public Godot.Vector2 displacement = Godot.Vector2.Zero;

	public const float rotateSpeed = 200;

	public const float gravitySnap = 45f;

	public bool joinFollow = false;

	public bool targetAnglePositive = true;

	public int xFlip = 1;

	public int yFlip = 1;


	public const float groundRez = 300f;

	public const float airAccel = 1200f;

	public const float diveVelocity = 800f;

	public const float tanAccel = 20f;

	public const float jumpTime = 0.5f;
	public float jumpTimer = 0f;

	public const float fallMultiplier = 1.2f;

	public bool timeOut = false;
	public float prevAngle = 0f;

	public bool landed = false;
	public uint landedAdd = 0;

	public Godot.Vector2 prevKeyPress = Godot.Vector2.Zero;
	public Godot.Vector2 keyPress = Godot.Vector2.Zero;


	public bool chainHit = false;

	public float progress = 0f;

	ChainAttach hook = null;
	public Godot.Vector2 lineVector = Godot.Vector2.Zero;
	Line2D norLine = null;
	Line2D tanLine = null;


	Line2D velLine = null;
	public UInt16 state = 1;
	

	public Chain chain = null;
	public FollowPath followPath = null;

	public Follower follower = null;

	public override void _Ready()
	{
		hook = GetNode<ChainAttach>("ChainAttach");
		norLine = GetNode<Line2D>("NormalDirection");
		tanLine = GetNode<Line2D>("TangentDirection");
		velLine = GetNode<Line2D>("VelocityDirection");
		followPath = GetNode<FollowPath>("Follow");
		follower = GetNode<Follower>("Follower");
		chainDetector = GetNode<Area2D>("ChainDetector");
		base._Ready();
	}


	public override void _PhysicsProcess(double delta)
	{
		Godot.Vector2 velocity = Velocity;
		GravityArea prevGrav = mainGravity;



		if (prevAngle != angle)
		{
			if (Math.Abs(angle - prevAngle) >= 15)
			{
				//GD.Print("hugeChange");

			}
			prevAngle = angle;
		}


		if (gravEmpty)
		{
			GD.Print("gravEmpty");
		}

		Godot.Vector2 prevNormal = normalDir;



		if (!gravPriorityLocked)
		{
			if (mainGravity != null)
			{
				(normalDir, tangentDir, angle) = mainGravity.getDirections(GlobalPosition);

			}
			else
			{
				normalDir = Godot.Vector2.Down;
				angle = -(float)Math.PI / 4;
			}

		}

		if (state != 3 && state != 3)
		{
			(normalVelocity, tangentVelocity) = getMagnitudes(velocity, normalDir);
		}
		bool jumpPressed = false;


		prevKeyPress = keyPress;
		keyPress = Godot.Vector2.Zero;

		if (active)
		{
			jumpPressed = Input.IsActionPressed("jump");

			if (Input.IsActionPressed("right"))
			{
				keyPress.X += 1;
			}
			if (Input.IsActionPressed("left"))
			{
				keyPress.X += -1;
			}
			if (Input.IsActionPressed("up"))
			{
				keyPress.Y += -1;
			}
			if (Input.IsActionPressed("down"))
			{
				keyPress.Y += 1;
			}
		}




		if (newGravPriority)
		{
			//runs when a new gravity zone is acting on the player
			GD.Print("newGravPriority");
			newGravPriority = false;
			float angleDiff = prevNormal.AngleTo(normalDir);
			angleDiff *= 180 / (float)Math.PI;
			//GD.Print("angleDiff: ", angleDiff);
			if (Math.Abs(normalVelocity) >= 25)
			{
				if (Math.Abs(Math.Abs(angleDiff) - 180) <= 15)
				{
					GD.Print("xFlipped!");
					xFlip = -1;
				}
			}

		}

		tangentDir = getPerp(normalDir);







		UpDirection = -normalDir;
		RotationDegrees = angle;

		chainDetector.Monitorable = true;
		if (chainHit)
		{
			state = 3;
		}

		if (joinFollow)
		{
			joinFollow = false;
			state = 4;
		}


		switch (state)
		{
			case 0:
				//state walk


				if (keyPress.X != prevKeyPress.X)
				{
					xFlip = 1;
				}


				if (jumpPressed)
				{
					jump();

					//enter Jump
					state = 1;
					break;
				}

				if (keyPress.X == 0)
				{
					//not accelerating in any direction
					tangentVelocity = (float)Mathf.MoveToward(tangentVelocity, 0, groundRez * delta);
					//reset the xFLip
					xFlip = 1;
				}
				else
				{
					keyPress.X *= xFlip;
					if (Math.Sign(tangentVelocity) == Math.Sign(keyPress.X))
					{
						//accelerating in the same direction
						tangentVelocity += 500f * (float)keyPress.X * (float)delta;
					}
					else
					{
						//turning
						tangentVelocity += 1200f * (float)keyPress.X * (float)delta;
					}
					//limit the speed
					tangentVelocity = Math.Clamp(tangentVelocity, -800, 800);
				}

				if (landedAdd > 0)
				{
					followPath.addFollowPoint();
				}
				else
				{
					landed = false;
				}





				if (!IsOnFloor())
				{
					jumpTimer = 0f;
					state = 1;
					break;
				}

				if (normalVelocity < 0)
				{
					normalVelocity = 0;
				}
				displacement.X += tangentVelocity * (float)delta;
				if (Math.Abs(displacement.X) >= FollowPath.pointDistance)
				{
					followPath.addFollowPoint();
				}



				break;

			case 1:
				//state jump

				if (xFlip == -1)
				{
					if (keyPress.X != prevKeyPress.X)
					{
						GD.Print("xUNFLIPPED");
						xFlip = 1;
					}
				}



				if (IsOnCeiling())
				{
					//set the velocity to be downard and make it so that the jump doesnt rise anymore
					normalVelocity = +200f;
					timeOut = true;
				}

				if (!timeOut)
				{
					if (jumpPressed)
					{
						//holding to jump higherdw
						jumpTimer -= (float)delta;
						normalVelocity += mainGravity.gravityStrength * 0.3f * (float)delta;
						if (jumpTimer <= 0)
						{
							timeOut = true;
							break;
						}


					}
					else
					{
						timeOut = true;
					}


				}
				else
				{
					float fallMultiplier = 1f;
					if (keyPress.Y == 1)
					{
						//enter dive
						state = 2;
						normalVelocity += 200f;
						int dir = Math.Sign(keyPress.X);
						if (dir == 0)
						{
							dir = Math.Sign(tangentVelocity);
							if (dir == 0)
							{
								dir = 1;
							}
						}
						if (Math.Sign(tangentVelocity) != dir)
						{
							tangentVelocity = 0;
						}

						tangentVelocity += dir * diveVelocity;
						break;
					}
					else if (keyPress.Y == -1)
					{
						if (chain != null)
						{
							chainAttach();
							return;
						}
					}
					normalVelocity += mainGravity.gravityStrength * fallMultiplier * (float)delta;

				}


				if (keyPress.X == 0)
				{
					xFlip = 1;
				}
				else
				{
					tangentVelocity += keyPress.X * airAccel * (float)delta;
				}


				tangentVelocity = Math.Clamp(tangentVelocity, -1000, 1000);

				if (IsOnFloor())
				{
					//enter walk
					landed = true;
					landedAdd = (uint)(followPath.followers * 5) + 20;
					state = 0;
					break;
				}

				displacement.Y += normalVelocity;
				displacement.X += tangentVelocity;
				

				if (displacement.Length() >= FollowPath.pointDistance)
				{
					followPath.addFollowPoint();
				}




				break;

			case 2:
				normalVelocity += mainGravity.gravityStrength * fallMultiplier * (float)delta;
				//tangentVelocity += keyPress.X * 
				//diving

				if (IsOnFloor())
				{
					state = 0;
					break;
				}


				break;


			case 3:
				//state chain attach
				GD.Print("gravPriorityLocked: ", gravPriorityLocked);

				Int16 chainDirPress = 0;
				if (hook.state == 0)
				{
					//walk
					chainDirPress = (Int16)keyPress.X;
				}
				else
				{
					chainDirPress = (Int16)keyPress.Y;
				}

				chainDirPress *= (Int16)xFlip;
				//normalVelocity = 0;
				Godot.Vector2 progressVector = GlobalPosition - chain.start;
				hook.progress = getProjection(progressVector, chain.chainVectorFull);



				if (jumpPressed)
				{
					chainDetach();
					(normalVelocity, tangentVelocity) = getMagnitudes(Velocity, normalDir);
					jump();

					state = 1;
					break;
				}

				velocity = chainDirPress * hook.chainVector * topSpeed * (float)delta;
				displacement += velocity;

				if (displacement.Length() >= FollowPath.pointDistance)
				{
					followPath.addFollowPoint();
				}


				break;

			case 4:

				break;

		}



		if (state != 3)
		{
			velocity = normalDir * normalVelocity;
			velocity += tangentDir * tangentVelocity;
		}
		


		velLine.ClearPoints();
		velLine.AddPoint(Godot.Vector2.Zero);



		Velocity = velocity;

		if (state == 3)
		{
			//when on a chain
			MoveAndCollide(Velocity);
		}
		else
		{
			MoveAndSlide();

		}

		ApplyFloorSnap();

		if (mainGravity != prevGrav)
		{
			//GD.Print("GRAVAAA");
		}

		followPath.GlobalPosition = Godot.Vector2.Zero;
		followPath.GlobalRotationDegrees = 0f;

		GD.Print("landedAdd: ", landedAdd);




	}

	public void jump()
	{
		normalVelocity = JumpVelocity;
		normalVelocity -= Math.Abs(tangentVelocity) * 0.3f;
		jumpTimer = jumpTime;
		timeOut = false;
	}




	
	

	public void _on_chain_area_entered(Area2D area)
	{
		GD.Print("chainEntered");
		chain = (GetNode<ChainArea>(GetPathTo(area)).GetParent<Chain>());
		if (!chainCheck())
		{
			return;
		}

		GD.Print("chainAttachFrick");
		chainAttach();
		

		

	}
	public void _on_chain_area_exited(Area2D area)
	{
		state = 1;
		gravPriorityLocked = false;
		chain = null;
		chainHit = false;
		GD.Print("chainexit");
	}

	public void chainDetach()
	{
		if (hook.state == 0)
		{
			chainDetector.Monitorable = false;
		}
		chain = null;
		chainHit = false;
		gravPriorityLocked = false;
		mainGravity = gravityAreas[0];
		

	}

	public bool chainCheck()
	{
		int closest = 0;
		if (GlobalPosition.DistanceSquaredTo(chain.Points[0] + chain.GlobalPosition) > GlobalPosition.DistanceSquaredTo(chain.Points[1] + chain.GlobalPosition))
		{
			closest = 1;
		}


		if (closest == 0)
		{
			lineVector = chain.Points[1] - chain.Points[0];
			GD.Print("0");
		}
		else
		{
			lineVector = chain.Points[0] - chain.Points[1];
			GD.Print("1");

		}
		GD.Print("closest: ", closest);

		float checkAngle = (-UpDirection).AngleTo(lineVector);

		float pi = (float)Math.PI;
		float pi2 = 2 * pi;

		if (checkAngle < 0)
		{
			checkAngle += 2*pi;
		}
		GD.Print("checkAngle: ", checkAngle*180/(float)Math.PI);
		//chainHit = true;

		if (checkAngle > (5 * pi / 6) && checkAngle < (7 * pi / 6))
		{
			GD.Print("wall");
			hook.state = 1;
			return false;
		}
		else
		{
			GD.Print("floor");
			hook.state = 0;
			return true;

		}
	}



	public void chainAttach()
	{
		progress = getProjection(GlobalPosition - chain.GetPointPosition(0), chain.chainVectorFull);
		//we need to get the "right" direction relative to the rotation of the captain
		float posXAngle = normalDir.Angle();
		//posXAngle *= 180 / (float)Math.PI;
		//GD.Print("posXAngle: ", posXAngle);


		//rotate the vector by -90 degrees so that it is perpendicular
		if (hook.state == 0)
		{
			posXAngle -= (float)Math.PI / 2;

		}

		//convert the angle to a vector
		Godot.Vector2 posXvector = new Godot.Vector2((float)Math.Cos(posXAngle), (float)Math.Sin(posXAngle));

		hook.chainAttach(chain);
		normalVelocity = 0f;
		tangentVelocity = 0f;
		GlobalPosition = hook.start + (hook.chainVector * hook.progress);


		float prog = 0;
		prog = getProjection(posXvector, hook.chainVector);
		if (prog < 0)
		{
			xFlip = -1;
		}
		//GD.Print("prog: ", prog);

		gravPriorityLocked = true;
		chainHit = true;


	}


	public void smoothAngle(float delta)
	{
		
	}

	public float angleCorrected(float enterAngle) {
		if (enterAngle < 0)
		{
			enterAngle += 360;
		}
		return enterAngle;
	}
}
