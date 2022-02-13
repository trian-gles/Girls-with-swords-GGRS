using Godot;
using System;
using System.Collections.Generic;


public abstract class AirState : State
{
    public override void _Ready()
    {
        base._Ready();
        stop = false;
    }
}
