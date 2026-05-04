using Godot;
using System;
using System.Collections.Generic;


[Tool]
public partial class CarryNode : Node2D
{

	[Export]
	public Godot.Collections.Array<CarryNode> linkedNodes;

	private PikType _exclusiveTo;

	[Export]
	public PikType ExclusiveTo{ 
		get => _exclusiveTo;
		set
		{
			_exclusiveTo = value;
			CallDeferred(nameof(UpdateExclusive));
		}
		}

	
	

	public override void _Ready()
	{
		
	}

	public void UpdateExclusive()
	{
		var color = GetNode<ColorRect>("ColorRect");
		color.Color = PikRGB.Map[ExclusiveTo];
		GD.Print("Color changed");
 


	}
		


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
