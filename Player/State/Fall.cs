using Godot;
using System;

public class Fall : Jump
{
    public override void _Ready()
    {
        base._Ready();
        loop = true;
    }
    public override void Enter()
    {
    }
}
