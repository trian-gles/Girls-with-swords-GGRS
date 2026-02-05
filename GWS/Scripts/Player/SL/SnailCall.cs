using Godot;
using System;
using System.Collections.Generic;

public class SnailCall : State
{

	private const string PhonePutAwayString = "PhonePutAway";
	private const string PhoneTossString = "PhoneToss";
    private const string SnailString = "Snail";
	private const string SnailCallAnimString = "SnailCall";

	/// <summary>
	/// 0 : jumping snail, 1 : double snail, 2: phone call
	/// </summary>
	[Export]
	public int callMode = 0;

	[Export]
	public int snailCommandFrame = 10;

	[Export]
	public int snailRideLastFrame = 10;

	public override string animationName { get { return SnailCallAnimString; } }

	public override void _Ready()
	{
		base._Ready();
		AddKara(new char[] { 's', 'p' }, () => owner.CheckFlippableHeldKey('6') && owner.grounded && owner.TrySpendMeter(), owner.easySuper);
	}

	private void SendSnailAttack()
	{
		var sl = (SL)owner;
		if (!sl.leftCornerSnail || !sl.rightCornerSnail)
			owner.CommandHadouken(SnailString, HadoukenPart.ProjectileCommand.SnailAttack);
		else
		{
			if (owner.facingRight && sl.leftCornerSnailArrived || !sl.rightCornerSnailArrived)
				owner.CommandHadouken(SnailString, HadoukenPart.ProjectileCommand.LeftSnailAttack);
			else
				owner.CommandHadouken(SnailString, HadoukenPart.ProjectileCommand.RightSnailAttack);
		}
		owner.ChangeState(PhonePutAwayString);
	}

	private void SendSnailJump()
	{
		var sl = (SL)owner;
		if (!sl.leftCornerSnail || !sl.rightCornerSnail)
			owner.CommandHadouken(SnailString, HadoukenPart.ProjectileCommand.SnailJump);
		else
		{
			if (owner.facingRight)
				owner.CommandHadouken(SnailString, HadoukenPart.ProjectileCommand.LeftSnailJump);
			else
				owner.CommandHadouken(SnailString, HadoukenPart.ProjectileCommand.RightSnailJump);
		}
		owner.ChangeState(PhonePutAwayString);
	}

	public override void FrameAdvance()
	{
		base.FrameAdvance();
		if (frameCount == snailCommandFrame)
		{
			if (callMode == 1)
				SendSnailJump();
			else if (callMode == 2)
				owner.ChangeState(PhoneTossString);
			else
				SendSnailAttack();
		}

		if (frameCount > 2 && frameCount < snailRideLastFrame)
		{
			owner.CommandHadouken(SnailString, HadoukenPart.ProjectileCommand.SnailRide);
		}
	}


	public override void ReceiveHit(Globals.AttackDetails details)
	{
		ReceiveHitNoBlock(details);
	}
}
