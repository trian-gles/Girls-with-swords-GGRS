using Godot;
using System;
using System.Collections.Generic;

public class SnailCall : State
{


	public override void _Ready()
	{
		base._Ready();
		loop = true;
		AddGatling(new[] { 's', 'r' }, "PhonePutAway");
		AddGatling(new List<char[]>() { new char[] { '2', 'p' }, new char[] { '4', 'r' }, new char[] { '6', 'p' }, new char[] { '2', 'r' }, new[] { 'p', 'p' } }, "PhoneToss");
	}

	public override void ReceiveHit(Globals.AttackDetails details)
	{
		switch (details.dir)
		{
			case BaseAttack.ATTACKDIR.RIGHT:
				break;
			case BaseAttack.ATTACKDIR.LEFT:
				details.opponentLaunch.x *= -1;
				details.hitPush *= -1;
				break;
			case BaseAttack.ATTACKDIR.EQUAL:
				details.opponentLaunch.x = 0;
				details.hitPush = 0;
				break;
		}

		owner.hitPushRemaining = details.hitPush;

		if (owner.velocity.y < 0)
		{
			owner.grounded = false;
		}

		EnterHitState(details.knockdown, details.opponentLaunch, details.collisionPnt, details.effect);
	}
}