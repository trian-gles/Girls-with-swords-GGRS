using Godot;
using System;
using System.Collections.Generic;

public class ThrowBreak : HitStun
{
    public override string animationName { get { return "HitStun"; } }

    public override void Enter()
    {
        base.Enter();
        owner.GFXEvent("ThrowBreak");
        stunRemaining = 30;
        if (owner.facingRight)
            owner.hitPushRemaining = -2000;
        else
            owner.hitPushRemaining = 2000;
        if (!owner.grounded)
        {
            stunRemaining = 0;
            EmitSignal(nameof(StateFinished), "Fall");
        }
    }
}
