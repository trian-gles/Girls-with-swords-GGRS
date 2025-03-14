using Godot;
using System;
using System.Collections.Generic;

public class SnailCall : State
{

	/// <summary>
	/// 0 : jumping snail, 1 : double snail, 2: phone call
	/// </summary>
	[Export]
	public int callMode = 0;

	[Export]
	public int snailCommandFrame = 10;

	[Export]
	public int snailRideLastFrame = 10;

	public override string animationName { get { return "SnailCall"; } }

	private void SendSnailAttack()
    {
		var sl = (SL)owner;
		if (!sl.leftCornerSnail || !sl.rightCornerSnail)
			owner.CommandHadouken("Snail", HadoukenPart.ProjectileCommand.SnailAttack);
		else
        {
			if (owner.facingRight)
				owner.CommandHadouken("Snail", HadoukenPart.ProjectileCommand.LeftSnailAttack);
			else
				owner.CommandHadouken("Snail", HadoukenPart.ProjectileCommand.RightSnailAttack);
		}
		EmitSignal(nameof(StateFinished), "PhonePutAway");
	}

	private void SendSnailJump()
	{
		var sl = (SL)owner;
		if (!sl.leftCornerSnail || !sl.rightCornerSnail)
			owner.CommandHadouken("Snail", HadoukenPart.ProjectileCommand.SnailJump);
		else
		{
			if (owner.facingRight)
				owner.CommandHadouken("Snail", HadoukenPart.ProjectileCommand.LeftSnailJump);
			else
				owner.CommandHadouken("Snail", HadoukenPart.ProjectileCommand.RightSnailJump);
		}
		EmitSignal(nameof(StateFinished), "PhonePutAway");
	}

	public override void FrameAdvance()
    {
        base.FrameAdvance();
		if (frameCount == snailCommandFrame)
        {
			if (callMode == 1)
				SendSnailJump();
			else if (callMode == 2)
				EmitSignal(nameof(StateFinished), "PhoneToss");
			else
				SendSnailAttack();
        }

		if (frameCount > 2 && frameCount < snailRideLastFrame)
        {
			owner.CommandHadouken("Snail", HadoukenPart.ProjectileCommand.SnailRide);
		}
	}


	public override void ReceiveHit(Globals.AttackDetails details)
	{
		ReceiveHitNoBlock(details);
	}
}