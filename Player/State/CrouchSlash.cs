using Godot;
using System;

public class CrouchSlash : BaseAttack
{
    public override void _Ready()
    {
        base._Ready();
        AddGatling(new char[] { '8', 'p' }, "Jump");
        AddGatling(new char[] { 'k', 'p' }, "Sweep");
    }
}
