using Godot;
using System;

public class Knockdown : HitState
{
    public override void _Ready()
    {
        base._Ready();
        loop = false;
    }
    public override void Enter()
    {
        frameCount = 0;
        owner.velocity.x = 0;
        owner.velocity.y = 0;
        owner.ScheduleEvent(EventScheduler.EventType.AUDIO);
        owner.EmitSignal(nameof(Player.GenericGFX), "Knockdown", owner.Name);
        //owner.GFXEvent("Blood");
        ResetTerminalVelocity();
    }
    public override void AnimationFinished()
    {

        owner.ResetComboAndProration();
        owner.invulnFrames = 1;
        EmitSignal(nameof(StateFinished), "Idle");
    }

    public override void ReceiveHit(Globals.AttackDetails details)
    {
        ReceiveHitNoBlock(details);
    }

    public override bool IsProjectileInvuln()
    {
        return true;
    }

    public override bool DelayInputs()
    {
        return frameCount > animationLength - 3;
    }
    
    public override void TrySpecialBreak()
    {
        base.TrySpecialBreak();
		owner.SpecialBreak();
    }
}
