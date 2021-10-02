using Godot;
using System;

public class Knockdown : State
{
    public override void Enter()
    {
        base.Enter();
        owner.ComboUp();
        owner.ScheduleEvent(EventScheduler.EventType.AUDIO);
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

    public override void ReceiveHit(bool rightAttack, HEIGHT height, int hitPush, Vector2 launch, bool knockdown)
    {
        // No OTG
    }
}
