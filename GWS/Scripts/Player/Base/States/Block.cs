using Godot;
using System.Collections.Generic;

public class Block : HitState
{

	public override HashSet<string> tags { get; set; } = new HashSet<string>() { "block" };
	public override void _Ready()
	{
		base._Ready();
		loop = true;
	}

	public override void Enter()
	{
		
		base.Enter();
		if (owner.CheckHeldKeys(new[] { 'p', 'k' }))
		{
			EnterShieldState();
        }
		owner.ForceEvent(EventScheduler.EventType.AUDIO, "Block"); // this will be inherited by crouchblock
		owner.GainMeter(300);
	}

	public virtual void EnterShieldState()
	{
        owner.ChangeState("Shield");
    }
	public override void FrameAdvance() // Note that CrouchBlock overrides this!!!!!
	{
		base.FrameAdvance();
		
		stunRemaining--;
		if (stunRemaining == 0)
		{
			if (owner.grounded)
			{
				owner.ChangeState("Idle");
			}
			else
			{
				owner.ChangeState("Fall");
			}
			
		}

		if (owner.CheckHeldKeys(new[] { 'p', 'k' }) && owner.CheckFlippableHeldKey('6') && owner.TrySpendMeter())
		{
			owner.ChangeState("GuardCancel");
		}

		if (!owner.grounded)
		{
			ApplyGravity();
		}
	}

	public override GFXStates GetExtraGFXState()
	{
		if (owner.CheckHeldKeys(new[] { 'p', 'k' }) && owner.CheckFlippableHeldKey('4'))
			return GFXStates.SHIELDACTIVE;
		return GFXStates.NONE;
		
	}


	public override void receiveStun(int hitStun, int blockStun)
	{

		stunRemaining = blockStun;
	}
	
	public override void ReceiveStunDamage(Globals.AttackDetails details)
	{
		owner.GFXEvent("Light", details.collisionPnt / 100);
		stunRemaining = details.blockStun;
		if (details.chipDmg)
			owner.DeductHealth(details.dmg);
	}

	protected override void ReceiveHighBlock(Globals.AttackDetails details, bool leftBlock, bool rightBlock, bool anyBlock)
    {
        if (owner.CheckOverrideBlock())
			EnterBlockState("Block", details.collisionPnt, details.hitStop);
		else if (!owner.CheckHeldKey('2'))
		{
			EnterBlockState("Block", details.collisionPnt, details.hitStop);
		}
		else
		{
			if (owner.CheckFlippableHeldKey('4'))
				owner.EmitSignal("Mixup", owner.Name);
			EnterHitState(details.knockdown, details.opponentLaunch, details.collisionPnt, details.effect, details.graphicFX);
		}
    }

	protected override void ReceiveMidBlock(Globals.AttackDetails details, bool leftBlock, bool rightBlock, bool anyBlock)
    {
        if (owner.CheckHeldKey('2') && owner.grounded)
			EnterBlockState("CrouchBlock", details.collisionPnt, details.hitStop);
		else
			EnterBlockState("Block", details.collisionPnt, details.hitStop);
    }


    public override void TrySpecialBreak()
    {
        base.TrySpecialBreak();
		owner.SpecialBreak();
    }
}

