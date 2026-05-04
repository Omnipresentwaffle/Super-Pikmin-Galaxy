using Godot;
using Godot.NativeInterop;
using System;
//this is intended for complex shapes made with SS2D
[Tool]
public partial class ExecuteSurface : EditorInspectorPlugin
{
	public override bool _CanHandle(GodotObject obj)
	{
		return obj is Platform; // match your custom node type
	}
	public override void _ParseBegin(GodotObject obj)
	{
		if (obj is Platform myNode)
		{
			var button = new Button();
			button.Text = "Do Action";
			button.Pressed += () => myNode.pasteSurface();
			AddCustomControl(button);
		}
	}
	


}
