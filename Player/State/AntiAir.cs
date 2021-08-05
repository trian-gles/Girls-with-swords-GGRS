using Godot;
using System;

public class AntiAir : BaseAttack
{

    [Export]
    protected Vector2 launch = new Vector2();

    public override void Enter()
    {
        base.Enter();
        owner.velocity = launch;
        owner.grounded = false;
    }

    public override void FrameAdvance()
    {
        base.FrameAdvance();
        ApplyGravity();

    }

    public override void AnimationFinished()
    {
        EmitSignal(nameof(StateFinished), "Fall");
    }
}
