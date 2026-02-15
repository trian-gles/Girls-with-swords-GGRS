using Godot;
using System;

public class Jive : Stagger
{
    private const string FallString = "Fall";
    private const string IdleString = "Idle";

    public override void Enter()
    {
        base.Enter();
        owner.CorrectGrounded();
        owner.velocity = new Vector2(0, owner.velocity.y);
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
            owner.ChangeState(FallString);
        else
            owner.ChangeState(IdleString);
    }
}



