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
            owner.ForceEvent(EventScheduler.EventType.AUDIO, "Landing");
            EmitSignal(nameof(StateFinished), "Idle");
        }
        //owner.CheckTurnAround();
        ApplyGravity();
    }

    public override void PushMovement(float _xVel)
    {
    }

    public override void HandleInput(char[] inputArr)
    {
        if (Globals.CheckKeyPress(inputArr, 'k'))
        {
            EmitSignal(nameof(StateFinished), "JumpB");
        }
        else if (Globals.CheckKeyPress(inputArr, 'p'))
        {
            EmitSignal(nameof(StateFinished), "JumpA");
        }
        else if (Globals.CheckKeyPress(inputArr, 's'))
        {
            EmitSignal(nameof(StateFinished), "JumpC");
        }
    }
}
