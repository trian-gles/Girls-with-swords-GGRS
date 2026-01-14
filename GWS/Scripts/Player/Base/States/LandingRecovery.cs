using Godot;
using System;
using System.Collections.Generic;

public class LandingRecovery : State
{

	public override string animationName { get { return "Crouch"; } }
	public override void _Ready()
	{
		base._Ready();
		stop = false;
	}
	//public override bool DelayInputs()
	//{
	//	return true;
	//}
	public override void Enter()
	{
		base.Enter();
		owner.canDoubleJump = true;
		owner.hasDoubleOrSuperJumped = false;
		owner.canAirDash = true;
		owner.velocity.x = 0;
	}
	public override void FrameAdvance()
	{
		base.FrameAdvance();
		if (owner.landingRecoveryFramesRemaining-- == 0)
			owner.ChangeState("Idle");
	}

	public override void ReceiveHit(Globals.AttackDetails details)
	{
		ReceiveHitNoBlock(details);
	}
}


