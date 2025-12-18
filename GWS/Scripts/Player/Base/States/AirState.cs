using Godot;
using System;
using System.Collections.Generic;


public abstract class AirState : State
{
	[Export]
	public int preAirdashFrames = 0;

	public override HashSet<string> tags { get; set; } = new HashSet<string>() { "aerial"};
	public override void _Ready()
	{
		base._Ready();
		stop = false;
	}

    //public override bool DelayInputs()
    //{
    //    return owner.internalPos.y > 18000;
    //}
	protected bool CheckAirDashConditions()
    {
        return owner.canAirDash && owner.internalPos.y < Globals.MAXAIRDASHDEPTH && frameCount >= preAirdashFrames;
    }
	protected void AddAirdash()
    {
        // AIRDASH
		AddGatling(new List<char[]>() { new char[] { '6', 'p' }, new char[] { '6', 'p' } }, () => owner.facingRight && CheckAirDashConditions(), "AirDash", () =>
		{
			owner.velocity.x = owner.airDashSpeed;
			owner.canDoubleJump = false;
			owner.canAirDash = false;
		}, false, false);


		AddGatling(new List<char[]>() { new char[] { '4', 'p' }, new char[] { '4', 'p' } }, () => !owner.facingRight && CheckAirDashConditions(), "AirDash", () =>
		{
			owner.velocity.x = owner.airDashSpeed * -1;
			owner.canDoubleJump = false;
			owner.canAirDash = false;
		}, false, false);

		AddGatling(new List<char[]>() { new char[] { '6', 'p' }, new char[] { '6', 'p' } }, () => !owner.facingRight && CheckAirDashConditions(), "AirBackdash", () =>
		{
			owner.velocity.x = owner.airBackdashSpeed;
			owner.canDoubleJump = false;
			owner.canAirDash = false;
		}, false, false);

		AddGatling(new List<char[]>() { new char[] { '4', 'p' }, new char[] { '4', 'p' } }, () => owner.facingRight && CheckAirDashConditions(), "AirBackdash", () =>
		{
			owner.velocity.x = owner.airBackdashSpeed * -1;
			owner.canDoubleJump = false;
			owner.canAirDash = false;
		}, false, false);

		// EASY AIRDASH
		AddGatling(new char[] { 'c', 'p' }, () => owner.CheckFlippableHeldKey('6') && CheckAirDashConditions(), "AirDash", () =>
		{
			owner.velocity.x = owner.airDashSpeed;
			if (!owner.facingRight)
				owner.velocity.x *= -1;
			owner.canDoubleJump = false;
			owner.canAirDash = false;
		});
		
		AddGatling(new char[] { 'c', 'p' }, () => owner.CheckFlippableHeldKey('4') && CheckAirDashConditions(), "AirBackdash", () => {
			owner.velocity.x = owner.airBackdashSpeed;
			if (owner.facingRight)
				owner.velocity.x *= -1;
			owner.canDoubleJump = false;
			owner.canAirDash = false;
		});

		AddGatling(new char[] { '6', 'p' },
			() => owner.CheckBuffer(new char[] { 'c', 'p' }) && owner.canAirDash && owner.facingRight&& owner.internalPos.y < Globals.MAXAIRDASHDEPTH,
			"AirDash",
			() =>
		{
			owner.velocity.x = owner.airDashSpeed;
			owner.canDoubleJump = false;
			owner.canAirDash = false;
		});

		AddGatling(new char[] { '6', 'p' },
			() => owner.CheckBuffer(new char[] { 'c', 'p' }) && owner.canAirDash && !owner.facingRight&& owner.internalPos.y < Globals.MAXAIRDASHDEPTH,
			"AirBackdash",
			() =>
		{
			owner.velocity.x = owner.airBackdashSpeed;
			owner.canDoubleJump = false;
			owner.canAirDash = false;
		});

		AddGatling(new char[] { '4', 'p' },
			() => owner.CheckBuffer(new char[] { 'c', 'p' }) && owner.canAirDash && owner.facingRight && owner.internalPos.y < Globals.MAXAIRDASHDEPTH,
			"AirBackdash",
			() =>
		{
			owner.velocity.x = -owner.airBackdashSpeed;
			owner.canDoubleJump = false;
			owner.canAirDash = false;
		});

		AddGatling(new char[] { '4', 'p' },
			() => owner.CheckBuffer(new char[] { 'c', 'p' }) && owner.canAirDash && !owner.facingRight && owner.internalPos.y < Globals.MAXAIRDASHDEPTH,
			"AirDash",
			() =>
		{
			owner.velocity.x = -owner.airDashSpeed;
			owner.canDoubleJump = false;
			owner.canAirDash = false;
		});
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
				EnterBlockState("Block", details.collisionPnt, details.hitStop);

			else if (rightBlock || leftBlock || anyBlock)
			{
				EnterBlockState("Block", details.collisionPnt, details.hitStop);
			}
			else
			{
				EnterHitState(details.knockdown, details.opponentLaunch, details.collisionPnt, details.effect, details.graphicFX);
			}
		}
	}
}
