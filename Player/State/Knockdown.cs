using Godot;
using System;

public class Knockdown : HitState
{
    public override void Enter()
    {
        frameCount = 0;
        loop = false;
        owner.velocity.x = 0;
        owner.velocity.y = 0;
        owner.ScheduleEvent(EventScheduler.EventType.AUDIO);
        owner.GFXEvent("Blood");
    }
    public override void AnimationFinished()
    {

        owner.ResetCombo();
        if (owner.grounded) 
        {
            EmitSignal(nameof(StateFinished), "Idle");
        }
        else
        {
            EmitSignal(nameof(StateFinished), "Fall");
        }
    }
}
