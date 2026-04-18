using Godot;
using System;
using static State;
using System.Collections.Generic;

public class Shield : HitState
{
	private const string BlockAnim = "Block";
	private const string IdleString = "Idle";
	private const string WalkString = "Walk";
	private const string FallString = "Fall";
	private const string CrouchShieldString = "CrouchShield";
	private const string ShieldStateString = "Shield";
	private const string CrouchBlockString = "CrouchBlock";
	private const string ShieldFxString = "shield";
	private const string HitConfirmString = "HitConfirm";
	private const string BlockString = "Block";
	private const string LightString = "Light";

	public override HashSet<Globals.Tags> tags { get; set; } = new HashSet<Globals.Tags>() { Globals.Tags.block, Globals.Tags.shield };
	protected char[] requiredKeys = new[] { 'p', 'k', '4' };

	public override string animationName { get { return BlockAnim; } }
	public override void _Ready()
	{
		base._Ready();
		loop = true;
		stop = false;
	}

    public override void Enter()
    {
        base.Enter();
		ResetTerminalVelocity();
		if (owner.grounded)
			owner.velocity.x = 0;
    }

	public override void FrameAdvance()
	{
		base.FrameAdvance(); 

		if (stunRemaining == 0)
		{
			CheckShieldSwitch();
			if (!owner.TrySpendMeter(5)) {
				owner.EmptyMeter();
				ExitShield();
			}

			bool ownerHoldingInput = owner.CheckHeldFlippableKeys(requiredKeys);
			if (!ownerHoldingInput && (stunRemaining == 0) && (frameCount > 2))
			{
				ExitShield();
			}

		}
		else
		{
			stunRemaining--;
		}


		if (!owner.grounded)
		{
			ApplyGravity();
		}
			
	}

	protected virtual void ExitShield()
	{
		if (owner.grounded)
		{
			if (owner.CheckHeldKey('4'))
			{
				owner.velocity.x = -owner.speed;
				owner.ChangeState(WalkString);
			}
			else if (owner.CheckHeldKey('6')) {
				owner.velocity.x = owner.speed;
				owner.ChangeState(WalkString);
			}
			owner.ChangeState(IdleString);
		}
		else
		{
			owner.ChangeState(FallString);
		}
	}

	protected virtual void CheckShieldSwitch()
	{
		if (owner.CheckHeldKey('2') && owner.grounded)
			owner.ChangeState(CrouchShieldString);
	}


	public override GFXStates GetExtraGFXState()
	{
		if (stunRemaining > 0)
			return GFXStates.SHIELDACTIVE;
		else return GFXStates.SHIELD;
	}

	public override bool DelayInputs()
	{
		return false;
	}

	public override void ReceiveHit(Globals.AttackDetails details)
	{
		details.hitPush = (int)Math.Floor(details.hitPush * 1.5f);
		details.airBlockable = true;
		base.ReceiveHit(details);
	}


	public override void receiveStun(int hitStun, int blockStun)
	{

		stunRemaining = blockStun + 3;
		owner.ForceEvent(EventScheduler.EventType.AUDIO, BlockString); // this will be inherited by crouchblock
	}

	public override void ReceiveStunDamage(Globals.AttackDetails details)
	{
		owner.GFXEvent(LightString, details.collisionPnt / 100);
		if (!owner.TrySpendMeter(300)) owner.EmptyMeter();

		stunRemaining = details.blockStun;
	}

	public override bool IsGrabbable()
	{
		return stunRemaining == 0;
	}

	public override void TrySpecialBreak()
	{
		base.TrySpecialBreak();
		if (stunRemaining > 0)
			owner.SpecialBreak();
	}

	protected override void EnterBlockState(string stateName, Vector2 collisionPnt, int blockStop)
	{
		if (stateName == BlockString)
			stateName = ShieldStateString;
		else if (stateName == CrouchBlockString)
			stateName = CrouchShieldString;
		Globals.EmitPlayerFXEmitted(collisionPnt, ShieldFxString, owner.OtherPlayerOnLeft());
		owner.ChangeState(stateName);
		Globals.EmitSignal(Globals.PlayerSignal.HitStop, owner.Name, blockStop);
	}

    public override void Land()
    {
        base.Land();
		owner.velocity.x = 0;
    }
}
