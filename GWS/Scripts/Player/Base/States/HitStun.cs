using Godot;
using System;

public class HitStun : HitState
{
	public override void _Ready()
	{
		base._Ready();
		loop = false;
	}
	public override void Enter()
	{
		base.Enter();
		
		owner.ScheduleEvent(EventScheduler.EventType.AUDIO, "HitStun", Name);
		owner.GFXEvent("Blood");
	}


	public override void FrameAdvance()
	{
		base.FrameAdvance();
		if (owner.counterStopFrames == 0)
			stunRemaining--;

		if (stunRemaining == 0)
		{
			EmitSignal(nameof(StateFinished), "Idle");
		}

	}
	
	public override void ReceiveHit(Globals.AttackDetails details)
	{
		ReceiveHitNoBlock(details);
	}
}

