using Godot;
using System;
using System.Collections.Generic;

public class Slash : GroundAttack
{
    public override void _Ready()
    {
        base._Ready();
        AddSpecials(owner.groundSpecials);
    }
}
