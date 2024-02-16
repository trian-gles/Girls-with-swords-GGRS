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
    public override void FrameAdvance()
    {
        base.FrameAdvance();
        if (frameCount % 5 == 0)
        {
            GetNode<Node>("/root/Globals").EmitSignal(nameof(GhostEmitted), (Player)owner);
        }
    }
}


