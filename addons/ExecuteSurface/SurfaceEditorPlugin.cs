using Godot;
[Tool]
public partial class SurfaceEditorPlugin : EditorPlugin {
	private ExecuteSurface inspector;
	public override void _EnterTree() {
		inspector = new ExecuteSurface();
		GD.Print("Plugin loaded!");
		AddInspectorPlugin(inspector);
	}
	public override void _ExitTree() {
		RemoveInspectorPlugin(inspector);
	}
}
