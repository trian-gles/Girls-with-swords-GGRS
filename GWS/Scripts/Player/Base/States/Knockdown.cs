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
        owner.GFXEvent("Blood");
        ResetTerminalVelocity();
    }
    public override void AnimationFinished()
    {

        owner.ResetComboAndProration();
        owner.invulnFrames = 2;
        EmitSignal(nameof(StateFinished), "Idle");
    }
}
