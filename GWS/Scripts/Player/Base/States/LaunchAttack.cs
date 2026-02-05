using Godot;
using Godot.Collections;
using System;
using System.Linq;

public class LaunchAttack : AirAttack
{
	private const string SuperFlashString = "SuperFlash";
	private const string SuperPowerUpString = "SuperPowerUp";
	private const string LandingRecoveryString = "LandingRecovery";
	private const string LandingString = "Landing";
	private const string FallString = "Fall";
	private const string DustString = "dust";

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
			owner.EmitSignal(SuperFlashString, owner.Name);
			owner.GFXEvent(SuperPowerUpString);
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
						owner.ChangeState(LandingRecoveryString);
					else
						owner.ChangeState(LandingString);
				}
			}
		}

		if (emitGhost)
		{
			if (frameCount % 5 == 0)
			{
				Globals.EmitGhostEmitted(owner);
			}
		}

		if (dustFrames.Contains(frameCount))
		{
			dustEmissionVector.x = owner.internalPos.x;
			dustEmissionVector.y = owner.GetCollisionRect().End.y;
			Globals.EmitPlayerFXEmitted(dustEmissionVector, DustString, owner.facingRight);
		}
	}

	public override void AnimationFinished()
	{
		owner.ChangeState(FallString);
	}
}
