using Godot;
using System;

public class BackToss : Hadouken
{
	[Export]
	public int earlyReleaseFrame = 14;

    public override void Enter()
    {
		base.Enter();
		owner.ScheduleEvent(EventScheduler.EventType.AUDIO, "BackToss", Name);
    }
	public override void FrameAdvance()
	{
		base.FrameAdvance();
		var sl = (SL)owner;
		if (sl.leftCornerSnail && sl.rightCornerSnail && frameCount > earlyReleaseFrame)
			owner.ChangeState("Idle");

	}
}
