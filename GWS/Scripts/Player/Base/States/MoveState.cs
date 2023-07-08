using Godot;
using System;
using System.Collections.Generic;


public abstract class MoveState : State
{

    public override HashSet<string> tags { get; set; } = new HashSet<string>() {"groundmovement" };
    public override void _Ready()
    {
        base._Ready();
        stop = false;
    }
}
