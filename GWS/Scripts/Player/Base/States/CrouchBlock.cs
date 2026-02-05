using Godot;
using System;
using System.Collections.Generic;

public class CrouchBlock : Block
{
    public override HashSet<Globals.Tags> tags { get; set; } = new HashSet<Globals.Tags>() { Globals.Tags.crouching, Globals.Tags.block };
	private const string RecoveryString = "Recovery";
	private const string CrouchString = "Crouch";
	private const string IdleString = "Idle";
	private const string GuardCancelString = "GuardCancel";
	private const string CrouchShieldString = "CrouchShield";

    public override void FrameAdvance()
	{
		frameCount++;
		if (frameCount == 1)
			owner.EmitSignal(RecoveryString, owner.Name);
		if (slowdownSpeed != 0) SlowDown();
		stunRemaining--;
		if (stunRemaining == 0)
		{
			if (owner.CheckHeldKey('2'))
				owner.ChangeState(CrouchString);
			else
				owner.ChangeState(IdleString);

		}

        if (owner.CheckHeldKeys(guardCancelKeys) && owner.CheckFlippableHeldKey('6') && owner.TrySpendMeter())
        {
			owner.ChangeState(GuardCancelString);
        }
    }

    public override void EnterShieldState()
    {
		owner.ChangeState(CrouchShieldString);
    }
}
