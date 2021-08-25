using Godot;
using System;

public class Jump : State
{
    public override void Enter()
    {
        base.Enter();
        owner.velocity.y = -1 * owner.jumpForce;
        owner.grounded = false;
    }

    public override void FrameAdvance()
    {
        base.FrameAdvance();
        if (owner.grounded && frameCount > 0) 
        {
            EmitSignal(nameof(StateFinished), "Idle");
        }
        ApplyGravity();
        
    }

    public override void PushMovement(float _xVel)
    {
    }

    public override void HandleInput(char[] inputArr)
    {
        if (Globals.CheckKeyPress(inputArr, 'p'))
        {
            EmitSignal(nameof(StateFinished), "JumpPunch");
        }
        else if (Globals.CheckKeyPress(inputArr, 'k'))
        {
            EmitSignal(nameof(StateFinished), "JumpKick");
        }
        else if (Globals.CheckKeyPress(inputArr, 's'))
        {
            EmitSignal(nameof(StateFinished), "JumpSlash");
        }
    }

    public override void AnimationFinished()
    {
        EmitSignal(nameof(StateFinished), "Fall");
    }
}


