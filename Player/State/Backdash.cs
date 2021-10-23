using Godot;
using System;

public class Backdash : State
{
    [Export]
    public int len = 20;

    public override void Enter()
    {
        base.Enter();
        owner.ScheduleEvent(EventScheduler.EventType.AUDIO);
    }

    public override void FrameAdvance()
    {
        base.FrameAdvance();
        if (frameCount == len)
        {
            EmitSignal(nameof(StateFinished), "Idle");
        }
    }

    public override void ReceiveHit(bool rightAttack, HEIGHT height, int hitPush, Vector2 launch, bool knockdown)
    {
        if (!rightAttack)
        {
            launch.x *= -1;
            hitPush *= -1;
        }

        owner.hitPushRemaining = hitPush;

        if (owner.velocity.y < 0)
        {
            owner.grounded = false;
        }

        EnterHitState(knockdown, launch);
    }
}
