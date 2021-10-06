using Godot;
using System;

public class AirKnockdown : HitStun
{
    protected override void EnterHitState(bool knockdown, bool launch)
    {
        EmitSignal(nameof(StateFinished), "AirKnockdown");
    }

    public override void FrameAdvance()
    {
        frameCount++;
        if (owner.grounded)
        {
            EmitSignal(nameof(StateFinished), "Knockdown");
            owner.ResetCombo();
        }
        ApplyGravity();
    }
}
