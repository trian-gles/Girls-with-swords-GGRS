using Godot;
using System.Collections.Generic;

public class Block : HitState
{

	public override HashSet<Globals.Tags> tags { get; set; } = new HashSet<Globals.Tags>() { Globals.Tags.block };
	protected char[] guardCancelKeys = new[] { 'p', 'k' };
	private string blockString = "Block";
	private string crouchBlockString = "CrouchBlock";
	private string shieldString = "Shield";
	private string idleString = "Idle";
	private string fallString = "Fall";
	private string mixupString = "Mixup";
	private string lightString = "Light";
	private string guardCancelString = "GuardCancel";
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
		owner.ForceEvent(EventScheduler.EventType.AUDIO, blockString); // this will be inherited by crouchblock
		owner.GainMeter(300);
	}

	public virtual void EnterShieldState()
	{
        owner.ChangeState(shieldString);
    }
	public override void FrameAdvance() // Note that CrouchBlock overrides this!!!!!
	{
		base.FrameAdvance();
		
		stunRemaining--;
		if (stunRemaining == 0)
		{
			if (owner.grounded)
			{
				owner.ChangeState(idleString);
			}
			else
			{
				owner.ChangeState(fallString);
			}
			
		}

		if (owner.CheckHeldKeys(guardCancelKeys) && owner.CheckFlippableHeldKey('6') && owner.TrySpendMeter())
		{
			owner.ChangeState(guardCancelString);
		}

		if (!owner.grounded)
		{
			ApplyGravity();
		}
	}

	public override GFXStates GetExtraGFXState()
	{
		if (owner.CheckHeldKeys(guardCancelKeys) && owner.CheckFlippableHeldKey('4'))
			return GFXStates.SHIELDACTIVE;
		return GFXStates.NONE;
	
	}

	public override void receiveStun(int hitStun, int blockStun)
	{
		stunRemaining = blockStun;
	}
	
	public override void ReceiveStunDamage(Globals.AttackDetails details)
	{
		owner.GFXEvent(lightString, details.collisionPnt / 100);
		stunRemaining = details.blockStun;
		if (details.chipDmg)
			owner.DeductHealth(details.dmg);
	}

	protected override void ReceiveHighBlock(Globals.AttackDetails details, bool leftBlock, bool rightBlock, bool anyBlock)
    {
        if (owner.CheckOverrideBlock())
			EnterBlockState(blockString, details.collisionPnt, details.hitStop);
		else if (!owner.CheckHeldKey('2'))
		{
			EnterBlockState(blockString, details.collisionPnt, details.hitStop);
		}
		else
		{
			if (owner.CheckFlippableHeldKey('4'))
				owner.EmitSignal(mixupString, owner.Name);
			EnterHitState(details.knockdown, details.opponentLaunch, details.collisionPnt, details.effect, details.graphicFX);
		}
    }

	protected override void ReceiveMidBlock(Globals.AttackDetails details, bool leftBlock, bool rightBlock, bool anyBlock)
    {
        if (owner.CheckHeldKey('2') && owner.grounded)
			EnterBlockState(crouchBlockString, details.collisionPnt, details.hitStop);
		else
			EnterBlockState(blockString, details.collisionPnt, details.hitStop);
    }


    public override void TrySpecialBreak()
    {
        base.TrySpecialBreak();
		owner.SpecialBreak();
    }
}

