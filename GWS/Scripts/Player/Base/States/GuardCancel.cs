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
		owner.EmitSignal(nameof(Player.GenericGFX), GuardCancelString, owner.Name); // ALLOCATION
		owner.ScheduleEvent(EventScheduler.EventType.AUDIO, GuardCancelString, Name);
    }
}
