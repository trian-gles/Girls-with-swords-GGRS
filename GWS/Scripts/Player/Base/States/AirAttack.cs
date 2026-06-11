using Godot;
using System;
using System.Collections.Generic;
using System.Linq;


public abstract class AirAttack : BaseAttack
{

	private const string LandingString = "Landing";
	private const string LandingRecoveryString = "LandingRecovery";
	private const string FallString = "Fall";
	private const string DoubleJumpString = "DoubleJump";

	[Export]
	public int landingRecoveryFrames = 0;
	public override void _Ready()
	{
		base._Ready();
		tags.Add(Globals.Tags.aerial);
		slowdownSpeed = 0;
		AddCancel(FallString);
		hitDetails.airBlockable = true;
	}

	public override void Enter()
	{
		base.Enter();
		if (landingRecoveryFrames > 0)
			owner.landingRecoveryFramesRemaining = landingRecoveryFrames;
	}

	protected bool CheckDoubleJumpConditions()
    {
        return owner.canDoubleJump && owner.internalPos.y < Globals.MAXDOUBLEJUMPDEPTH;
    }
	protected override void AddJumpCancel()
	{
		AddGatling(new char[] { '8', 'p' }, () => owner.CheckHeldKey('6') && CheckDoubleJumpConditions(), DoubleJumpString, () =>
		{
			owner.velocity.x = owner.speed;
			owner.canDoubleJump = false;
			owner.canAirDash = false;
			owner.hasDoubleOrSuperJumped = true;
		});
		AddGatling(new char[] { '8', 'p' }, () => owner.CheckHeldKey('4') && CheckDoubleJumpConditions(), DoubleJumpString, () =>
		{
			owner.velocity.x = -owner.speed;
			owner.canDoubleJump = false;
			owner.canAirDash = false;
			owner.hasDoubleOrSuperJumped = true;
		});
		AddGatling(new char[] { '8', 'p' }, () => CheckDoubleJumpConditions(), DoubleJumpString, () =>
		{
			owner.velocity.x = 0;
			owner.canDoubleJump = false;
			owner.canAirDash = false;
			owner.hasDoubleOrSuperJumped = true;
		});
	}

	public override void FrameAdvance()
	{
		base.FrameAdvance();
		if (owner.grounded && frameCount > 1)
		{
			if (owner.landingRecoveryFramesRemaining > 0)
				owner.ChangeState(LandingRecoveryString);
			else
				owner.ChangeState(LandingString);
		}

		if (restoreHitFrames != null && restoreHitFrames.Contains(frameCount))
		{
			hitConnect = false;
		}


		

		if (owner.airDashFrames > 0)
			owner.airDashFrames--;
		else
			ApplyGravity();
	}

}
