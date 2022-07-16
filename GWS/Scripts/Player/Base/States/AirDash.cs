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

	[Signal]
	public delegate void GhostEmitted(Player p);

	public override void _Ready()
	{
		base._Ready();
		loop = false;
	}

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
		if (frameCount % 5 == 0)
		{
			//GD.Print("Airdash emitting ghost");
			GetNode<Node>("/root/Globals").EmitSignal(nameof(GhostEmitted), (Player)owner);
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
	}
}
