using Godot;
using System;
using System.Collections.Generic;

public class Kick : BaseAttack
{
    public override void _Ready()
    {
        base._Ready();
        AddGatling(new char[] { '8', 'p' }, "Jump");
        AddGatling(new char[] { 'k', 'p' }, "Kick");
    }
}

