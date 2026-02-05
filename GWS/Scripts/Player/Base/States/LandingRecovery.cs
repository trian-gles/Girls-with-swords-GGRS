using Godot;
using System;
using System.Collections.Generic;

public class LandingRecovery : State
{
	private const string IdleString = "Idle";
	private const string CrouchAnimString = "Crouch";
	public override string animationName { get { return CrouchAnimString; } }
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
			owner.ChangeState(IdleString);
	}

	public override void ReceiveHit(Globals.AttackDetails details)
	{
		ReceiveHitNoBlock(details);
	}
}


