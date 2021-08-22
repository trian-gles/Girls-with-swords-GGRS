using Godot;
using System;

public class JumpPunch : JumpKick
{
    public override void _Ready()
    {
        base._Ready();
        AddGatling(new char[] { 'k', 'p' }, "JumpKick");
    }
}
