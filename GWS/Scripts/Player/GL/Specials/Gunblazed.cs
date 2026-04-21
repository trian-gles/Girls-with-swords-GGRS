using Godot;
using System;

public class Gunblazed : GroundAttack
{
	private const string Fire2String = "Fire2";
	public override void Enter()
	{
		base.Enter();
		owner.ScheduleEvent(EventScheduler.EventType.AUDIO, Fire2String, Name);
	}
}
