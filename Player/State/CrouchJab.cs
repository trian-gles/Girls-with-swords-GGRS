using Godot;
using System;
using System.Collections.Generic;

public class CrouchJab : BaseAttack
{
    public override void _Ready()
    {
        base._Ready();
        AddGatling(new char[] { '8', 'p' }, "Jump");
        AddGatling(new char[] { 'p', 'p' }, "CrouchJab");
        AddGatling(new char[] { 's', 'p' }, "CrouchSlash");
        AddGatling(new List<char[]> { new char[] { '2', 'r' }, new char[] { 'k', 'p' } }, "Kick");
    }
}

