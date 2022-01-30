using Godot;
using System;

public class AirDash: Fall
{
	[Export]
	public int len = 20;

	[Export]
	public int hopForce = 100;

	public override void Enter()
	{
		base.Enter();
		owner.ScheduleEvent(EventScheduler.EventType.AUDIO, "Backdash", "AirDash");
		owner.velocity.y = 0;
	}

	public override void FrameAdvance()
	{
		frameCount++;
		if (frameCount == len)
		{
			EmitSignal(nameof(StateFinished), "Fall");
		}
	}
}
