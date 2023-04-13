using Godot;
using System;

public class SuperJump : Jump
{
    public override void _Ready()
    {
        base._Ready();
        animationName = "Jump";
    }

    public override void Enter()
    {
        base.Enter();
        owner.velocity.y = -owner.superJumpForce;
        owner.canDoubleJump = false;
    }
}


