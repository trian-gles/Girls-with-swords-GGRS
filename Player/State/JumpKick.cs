using Godot;
using System;

public class JumpKick : Kick
{

    public override void Enter()
    {
        hitConnect = false;
    }
    public override void FrameAdvance()
    {
        if (owner.grounded && frameCount > 0)
        {
            EmitSignal(nameof(StateFinished), "Idle");
        }
        frameCount += 1;
        owner.velocity.y += owner.gravity;
    }

    public override void HandleInput(char[] inputArr)
    {

        if (Globals.CheckKeyPress(inputArr, 'k') && hitConnect)
        { 
        EmitSignal(nameof(StateFinished), "JumpKick");
        }
    }

    public override void AnimationFinished()
    {
        EmitSignal(nameof(StateFinished), "Fall");
    }
}

