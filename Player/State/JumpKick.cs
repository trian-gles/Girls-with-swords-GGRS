using Godot;
using System;

public class JumpKick : JumpSlash
{
    public override void _Ready()
    {
        base._Ready();
        AddGatling(new char[] { 's', 'p' }, "JumpSlash");
    }
}

