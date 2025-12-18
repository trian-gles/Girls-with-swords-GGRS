using Godot;
using System;


public class Burst : LaunchAttack
{
	public override void Enter()
	{
		base.Enter();
		owner.ClearHit();
		owner.EmitSignal(nameof(Player.GenericGFX), "Burst", owner.Name);
		owner.landingRecoveryFramesRemaining = 5;
	}

	public override void FrameAdvance()
	{
		base.FrameAdvance();
		if (frameCount == 8)
		{
			GetNode<Node>("/root/Globals").EmitSignal(nameof(PlayerFXEmitted),
			new Vector2(owner.internalPos.x, owner.internalPos.y),
			"burst", owner.facingRight);
		}
	}

}
