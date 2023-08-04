using Godot;
using System;
using System.Collections.Generic;

public class SnailCall : State
{


	public override void _Ready()
	{
		base._Ready();
		loop = true;
		stop = false;
		AddGatling(new[] { 's', 'r' }, "PhonePutAway");
		AddGatling(new List<char[]>() { new char[] { '2', 'p' }, new char[] { '4', 'r' }, new char[] { '6', 'p' }, new char[] { '2', 'r' }, new[] { 'p', 'p' } }, "PhoneToss");


		AddGatling(new char[] { 'p', 'p' }, () => { return owner.CheckHeldKey('2'); }, "", () => {
			owner.CommandHadouken("Snail", HadoukenPart.ProjectileCommand.SnailAttack);
		});
		AddGatling(new char[] { '2', 'p' }, () => { return owner.CheckHeldKey('p'); }, "", () => {
			owner.CommandHadouken("Snail", HadoukenPart.ProjectileCommand.SnailAttack);
		});

		AddGatling(new char[] {'k', 'p'}, () => { return owner.CheckHeldKey('2'); }, "", () => {
			owner.CommandHadouken("Snail", HadoukenPart.ProjectileCommand.SnailJump);
		});
		AddGatling(new char[] { '2', 'p' }, () => { return owner.CheckHeldKey('k'); }, "", () => {
			owner.CommandHadouken("Snail", HadoukenPart.ProjectileCommand.SnailJump);
		});

		AddGatling(new char[] { '6', 'p' }, "", () => {
			owner.velocity.x = owner.speed;
		});
		AddGatling(new char[] { '4', 'p' }, "", () => {
			owner.velocity.x = -owner.speed;
		});
		AddGatling(new char[] { '6', 'r' }, "", () => {
			owner.velocity.x = 0;
		});
		AddGatling(new char[] { '4', 'r' }, "", () => {
			owner.velocity.x = 0;
		});
	}

    public override void FrameAdvance()
    {
        base.FrameAdvance();
		if (owner.CheckHeldKey('8'))
        {
			owner.CommandHadouken("Snail", HadoukenPart.ProjectileCommand.SnailRide);
		}
    }

    public override void Enter()
    {
        base.Enter();
		
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