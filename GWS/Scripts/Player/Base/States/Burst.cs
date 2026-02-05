using Godot;
using System;


public class Burst : LaunchAttack
{
	private const string BurstString = "Burst";
	private const string BurstLowerString = "burst";
	public override void Enter()
	{
		base.Enter();
		owner.ClearHit();
		owner.EmitSignal(nameof(Player.GenericGFX), BurstString, owner.Name);
		owner.landingRecoveryFramesRemaining = 5;
	}

	public override void FrameAdvance()
	{
		base.FrameAdvance();
		if (frameCount == 8)
		{
			Globals.EmitPlayerFXEmitted(owner.internalPos, BurstLowerString, owner.facingRight);
		}
	}

}
