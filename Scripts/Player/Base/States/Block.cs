using Godot;
using System;

public class Block : HitState
{
	public override void _Ready()
	{
		base._Ready();
		loop = true;
	}

	public override void Enter()
	{
		base.Enter();
		GD.Print("Blocking");
		owner.ScheduleEvent(EventScheduler.EventType.AUDIO, "Block", owner.currentState.Name); // this will be inherited by crouchblock
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
		GD.Print(details.blockStun);
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
}

