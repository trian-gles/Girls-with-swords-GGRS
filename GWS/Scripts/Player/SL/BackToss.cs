using Godot;
using System;

public class BackToss : Hadouken
{
	[Export]
	public int earlyReleaseFrame = 14;

	private const string BackTossString = "BackToss";
	private const string IdleString = "Idle";

    public override void Enter()
    {
		base.Enter();
		owner.ScheduleEvent(EventScheduler.EventType.AUDIO, BackTossString, Name);
    }
	public override void FrameAdvance()
	{
		base.FrameAdvance();
		var sl = (SL)owner;
		if (sl.leftCornerSnail && sl.rightCornerSnail && frameCount > earlyReleaseFrame)
				owner.ChangeState(IdleString);

	}
}
