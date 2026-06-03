using Godot;
using System;
using System.Collections.Generic;

public partial class NavPath : Line2D
{
	/*
	This is a type that stores the path back to a destination
	It can be used to simplify movement such that 
	we do not need to access every nodeConnection along the way.
	Additionally it has methods
	NavPaths do not have branching paths, they are a single path
	*/
	float distance = 0;
	
	List<NavNode> navNodes = null;
	public override void _Ready()
	{
	}

	public float getDistance()
	{
		float len = 0.0f;
		for(int i = 0; i < GetPointCount(); i++)
		{
			len += (GetPointPosition(i+1) - GetPointPosition(i)).Length(); 
		}

		return len;
	}



}
