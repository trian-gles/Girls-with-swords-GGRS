using Godot;
using System;
using System.Collections.Generic;


public abstract class AirGrabStart : AirState
{
	private const string FallString = "Fall";
	private const string LandingString = "Landing";
	private const string AirGrabString = "AirGrab";

	public override void _Ready()
    {
        base._Ready();
		owner.canDoubleJump = false;
		owner.canAirDash = false;
		owner.landingRecoveryFramesRemaining += 4;
    }

	public override void AnimationFinished()
	{
		owner.ChangeState(FallString);
	}

	public override void CheckHit()
	{
		Vector2 collisionPnt = owner.CheckHurtRectGrab();
		if (collisionPnt != Vector2.Inf && owner.otherPlayer.IsAirGrabbable())
		{
			owner.ChangeState(AirGrabString);
		}
	}


	public override void ReceiveHit(Globals.AttackDetails details)
	{
		ReceiveHitNoBlock(details);
	}

	public override void FrameAdvance()
    {
        base.FrameAdvance();
		if (owner.grounded && frameCount > 1)
		{
			owner.ChangeState(LandingString);
		}
		ApplyGravity();
	}
}
