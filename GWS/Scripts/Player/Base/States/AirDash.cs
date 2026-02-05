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

	private const string BackdashString = "Backdash";
	private const string AirDashString = "AirDash";
	private const string FallString = "Fall";

	public override void _Ready()
	{
		base._Ready();
		loop = false;
	}

	public override void Enter()
	{
		base.Enter();
		owner.ScheduleEvent(EventScheduler.EventType.AUDIO, BackdashString, AirDashString);
		owner.velocity.y = 0;
	}

	public override void FrameAdvance()
	{
		frameCount++;
		if (frameCount == len)
		{
			owner.ChangeState(FallString);
		}
		if (frameCount % 5 == 0)
		{
			Globals.EmitGhostEmitted(owner);
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
