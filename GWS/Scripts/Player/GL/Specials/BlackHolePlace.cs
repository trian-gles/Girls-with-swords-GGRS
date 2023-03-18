using Godot;
using System;

public class BlackHolePlace : Hadouken
{

    public override void Enter()
    {
        base.Enter();
        owner.ScheduleEvent(EventScheduler.EventType.AUDIO, "WarpSpawn", Name);
    }
    public override void FrameAdvance()
    {
        base.FrameAdvance();
        owner.velocity.y = 0;
    }
}