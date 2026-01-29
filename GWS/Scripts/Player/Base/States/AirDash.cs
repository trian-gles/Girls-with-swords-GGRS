using Godot;
using System;

public class AirDash: Fall
{
	[Export]
	public int len = 20;

	[Export]
	public int hopForce = 100;

	[Export]
	private int preAttackFrames = 6;

	private string backdashString = "Backdash";
	private string airDashString = "AirDash";
	private string fallString = "Fall";

	public override void _Ready()
	{
		base._Ready();
		loop = false;
	}

	public override void Enter()
	{
		base.Enter();
		owner.ScheduleEvent(EventScheduler.EventType.AUDIO, backdashString, airDashString);
		owner.velocity.y = 0;
	}

	public override void FrameAdvance()
	{
		frameCount++;
		if (frameCount == len)
		{
			owner.ChangeState(fallString);
		}
		if (frameCount % 5 == 0)
		{
			globalsEvents.EmitSignal(nameof(GhostEmitted), (Player)owner);
		}
	}

	public override bool DelayInputs()
	{
		return frameCount < 10;
	}

	public override void Exit()
	{
		base.Exit();
		owner.airDashFrames = len - frameCount;
		owner.velocity.x = (float)Math.Floor(owner.velocity.x / 2);

    }
}
