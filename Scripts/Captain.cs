using Godot;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text.RegularExpressions;

public partial class Captain : Entity
{
	[Export]
	public int id = 0;
	//0 = olimar
	//1 = louie
	//2 = prez... etc
	public int team = 0;
	public const float JumpVelocity = -700.0f;
	public GravityArea mainGravity = null;

	public bool gravEmpty = true;

	


	public int gravityIndex = 0;
	public List<GravityArea> gravityAreas = new List<GravityArea>();

	public const float topSpeed = 800f;

	public const float rotateSpeed = 200;

	public const float gravitySnap = 45f;

	public bool targetAnglePositive = true;

	public int xFlip = 1;

	public int yFlip = 1;

	public bool newGravPriority = false;

	public const float groundRez = 300f;

	public const float airAccel = 1200f;

	public const float diveVelocity = 800f;

	public const float tanAccel = 20f;

	public const float jumpTime = 0.5f;
	public float jumpTimer = 0f;

	public const float fallMultiplier = 1.2f;

	public bool timeOut = false;
	public float prevAngle = 0f;

	public Godot.Vector2 prevKeyPress = Godot.Vector2.Zero;
	public Godot.Vector2 keyPress = Godot.Vector2.Zero;


	public bool chainHit = false;

	public float progress = 0f;

	ChainAttach hook = null;
	public Godot.Vector2 lineVector = Godot.Vector2.Zero;
	Line2D norLine = null;
	Line2D tanLine = null;

	public GrooveJoint2D joint = null;

	Line2D velLine = null;
	UInt16 state = 1;

	public Chain chain = null;

