using Godot;
using Godot.Collections;
using System;
using System.Linq;

public class LaunchAttack : AirAttack
{

	[Export]
	protected Vector2 launch = new Vector2();

	[Export]
	protected int launchFrame = 1;

	[Export]
	protected Array<int> dustFrames = new Array<int>();

	[Export]
	protected bool exitOnLand = false;

	[Export]
	public bool emitGhost = false;

	private Vector2 dustEmissionVector = new Vector2();

	/// <summary>
	/// This doesn't call base.FrameAdvance() because that state includes things we don't want
	/// </summary>
	public override void FrameAdvance()
	{
		frameCount++;
		if (restoreHitFrames != null && restoreHitFrames.Contains(frameCount))
			hitConnect = false;

		if (slowdownSpeed != 0) SlowDown();

		if (frameCount > 0 && frameCount == superFrame)
		{
			owner.EmitSignal("SuperFlash", owner.Name);
			owner.GFXEvent("SuperPowerUp");
		}



		if (restoreHitFrames != null && restoreHitFrames.Contains(frameCount))
			hitConnect = false;


		if (frameCount == launchFrame)
		{
			owner.velocity = launch;
			if (!owner.facingRight)
			{
				owner.velocity.x *= -1;
			}
			owner.grounded = false;
		}
		else if (frameCount > launchFrame)
		{
			ApplyGravity();
			if (owner.grounded)
			{
				owner.velocity.x = 0;
				if (exitOnLand)
				{
					if (owner.landingRecoveryFramesRemaining > 0)
						owner.ChangeState("LandingRecovery");
					else
						owner.ChangeState("Landing");
				}
			}
		}

		if (emitGhost)
		{
			if (frameCount % 5 == 0)
			{
				GetNode<Node>("/root/Globals").EmitSignal(nameof(GhostEmitted), (Player)owner);
			}
		}

		if (dustFrames.Contains(frameCount))
		{
			dustEmissionVector.x = owner.internalPos.x;
			dustEmissionVector.y = owner.GetCollisionRect().End.y;
			GetNode<Node>("/root/Globals").EmitSignal(nameof(PlayerFXEmitted),
			dustEmissionVector,
			"dust", owner.facingRight);
		}
	}

	public override void AnimationFinished()
	{
		owner.ChangeState("Fall");
	}
}
