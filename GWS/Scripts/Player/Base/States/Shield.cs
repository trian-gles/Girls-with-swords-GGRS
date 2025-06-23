using Godot;
using System;
using static State;
using System.Collections.Generic;

public class Shield : HitState
{

	public override HashSet<string> tags { get; set; } = new HashSet<string>() { "block" };

	public override string animationName { get { return "Block"; } }
	public override void _Ready()
	{
		base._Ready();
		loop = true;
		stop = false;
	}

	public override void FrameAdvance()
	{
		base.FrameAdvance();
		
		if (stunRemaining > 0)
		{
			stunRemaining--;
		} 

		if (stunRemaining == 0)
		{
            CheckShieldSwitch();
			if (!owner.TrySpendMeter(5)) {
				ExitShield();
			}

        }
			

		if (!owner.grounded)
		{
			ApplyGravity();
		}

		bool ownerHoldingInput = owner.CheckHeldFlippableKeys(new[] {'p', 'k', '4' });
		if (!ownerHoldingInput && (stunRemaining == 0) && (frameCount > 2))
		{
			ExitShield();
		}

		if (owner.grounded)
			owner.velocity.x = 0;
	}

	protected virtual void ExitShield()
	{
        if (owner.grounded)
        {
            EmitSignal(nameof(StateFinished), "Idle");
        }
        else
        {
            EmitSignal(nameof(StateFinished), "Fall");
        }
    }

    protected virtual void CheckShieldSwitch()
	{
        if (owner.CheckHeldKey('2') && owner.grounded)
            EmitSignal(nameof(StateFinished), "CrouchShield");
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
		details.hitPush = (int)Math.Floor(details.hitPush * 1.5);
		details.airBlockable = true;
		details.hitPush *= 2;
		base.ReceiveHit(details);
	}


    public override void receiveStun(int hitStun, int blockStun)
	{

		stunRemaining = blockStun + 3;
		owner.ForceEvent(EventScheduler.EventType.AUDIO, "Block"); // this will be inherited by crouchblock
	}

	public override void ReceiveStunDamage(Globals.AttackDetails details)
	{
		owner.GFXEvent("Light", details.collisionPnt / 100);
		owner.TrySpendMeter(300);

        stunRemaining = details.blockStun;
	}

	/// <summary>
	/// No chip damage at all
	/// </summary>
	/// <param name="dmg"></param>
	public override void receiveDamage(int dmg, int prorationLevel)
	{
		
	}


	public override void TrySpecialBreak()
	{
		base.TrySpecialBreak();
		owner.SpecialBreak();
	}

    protected override void EnterBlockState(string stateName, Vector2 collisionPnt, int blockStop)
	{
		if (stateName == "Block")
			stateName = "Shield";
        if (stateName == "CrouchBlock")
            stateName = "CrouchShield";
    }
}
