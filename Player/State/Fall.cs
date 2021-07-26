using Godot;
using System;

public class Fall : State
{
    public override void _Ready()
    {
        base._Ready();
        loop = true;
    }
    public override void FrameAdvance()
    {
        base.FrameAdvance();
        if (owner.grounded && frameCount > 0)
        {
            EmitSignal(nameof(StateFinished), "Idle");
        }
        owner.CheckTurnAround();
        owner.velocity.y += owner.gravity;
    }

    public override void PushMovement(float _xVel)
    {
    }

    public override void HandleInput(char[] inputArr)
    {
        if (Globals.CheckKeyPress(inputArr, 'k'))
        {
            EmitSignal(nameof(StateFinished), "JumpKick");
        }
    }
}
