using Godot;
using System;

public class Stagger : HitStun
{
	[Export]
	public int dur = 40;
	public override void _Ready()
	{
		base._Ready();
		loop = true;
	}

	public override void Enter()
	{
		frameCount = 0;
		if (stop)
		{
			owner.velocity.x = 0;
		}
		stunRemaining = dur;
		owner.ScheduleEvent(EventScheduler.EventType.AUDIO, "Stagger", Name);
	}

	/// <summary>
	/// all staggers are the same length
	/// </summary>
	/// <param name="hitStun"></param>
	/// <param name="blockStun"></param>
	public override void receiveStun(int hitStun, int blockStun)
	{
		stunRemaining = dur;
	}
}

