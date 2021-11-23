using Godot;
using System;

public class Run : Walk
{
    public override void _Ready()
    {
        base._Ready();
        soundRate = 10;
    }

    public override void Enter()
    {
        base.Enter();
        if (owner.CheckHeldKey('8'))
        {
            EmitSignal(nameof(StateFinished), "MovingJump");
        }
    }
}
