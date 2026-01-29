using Godot;
using System;
using System.Collections.Generic;

public class CrouchBlock : Block
{
    public override HashSet<Globals.Tags> tags { get; set; } = new HashSet<Globals.Tags>() { Globals.Tags.crouching, Globals.Tags.block };
	private string recoveryString = "Recovery";
	private string crouchString = "Crouch";
	private string idleString = "Idle";
	private string guardCancelString = "GuardCancel";
	private string crouchShieldString = "CrouchShield";

    public override void FrameAdvance()
	{
		frameCount++;
		if (frameCount == 1)
			owner.EmitSignal(recoveryString, owner.Name);
		if (slowdownSpeed != 0) SlowDown();
		stunRemaining--;
		if (stunRemaining == 0)
		{
			if (owner.CheckHeldKey('2'))
				owner.ChangeState(crouchString);
			else
				owner.ChangeState(idleString);

		}

        if (owner.CheckHeldKeys(guardCancelKeys) && owner.CheckFlippableHeldKey('6') && owner.TrySpendMeter())
        {
            owner.ChangeState(guardCancelString);
        }
    }

    public override void EnterShieldState()
    {
        owner.ChangeState(crouchShieldString);
    }
}
