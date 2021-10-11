using Godot;
using System;
using System.Collections.Generic;

public class CrouchJab : BaseAttack
{
    public override void _Ready()
    {
        base._Ready();
        AddGatling(new List<char[]> { new char[] { '2', 'r' }, new char[] { 'k', 'p' } }, "Kick");
        AddGatling(new char[] { '8', 'p' }, "Jump");
        AddGatling(new char[] { 'p', 'p' }, "CrouchJab");
        AddGatling(new char[] { 'k', 'p' }, "Sweep");
        AddGatling(new char[] { 's', 'p' }, "CrouchSlash");
        
    }
}

