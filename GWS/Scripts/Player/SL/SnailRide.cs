using Godot;
using System;

public class SnailRide : MovingAttack
{
    private const string SnailRideString = "SnailRide";
    private const string DustString = "dust";
    public override void Enter()
    {
        base.Enter();
        owner.ScheduleEvent(EventScheduler.EventType.AUDIO, SnailRideString, Name);
    }
    public override void FrameAdvance()
    {
        base.FrameAdvance();
        if (frameCount % 5 == 0)
            Globals.EmitPlayerFXEmitted(new Vector2(owner.internalPos.x, owner.GetCollisionRect().End.y), DustString, owner.facingRight);
			
    }
}