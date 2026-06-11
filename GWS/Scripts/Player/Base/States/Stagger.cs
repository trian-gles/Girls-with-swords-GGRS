using Godot;
using System;

public class Stagger : HitStun
{
	[Export]
	public int dur = 40;

	public override int maxStun { get { return 40; } }

	private const string StaggerString = "Stagger";
	public override void _Ready()
	{
		base._Ready();
		loop = true;
	}

	public override void Enter()
	{
		base.Enter();
		owner.ScheduleEvent(EventScheduler.EventType.AUDIO, StaggerString, Name);
	}
}

