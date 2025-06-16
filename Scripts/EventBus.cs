using Godot;
using System;
using System.Collections.Generic;
using System.Numerics;

public partial class EventBus : Node
{
	[Signal] public delegate void SwapCaptainEventHandler();
	
}
