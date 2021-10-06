using Godot;
using System;

public class AirKnockdown : HitStun
{
    protected override void EnterHitState(bool knockdown, Vector2 launch)
    {
        if (!(launch == Vector2.Zero))
        {
            owner.velocity = launch;
        }

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
