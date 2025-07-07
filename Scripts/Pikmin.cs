using Godot;
using System;


public partial class Pikmin : Passive
{

	//0 = idle
	//1 = follow
	//2 = thrown
	//3 = carry
	//4 = attack/attached
	//5 = 


	[Export] public PikminData pikminData = new PikminData();

	public AnimatedSprite2D animation = null;

	public Area2D whistleHitbox = null;	
	







	public override void _Ready()
	{
		animation = GetNode<AnimatedSprite2D>("Animation");
		AnimatedSprite2D loadAnim = (AnimatedSprite2D)pikminData.animation.Instantiate();
		animation.SpriteFrames = loadAnim.SpriteFrames;
		whistleHitbox = GetNode<Area2D>("WhistleDetector");
		follower = GetNode<Follower>("Follower");


	}

	public override void _PhysicsProcess(double delta)
	{
		Godot.Vector2 velocity = Velocity;

		if (joinFollow)
		{
			state = 1;
			joinFollow = false;
		}

		switch (state)
		{
			case 0:
				//state idle
			

				//GD.Print("pikpos: ", GlobalPosition);
				//GD.Print("mainGravpik: ", normalVelocity);
				if (mainGravity != null)
				{
					(normalDir, tangentDir, angle) = mainGravity.getDirections(GlobalPosition);


				}
				else
				{
					normalDir = Godot.Vector2.Down;
					tangentDir = getPerp(normalDir);
					angle = -(float)Math.PI / 4;
				}


				
				normalVelocity += mainGravity.gravityStrength * (float)delta;
				GlobalRotationDegrees = angle;

				velocity = normalDir * normalVelocity;
				velocity += tangentDir * tangentVelocity;



				break;

			case 1:
				//state follow

				velocity = follow((float)delta);

				



				break;
		}

		Velocity = velocity;
		MoveAndSlide();


	}



}
