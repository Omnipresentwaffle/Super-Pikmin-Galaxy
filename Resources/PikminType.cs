using Godot;
using System;
using System.Collections.Generic;



public enum PikType
{
    none,
    red,
    yellow,
    blue,
    purple,
    white,
    rock,
    winged,
    ice

}

public enum Hazard
{
    none,
    fire,
    electricity,
    water,
    poision,
    crushing,
    ice,
    acid

}
public static class PikRGB
{
    public static readonly Dictionary<PikType, Color> Map = new()
    {
        { PikType.none, Color.Color8(0, 0, 0)},
        { PikType.red,      Color.Color8(255, 0, 0) },
        { PikType.yellow,   Color.Color8(255, 255, 20) },
        { PikType.blue,     Color.Color8(0, 0, 255) },
        { PikType.purple,   Color.Color8(146, 0, 214)},
        { PikType.white,    Color.Color8(255,255,255)},
        { PikType.rock,     Color.Color8(62, 62, 69)},
        { PikType.winged,   Color.Color8(255, 0, 237)},
        { PikType.ice,      Color.Color8(0,255,255)}

    };
}