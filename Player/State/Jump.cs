using Godot;
using System;

public class Jump : State
{
    public override void Enter()
    {

        owner.velocity.y = -1 * owner.jumpForce;
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

    public override void PushMovement(float _xVel)
    {
    }
}


