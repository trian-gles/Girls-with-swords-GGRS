using Godot;
using System;

public class JumpKick : BaseAttack
{

    
    public override void FrameAdvance()
    {
        base.FrameAdvance();
        if (owner.grounded && frameCount > 1)
        {
            EmitSignal(nameof(StateFinished), "Idle");
        }

        ApplyGravity();
    }

    

    public override void AnimationFinished()
    {
        EmitSignal(nameof(StateFinished), "Fall");
    }
}

