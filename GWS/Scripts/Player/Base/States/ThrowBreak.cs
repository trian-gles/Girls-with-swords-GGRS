using Godot;
using System;
using System.Collections.Generic;

public class ThrowBreak : HitStun
{
    private const string HitStunAnim = "HitStun";
    private const string ThrowBreakGfx = "ThrowBreak";
    private const string FloatState = "Float";
    private const string FallState = "Fall";
    public override string animationName { get { return HitStunAnim; } }

    public override void Enter()
    {
        base.Enter();
        owner.CorrectGrounded();
        owner.GFXEvent(ThrowBreakGfx);
        stunRemaining = 30;
        if (owner.facingRight)
            owner.hitPushRemaining = -2000;
        else
            owner.hitPushRemaining = 2000;
        if (!owner.grounded)
        {
            stunRemaining = 0;
            owner.ChangeState(FloatState);
        }
    }

    public override void ExitHitstun()
    {
        owner.ChangeState(FallState);
    }
}
