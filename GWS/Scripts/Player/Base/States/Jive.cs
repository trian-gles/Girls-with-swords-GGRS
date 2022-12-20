using Godot;
using System;

public class Jive : Stagger
{

    public override void Enter()
    {
        base.Enter();
        owner.CorrectGrounded();
        owner.velocity = new Vector2(0, owner.velocity.y);
        owner.rhythmState = "";
    }
    public override void FrameAdvance()
    {
        base.FrameAdvance();
        if (!owner.grounded)
            ApplyGravity();
    }

    public override void AnimationFinished()
    {
        if (!owner.grounded)
            EmitSignal(nameof(StateFinished), "Fall");
        else
            EmitSignal(nameof(StateFinished), "Idle");
    }
}



