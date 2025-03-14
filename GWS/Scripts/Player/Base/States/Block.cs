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
		owner.ForceEvent(EventScheduler.EventType.AUDIO, "Block"); // this will be inherited by crouchblock
		owner.GainMeter(300);
	}
	public override void FrameAdvance()
	{
		base.FrameAdvance();
		stunRemaining--;
		if (stunRemaining == 0)
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
		if (!owner.grounded)
		{
			ApplyGravity();
		}
	}


	public override void receiveStun(int hitStun, int blockStun)
	{
		
		stunRemaining = blockStun;
	}
	
	public override void ReceiveStunDamage(Globals.AttackDetails details)
	{
		owner.GFXEvent("Light", details.collisionPnt / 100);
		stunRemaining = details.blockStun;
		owner.DeductHealth(details.dmg);
	}

	/// <summary>
	/// Not multiplied by proration because this is chip damage.  Also no proration is applied
	/// </summary>
	/// <param name="dmg"></param>
	public override void receiveDamage(int dmg, int prorationLevel)
	{
		owner.DeductHealth(dmg);
	}


    public override void TrySpecialBreak()
    {
        base.TrySpecialBreak();
		owner.SpecialBreak();
    }
}

