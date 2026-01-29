using Godot;
using System;
using System.Collections.Generic;

public class Fall : AirState
{
	private string landingString = "Landing";
	private string landingRecoveryString = "LandingRecovery";
	public override void _Ready()
	{
		base._Ready();
		loop = true;
		tags.Add(Globals.Tags.aerial);

		AddSpecials(owner.airSpecials);
		AddExSpecials(owner.airExSpecials);
		AddAirCommandNormals(owner.airCommandNormals);
		AddEasyAirSpecials();

		AddGatling(new[] { 'p', 'p' }, "JumpA");
		AddGatling(new[] { 'k', 'p' }, "JumpB");
		AddGatling(new[] { 's', 'p' }, "JumpC");

		AddAirdash();

		// DOUBLEJUMP
		AddGatling(new char[] { '8', 'p' }, () => owner.CheckHeldKey('6') && owner.canDoubleJump, "DoubleJump", () =>
		{
			owner.CheckTurnAround();
			owner.velocity.x = Math.Max(owner.speed, owner.velocity.x);
			owner.canDoubleJump = false;
			owner.canAirDash = false;
			owner.hasDoubleOrSuperJumped = true;
        });
		AddGatling(new char[] { '8', 'p' }, () => owner.CheckHeldKey('4') && owner.canDoubleJump, "DoubleJump", () =>
		{
			owner.CheckTurnAround();
			owner.velocity.x = Mathf.Min(-owner.speed, owner.velocity.x);
			owner.canDoubleJump = false;
			owner.canAirDash = false;
			owner.hasDoubleOrSuperJumped = true;
        });
		AddGatling(new char[] { '8', 'p' }, () => owner.canDoubleJump, "DoubleJump", () =>
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
		if (owner.grounded && frameCount > 0)
		{
			owner.ForceEvent(EventScheduler.EventType.AUDIO, landingString);

			if (owner.landingRecoveryFramesRemaining > 0)
				owner.ChangeState(landingRecoveryString);
			else
				owner.ChangeState(landingString);
		}
		if (!owner.canDoubleJump)
		{
			owner.CheckTurnAround();
		}

		ApplyGravity();
	}

	public override void PushMovement(float _xVel)
	{
	}

}
