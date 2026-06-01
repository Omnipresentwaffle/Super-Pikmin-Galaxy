using Godot;
using System;
using System.Collections.Generic;


[Tool]
public partial class NavNode : Node2D
{
	[Export]
	public nodeType type = nodeType.nav;


	
	public enum nodeType
	{
		nav,
		onion,
		ship

	}
	
	
	
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
		//GD.Print("dest: ", Connections[0].destination);

	}

	public void UpdateExclusive()
	{
		var color = GetNode<ColorRect>("ColorRect");
		color.Color = PikRGB.Map[ExclusiveTo];
		GD.Print("Color changed");
 


	}

	public void ConnectTo(NavNode o = null, bool isOneWay = false)
	{
		return;
		/*
		if (! connections.Contains(o))
		{
			 connections.Add(o);

		}
		if (!isOneWay)
		{
			if (!o. connections.Contains(this))
			{
				o. connections.Add(this);

			}
		}
		*/		
		

	}
		


	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		return;
	}
}
