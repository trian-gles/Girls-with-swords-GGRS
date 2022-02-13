using Godot;
using System;

/// <summary>
/// Never ending untechable air state
/// </summary>
public class AirKnockdown : HitStun
{
    public override void _Ready()
    {
        base._Ready();
        stop = false;
    }
    protected override void EnterHitState(bool knockdown, Vector2 launch, Vector2 collisionPnt)
    {
        GetNode<Node>("/root/Globals").EmitSignal(nameof(PlayerFXEmitted), collisionPnt, "hit");
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
            owner.ResetComboAndProration();
        }
        ApplyGravity();
    }
}
