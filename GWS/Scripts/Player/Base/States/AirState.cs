using Godot;
using System;
using System.Collections.Generic;


public abstract class AirState : State
{
	public override HashSet<string> tags { get; set; } = new HashSet<string>() { "aerial"};
	public override void _Ready()
    {
        base._Ready();
        stop = false;
    }

    public override bool DelayInputs()
    {
        return owner.internalPos.y > 18000;
    }

	public override void ReceiveHit(Globals.AttackDetails details)
	{
		owner.velocity = new Vector2(0, 0);
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

		bool rightBlock = details.dir == BaseAttack.ATTACKDIR.RIGHT && owner.CheckHeldKey('6');
		bool leftBlock = details.dir == BaseAttack.ATTACKDIR.LEFT && owner.CheckHeldKey('4');
		bool anyBlock = details.dir == BaseAttack.ATTACKDIR.EQUAL && (owner.CheckHeldKey('4') || owner.CheckHeldKey('6'));

		if (details.height == HEIGHT.LOW || details.airBlockable == false)
		{
			EnterHitState(details.knockdown, details.opponentLaunch, details.collisionPnt, details.effect, details.graphicFX);
		}
		else
		{
			if (owner.CheckOverrideBlock())
				EnterBlockState("Block", details.collisionPnt);

			else if (rightBlock || leftBlock || anyBlock)
			{
				EnterBlockState("Block", details.collisionPnt);
			}
			else
			{
				EnterHitState(details.knockdown, details.opponentLaunch, details.collisionPnt, details.effect, details.graphicFX);
			}
		}
	}
}
