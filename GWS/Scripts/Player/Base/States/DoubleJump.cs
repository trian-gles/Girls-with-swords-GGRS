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
        owner.hasDoubleOrSuperJumped = true;

        if (owner.CheckHeldKey('6'))
		{
			owner.velocity.x = Mathf.Max(owner.speed, (int)Math.Floor(owner.velocity.x / 2));
		}

		else if (owner.CheckHeldKey('4'))
		{
			owner.velocity.x = Mathf.Min(-owner.speed, (int)Math.Floor(owner.velocity.x / 2));
		}
    }
    public override void FrameAdvance()
    {
        base.FrameAdvance();
        if (frameCount % 5 == 0)
        {
            GetNode<Node>("/root/Globals").EmitSignal(nameof(GhostEmitted), (Player)owner);
        }
    }

    public override bool DelayInputs()
	{
		return frameCount < startupFrames;
	}

    protected override void ApplyGravity()
	{
		owner.velocity.y = Math.Min(owner.velocity.y + Mathf.Floor(owner.gravity / 2), CheckTerminalVelocity());
	}
}


