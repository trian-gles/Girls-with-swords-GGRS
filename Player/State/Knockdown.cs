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

    public override void ReceiveHit(bool rightAttack, HEIGHT height, int hitPush, Vector2 launch, bool knockdown)
    {
        // No OTG
    }

    public override void receiveStun(int hitStun, int blockStun)
    {
        
    }

    public override void receiveDamage(int dmg)
    {
        
    }
}