	public override void _Ready()
	{
		hook = GetNode<ChainAttach>("ChainAttach");
		norLine = GetNode<Line2D>("NormalDirection");
		tanLine = GetNode<Line2D>("TangentDirection");
		velLine = GetNode<Line2D>("VelocityDirection");
		joint = GetNode<GrooveJoint2D>("Joint");

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
				GD.Print("hugeChange");

			}
			prevAngle = angle;
		}


		if (gravEmpty)
		{
			GD.Print("gravEmpty");
		}

		Godot.Vector2 prevNormal = normalDir;


	

		if (mainGravity != null)
		{
			(normalDir, tangentDir, angle) = mainGravity.getDirections(GlobalPosition);

		}
		else
		{
			normalDir = Godot.Vector2.Down;
			angle = -(float)Math.PI / 4;
		}

		if (state != 3 && state != 3)
		{
			(normalVelocity, tangentVelocity) = getMagnitudes(velocity, normalDir);
		}


		prevKeyPress = keyPress;
		keyPress = Godot.Vector2.Zero;
		keyPress.X = 0;
		if (Input.IsActionPressed("right"))
		{
			keyPress.X += 1;
		}
		if (Input.IsActionPressed("left"))
		{
			keyPress.X -= 1;
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
					xFlip = -1;
				}
			}

		}

		tangentDir = getPerp(normalDir);


	




		UpDirection = -normalDir;
		RotationDegrees = angle;


		if (chainHit)
		{
			state = 3;
		}


		switch (state)
		{
			case 0:
				//walking

				
				if (keyPress.X != prevKeyPress.X)
				{
					xFlip = 1;
				}


				if (Input.IsActionPressed("jump"))
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




				break;

			case 1:
				//state jump

				if (keyPress.X != prevKeyPress.X)
				{
					xFlip = 1;
				}


				if (IsOnCeiling())
				{
					//set the velocity to be downard and make it so that the jump doesnt rise anymore
					normalVelocity = +200f;
					timeOut = true;
				}

				if (!timeOut)
				{
					if (Input.IsActionPressed("jump"))
					{
						//holding to jump higher
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
					if (Input.IsActionPressed("down"))
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
					state = 0;
					break;
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

				keyPress.X *= xFlip;
				//normalVelocity = 0;
				Godot.Vector2 progressVector = GlobalPosition - chain.start;
				hook.progress = getProjection(progressVector, chain.chainVectorFull);
				GD.Print("progress: ", hook.progress);
				GD.Print("progressVector: ", progressVector);


				if (Input.IsActionPressed("jump"))
				{
					chainDetach();
					(normalVelocity, tangentVelocity) = getMagnitudes(Velocity, normalDir);
					jump();

					state = 1;
					break;
				}

				velocity = keyPress.X * hook.chainVector * topSpeed * (float)delta;



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
			GD.Print("GRAVAAA");
		}





	}

	public void jump()
	{
		normalVelocity = JumpVelocity;
		normalVelocity -= Math.Abs(tangentVelocity) * 0.3f;
		jumpTimer = jumpTime;
		timeOut = false;
	}


	public void _on_gravity_area_entered(Area2D area)
	{
		//get the path to the gravity and 
		NodePath path = GetPathTo(area);
		GravityArea gZone = GetNode<GravityArea>(path);

		if (gravEmpty)
		{
			gravityAreas.Clear();
			gravEmpty = false;

		}

		//add the gravity area to the zones

		prioritizeGravityArea(gZone);

		
		GD.Print("gravCount: ", gravityAreas.Count);


		return;
	}

	public void prioritizeGravityArea(GravityArea gZone)
	{

		//loop through the gravity zones and check their priority levels
		//start at index 0
	
		if (gravityAreas.Count == 0)
		{
			gravityAreas.Add(gZone);
			if (mainGravity != gZone)
			{
				mainGravity = gZone;
				newGravPriority = true;
				
			}
			
			return;

		}


		for (int i = 0; i < gravityAreas.Count; i += 1)
		{
			//if the priority of the new gZone is >= the one in the list
			if (gZone.priority >= gravityAreas[i].priority)
			{
				gravityAreas.Insert(i, gZone);
				if (i == 0)
				{
					mainGravity = gZone;
					newGravPriority = true;
				}
				return;
			}
		}


	}
	public void _on_gravity_area_exited(Area2D area)
	{
		NodePath path = GetPathTo(area);
		GravityArea gZone = GetNode<GravityArea>(path);

		int gIndex = 0;


		if (gravityAreas.Count >= 2)
		{
			gravityAreas.Remove(gZone);
			mainGravity = gravityAreas[0];
		}
		else if (gravityAreas.Count == 1)
		{
			gravEmpty = true;
		}



		GD.Print("gIndex: ", gIndex);
		GD.Print("gravCountExited: ", gravityAreas.Count);



	}

	public void _on_chain_area_entered(Area2D area)
	{
		GD.Print("chainEntered");
		chain = (GetNode<ChainArea>(GetPathTo(area)).GetParent<Chain>());
		chainCheck();
		normalVelocity = 0f;
		tangentVelocity = 0f;
		hook.chainAttach(chain);
		chainAttach();
		GlobalPosition = hook.start + (hook.chainVector * hook.progress);

	}
	public void _on_chain_area_exited(Area2D area)
	{
		chain = null;
	}

	public void chainDetach()
	{
		chain = null;
		chainHit = false;
	}

	public void chainCheck()
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


		angle = (-UpDirection).AngleTo(lineVector);

		float pi = (float)Math.PI;
		float pi2 = 2 * pi;

		if (angle < 0)
		{
			angle += pi;
		}


		if (angle > (5 * pi / 6) && angle < (7 * pi / 6))
		{
			GD.Print("wall");
		}
		else
		{
			GD.Print("floor");
		}





		chainHit = true;
	}



	public void chainAttach()
	{
		progress = getProjection(GlobalPosition - chain.GetPointPosition(0), chain.chainVectorFull);
		//we need to get the "right" direction relative to the rotation of the captain
		float posXAngle = normalDir.Angle();
		//posXAngle *= 180 / (float)Math.PI;
		GD.Print("posXAngle: ", posXAngle);


		//rotate the vector by -90 degrees so that it is perpendicular
		posXAngle -= (float)Math.PI / 2;

		//convert the angle to a vector
		Godot.Vector2 posXvector = new Godot.Vector2((float)Math.Cos(posXAngle), (float)Math.Sin(posXAngle));

		float prog = 0;
		prog = getProjection(posXvector, hook.chainVector);
		if (prog < 0)
		{
			xFlip = -1;
		}
		GD.Print("prog: ", prog);

		tanLine.ClearPoints();
		tanLine.Show();
		tanLine.AddPoint(Godot.Vector2.Zero);
		tanLine.AddPoint(posXvector * 250);

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
