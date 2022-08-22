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
    }
    public override void AnimationFinished()
    {

        owner.ResetComboAndProration();
        EmitSignal(nameof(StateFinished), "Idle");
    }

    public override void Exit()
    {
        base.Exit();
        owner.invulnFrames = 2;
    }
}
