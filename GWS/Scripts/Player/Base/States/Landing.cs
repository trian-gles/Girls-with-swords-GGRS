using Godot;
using System;
using System.Collections.Generic;

public class Landing : State
{
	private const string IdleString = "Idle";
	[Export]
	public int len = 3;
	private const string CrouchAnimString = "Crouch";
	public override string animationName { get { return CrouchAnimString; } }
	public override void _Ready()
	{
		base._Ready();
		stop = false;
		AddSpecials(owner.groundSpecials);
		AddCommandNormals(owner.commandNormals);
		AddEasyGroundSpecials();
		AddNormals();
	}
	//public override bool DelayInputs()
	//{
	//	return true;
	//}
	public override void Enter()
	{
		base.Enter();
		if (Globals.mode == Globals.Mode.TRAINING && owner.otherPlayer.currentState.stunRemaining > 0)
			owner.DisplayPlusFrames(owner.otherPlayer.currentState.stunRemaining);
		owner.canDoubleJump = true;
		owner.canAirDash = true;
		owner.hasDoubleOrSuperJumped = false;
	}
	public override void FrameAdvance()
	{
		base.FrameAdvance();
		if (frameCount == len)
			owner.ChangeState(IdleString);
	}

	//public override void ReceiveHit(Globals.AttackDetails details)
	//{
	//	ReceiveHitNoBlock(details);
	//}

	public override void Exit()
	{
		base.Exit();
		owner.velocity.x = 0;
	}
}


