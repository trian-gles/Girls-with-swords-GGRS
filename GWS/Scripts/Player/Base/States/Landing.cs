using Godot;
using System;
using System.Collections.Generic;

public class Landing : State
{
	[Export]
	public int len = 3;

	public override string animationName { get { return "Crouch"; } }
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
		owner.canDoubleJump = true;
		owner.canAirDash = true;
		owner.hasDoubleOrSuperJumped = false;
	}
	public override void FrameAdvance()
	{
		base.FrameAdvance();
		if (frameCount == len)
			owner.ChangeState("Idle");
	}

	public override void ReceiveHit(Globals.AttackDetails details)
	{
		ReceiveHitNoBlock(details);
	}

	public override void Exit()
	{
		base.Exit();
		owner.velocity.x = 0;
	}
}


