using Godot;
using System;

public class BlackHolePlace : Hadouken
{
	public override void Enter()
	{
		base.Enter();
		owner.velocity.y = 0;
		
		owner.landingRecoveryFramesRemaining = 7;
		owner.ScheduleEvent(EventScheduler.EventType.AUDIO, "WarpSpawn", Name);
	}

	public bool CanBlackHole()
	{
		for (int i = 0; i < cachedHadoukens.Count; i++)
		{
			if (cachedHadoukens[i].active)
				return false;
		}
		return true;
	}

	public override void FrameAdvance()
	{
		base.FrameAdvance();
		if (frameCount == 1)
		{
			if (!CanBlackHole())
			{
				owner.ChangeState("Fall");
				return;
			}

		}
	}
}
