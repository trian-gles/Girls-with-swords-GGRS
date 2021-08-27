using Godot;
using System;

public class Slash : GroundAttack
{
    public override void _Ready()
    {
        base._Ready();
        AddGatling(new char[] { '8', 'p' }, "Jump");
    }

}
