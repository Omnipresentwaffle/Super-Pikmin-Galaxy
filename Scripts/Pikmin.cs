using Godot;
using System;


public partial class Pikmin : Entity
{

	public int state = 0;
	//0 = idle
	//1 = follow
	//2 = thrown
	//3 = carry
	//4 = attack/attached
	//5 = 

	[Export] public PikminData pikminData = new PikminData();



	public override void _Ready()
	{


	}

	public override void _PhysicsProcess(double delta)
	{



	}



}
