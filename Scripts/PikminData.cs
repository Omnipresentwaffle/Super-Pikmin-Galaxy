using Godot;


[GlobalClass]
public partial class PikminData : Resource
{
    #region VARIABLES
    [Export] public PackedScene animation;
    [Export] public int attack = 5;
    [Export] public int strength = 1;

    #endregion
}
