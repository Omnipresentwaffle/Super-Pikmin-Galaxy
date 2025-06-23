using Godot;
using System;


public partial class Pikmin : Entity
{

	public UInt16 state = 0;
	//0 = idle
	//1 = follow
	//2 = thrown
	//3 = carry
	//4 = attack/attached
	//5 = 

	public UInt16 subState = 0;

	[Export] public PikminData pikminData = new PikminData();

	public FollowPath follow = null;

	public UInt16 team = 0;

	public AnimatedSprite2D animation = null;



	public override void _Ready()
	{
		animation = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		AnimatedSprite2D loadAnim = (AnimatedSprite2D)pikminData.animation.Instantiate();
		animation.SpriteFrames = loadAnim.SpriteFrames;

	}

	public override void _PhysicsProcess(double delta)
	{
		Godot.Vector2 velocity = Velocity;
		
		switch (state)
		{
			case 0:


				break;
		}




	}



}
