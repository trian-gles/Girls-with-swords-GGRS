using Godot;
using System;

public class Down : Knockdown
{
    public override void Enter()
    {
        frameCount = 0;
        owner.velocity.x = 0;
        owner.velocity.y = 0;
        owner.ScheduleEvent(EventScheduler.EventType.AUDIO);
        //owner.GFXEvent("Blood");
        ResetTerminalVelocity();
    }
    public override void AnimationFinished()
    {
        
    }
}
