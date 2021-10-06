using Godot;
using System;

public class Float : HitStun
{
    /// <summary>
    /// I have to override this because float always goes into float!
    /// </summary>
    /// <param name="knockdown"></param>
    /// <param name="launch"></param>
    protected override void EnterHitState(bool knockdown, Vector2 launch)
    {
        if (!(launch == Vector2.Zero))
        {
            owner.velocity = launch;
        }

        EmitSignal(nameof(StateFinished), "Float");
    }

    public override void FrameAdvance()
    {
        frameCount++;
        if (owner.grounded)
        {
            EmitSignal(nameof(StateFinished), "Knockdown");
            owner.ResetCombo();
        }

        stunRemaining--;

        if (stunRemaining == 0)
        {
            owner.ResetCombo();
            EmitSignal(nameof(StateFinished), "Fall");
        }
        

        ApplyGravity();
    }
}
