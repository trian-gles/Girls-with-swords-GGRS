using Godot;
using System;

public class Run : Walk
{
    public override void _Ready()
    {
        base._Ready();
        soundRate = 10;
    }
}
