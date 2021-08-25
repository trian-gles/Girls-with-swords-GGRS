using Godot;
using System;

public class Knockdown : State
{
    public override void Enter()
    {
        base.Enter();
        owner.ComboUp();
    }
    public override void AnimationFinished()
    {

        owner.ResetCombo();
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
