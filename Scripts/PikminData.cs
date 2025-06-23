using System;
using System.Collections.Generic;
using Godot;


[GlobalClass, Tool]
public partial class PikminData : Resource
{
    #region VARIABLES
    [Export] public PackedScene animation;

    [Export] public Type type = Type.red;

    [Export] public string typeName = "";
    [Export] public UInt16 attack = 5;
    [Export] public UInt16 strength = 1;
    [Export] public float throwHeight = 200f;
    [Export] public float throwDistance = 200f;

    [Export] public UInt16 generation = 1;

    [Export] public Godot.Collections.Array immunities = [];

    #endregion

    public enum Type
    {
        red,
        yellow,
        blue,
        purple,
        white,
        rock,
        winged,
        ice,

    }
}
