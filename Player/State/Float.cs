using Godot;
using System;

public class Float : HitStun
{

    private bool setKnockdown;

    public override void FrameAdvance()
    {
        frameCount++;
        if (owner.grounded)
        {
            EmitSignal(nameof(StateFinished), "Knockdown");
            owner.ResetCombo();
        }

        if (!setKnockdown)
        {
            stunRemaining--;

            if (stunRemaining == 0)
            {
                owner.ResetCombo();
                EmitSignal(nameof(StateFinished), "Fall");
            }
        }

        ApplyGravity();
    }
}
