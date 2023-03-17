using Godot;
using System;

public class Gunblazed : GroundAttack
{
    public override void Enter()
    {
        base.Enter();
        owner.ScheduleEvent(EventScheduler.EventType.AUDIO, "Fire2", Name);
    }
}
