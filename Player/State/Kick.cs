using Godot;
using System;
using System.Collections.Generic;

public class Kick : BaseAttack
{
    public override void _Ready()
    {
        base._Ready();
        AddNormalGatling(new char[] { '8', 'p' }, "Jump");
        AddNormalGatling(new char[] { 'k', 'p' }, "Kick");
    }
}

