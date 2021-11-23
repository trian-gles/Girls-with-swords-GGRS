using Godot;
using System;

public class Jump : State
{
    public override void Enter()
    {
        base.Enter();
        owner.velocity.y = -1 * owner.jumpForce;
        owner.grounded = false;
        owner.ScheduleEvent(EventScheduler.EventType.AUDIO, "Jump", Name);

        AddGatling(new[] { 'p', 'p' }, "JumpPunch");
        AddGatling(new[] { 'k', 'p' }, "JumpKick");
        AddGatling(new[] { 's', 'p' }, "JumpSlash");
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

    public override void AnimationFinished()
    {
        EmitSignal(nameof(StateFinished), "Fall");
    }
}


