using Godot;
using System;
using System.Collections.Generic;

public class CrouchJab : BaseAttack
{
    public override void _Ready()
    {
        base._Ready();
        AddNormalGatling(new char[] { '8', 'p' }, "Jump");
        AddNormalGatling(new char[] { 'p', 'p' }, "CrouchJab");
        AddNormalGatling(new char[] { 's', 'p' }, "CrouchSlash");
        AddCommandGatling(new List<char[]> { new char[] { '2', 'r' }, new char[] { 'k', 'p' } }, "Kick");
    }
}

