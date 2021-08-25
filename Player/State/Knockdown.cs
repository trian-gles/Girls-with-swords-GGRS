using Godot;
using System;

public class Knockdown : State
{
    public override void AnimationFinished()
    {
        if (owner.grounded) 
        {
            EmitSignal(nameof(StateFinished), "Idle");
        }
        else
        {
            EmitSignal(nameof(StateFinished), "Fall");
        }
    }

    public override void ReceiveHit(bool rightAttack, HEIGHT height, Vector2 push, bool knockdown)
    {
        // No OTG
    }
}
