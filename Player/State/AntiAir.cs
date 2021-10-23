using Godot;
using System;

public class AntiAir : BaseAttack
{

    [Export]
    protected Vector2 launch = new Vector2();

    public override void _Ready()
    {
        base._Ready();
        AddGatling(new char[] { 'p', 'p' }, "JumpPunch");
        AddGatling(new char[] { 'k', 'p' }, "JumpKick");
    }
    public override void Enter()
    {
        base.Enter();
        owner.velocity = launch;
        if (!owner.facingRight)
        {
            GD.Print("Flipping launch x coor");
            owner.velocity.x *= -1;
        }
        owner.grounded = false;
        if (owner.grounded)
        {
            EmitSignal(nameof(StateFinished), "Idle");
        }
    }

    public override void FrameAdvance()
    {
        base.FrameAdvance();
        ApplyGravity();
        if (owner.grounded)
        {
            EmitSignal(nameof(StateFinished), "Idle");
        }
    }

    public override void AnimationFinished()
    {
        EmitSignal(nameof(StateFinished), "Fall");
    }
}
