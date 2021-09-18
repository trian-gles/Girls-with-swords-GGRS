using Godot;
using System;
using System.Collections.Generic;

public class Slash : GroundAttack
{
    public override void _Ready()
    {
        base._Ready();
        AddGatling(new char[] { '8', 'p' }, "Jump");
        AddGatling(new List<char[]> { new char[] { '4', 'p' }, new char[] { '2', 'p' }, new char[] { '4', 'p' }, new char[] { 'p', 'p' } }, "DP");
    }

}
