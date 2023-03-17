using Godot;
using System;

public class GLDP : LaunchAttack
{
    public override void Enter()
    {
        base.Enter();
        owner.ScheduleEvent(EventScheduler.EventType.AUDIO, "Fire1", Name);
    }
}
