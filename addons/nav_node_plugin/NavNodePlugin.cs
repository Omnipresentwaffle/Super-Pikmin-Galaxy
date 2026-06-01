using Godot;

[Tool]
public partial class NavNodePlugin : EditorPlugin
{
    private NavNode _firstSelected = null;

    public override void _EnterTree()
    {
        EditorInterface.Singleton.GetSelection().SelectionChanged += OnSelectionChanged;
    }

    public override void _ExitTree()
    {
        EditorInterface.Singleton.GetSelection().SelectionChanged -= OnSelectionChanged;
    }

    private void OnSelectionChanged()
    {
        return;
        var selection = EditorInterface.Singleton.GetSelection().GetSelectedNodes();

        if (selection.Count == 0)
            return;

        if (selection[0] is not NavNode node)
            return;

        if (_firstSelected == null)
        {
            _firstSelected = node;
            GD.Print("Start node: " + node.Name);
        }
        else
        {
            if (_firstSelected != node)
            {
                _firstSelected.ConnectTo(node);
                GD.Print($"Connected {_firstSelected.Name} <-> {node.Name}");

            
            }

            _firstSelected = null;
        }
    }
    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey keyEvent &&
            keyEvent.Pressed &&
            keyEvent.Keycode == Key.S)
        {
            _firstSelected = null;
            GD.Print("Reset Node Selection");
        }
    }
}