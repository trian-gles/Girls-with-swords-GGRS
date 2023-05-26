using Godot;
using System;

public class SuperJump : Jump
{
    public override string animationName { get { return "Jump"; } }

    public override void Enter()
    {
        base.Enter();
        owner.velocity.y = -owner.superJumpForce;
        owner.canDoubleJump = false;
    }
}


