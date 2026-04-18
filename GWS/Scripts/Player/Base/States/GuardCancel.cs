using Godot;
using System;

public class GuardCancel : GroundAttack
{

	private const string GuardCancelString = "GuardCancel";

	public override void _Ready()
	{
		base._Ready();
	}

	public override void Enter()
	{
		base.Enter();
		owner.GFXEvent(GuardCancelString);
		Globals.EmitPlayerGenericGfx( GuardCancelString, owner.Name);
		owner.ScheduleEvent(EventScheduler.EventType.AUDIO, GuardCancelString, Name);
    }
}
