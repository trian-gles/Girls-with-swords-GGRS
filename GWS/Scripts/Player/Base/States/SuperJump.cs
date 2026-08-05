using Godot;
using System;

public class DoubleJump : Jump
{
    public override void Enter()
    {
        base.Enter();
        owner.hasDoubleOrSuperJumped = true;
    }

    public override bool DelayInputs()
	{
		return frameCount < startupFrames;
	}
}


