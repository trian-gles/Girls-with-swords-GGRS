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
		owner.velocity.x *= 2;
		if (owner.CheckHeldKey('8'))
		{
			EmitSignal(nameof(StateFinished), "MovingJump");
		}
		if (!owner.CheckHeldKey('6') && !owner.CheckHeldKey('4')) // this will need to be fixed
		{
			EmitSignal(nameof(StateFinished), "PostRun");
		}
	}

	public override void FrameAdvance()
	{
		frameCount++;

		if (frameCount % soundRate == 0)
		{
			owner.ScheduleEvent(EventScheduler.EventType.AUDIO, "Step", Name);
		}
	}
}
