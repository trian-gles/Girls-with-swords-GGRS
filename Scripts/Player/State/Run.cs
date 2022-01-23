using Godot;
using System;

public class Run : Walk
{
	public override void _Ready()
	{
		AddGatling(new[] { '6', 'r' }, "PostRun", () => { owner.velocity.x = (float)Math.Floor(owner.velocity.x / 2); });
		AddGatling(new[] { '4', 'r' }, "PostRun", () => { owner.velocity.x = (float)Math.Floor(owner.velocity.x / 2); });
		base._Ready();
		
		soundRate = 10;
	}

	public override void Enter()
	{
		base.Enter();
		if (owner.CheckHeldKey('8'))
		{
			EmitSignal(nameof(StateFinished), "MovingJump");
		}
	}

	public override void FrameAdvance()
	{
		frameCount++;
		if (frameCount == 5)
		{
			owner.velocity.x *= 2;
		}

		if (frameCount % soundRate == 0)
		{
			owner.ScheduleEvent(EventScheduler.EventType.AUDIO, "Step", Name);
		}
	}
}
