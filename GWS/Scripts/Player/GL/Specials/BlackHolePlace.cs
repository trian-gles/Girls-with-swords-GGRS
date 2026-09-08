using Godot;
using System;

public class BlackHolePlace : Hadouken
{
	public override void Enter()
	{
		base.Enter();
		owner.velocity.y = 0;
		
		owner.landingRecoveryFramesRemaining = 5;
		owner.ScheduleEvent(EventScheduler.EventType.AUDIO, "WarpSpawn", Name);
	}

	protected virtual bool CanBlackHole()
	{
		return ((GL)owner).CanBlackHole();
	}

	public bool NoActiveBlackHoles()
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

	public override void AnimationFinished()
	{
		if ((owner.CheckHeldKey('s') || owner.CheckHeldKey('a'))&& Name == "BlackHolePlace")
		{
			owner.ChangeState("BlackHoleHold");
		}
		else
		{
			base.AnimationFinished();
		}
			
		
	}
}
