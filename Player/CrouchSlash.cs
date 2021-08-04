using Godot;
using System;

public class CrouchSlash : BaseAttack
{
    public override void _Ready()
    {
        base._Ready();
        AddNormalGatling(new char[] { '8', 'p' }, "Jump");
    }
}
