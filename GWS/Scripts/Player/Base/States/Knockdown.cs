using Godot;
using System;

public class Knockdown : HitState
{
    private const string KnockdownGfxString = "Knockdown";
    private const string IdleString = "Idle";
    public override void _Ready()
    {
        base._Ready();
        loop = false;
        tags.Add(Globals.Tags.knockdown);
    }
    public override void Enter()
    {
        frameCount = 0;
        owner.velocity.x = 0;
        owner.velocity.y = 0;
        owner.ScheduleEvent(EventScheduler.EventType.AUDIO);
        owner.EmitSignal(nameof(Player.GenericGFX), KnockdownGfxString, owner.Name); // ALLOCATION
        //owner.GFXEvent("Blood");
        ResetTerminalVelocity();
    }
    public override void AnimationFinished()
    {

        owner.ResetComboAndProration();
        owner.invulnFrames = 2;
        owner.ChangeState(IdleString);
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
