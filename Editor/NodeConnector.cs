using Godot;
using System;
using System.Collections.Generic;

[Tool]
public partial class NodeConnector : Node
{
    public override void _Ready()
    {
        if (!Engine.IsEditorHint())
            return;

        GD.Print("Running in editor");
    }
}