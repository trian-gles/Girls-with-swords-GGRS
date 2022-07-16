using Godot;
using System;

/// <summary>
/// Never ending untechable air state
/// </summary>
public class AirKnockdown : Float
{

    public override void _Ready()
    {
        base._Ready();
        animationName = "Float";
    }
    public override void FrameAdvance()
    {
        frameCount++;
        if (owner.grounded)
        {
            EmitSignal(nameof(StateFinished), "Knockdown");
            owner.ResetComboAndProration();
        }
        ApplyGravity();
    }
}
