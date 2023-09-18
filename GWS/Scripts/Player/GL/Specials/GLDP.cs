using Godot;
using System;

public class GLDP : LaunchAttack
{
    [Export]
    public int knockdownFrame = 30;

    Globals.AttackDetails finalAttack = Globals.attackLevels[3].hit;

    public override void _Ready()
    {
        base._Ready();
        finalAttack.hitStun = 70;
        finalAttack.knockdown = true;
        finalAttack.graphicFX = BaseAttack.GRAPHICEFFECT.EXPLOSION;

    }
    public override void Enter()
    {
        base.Enter();
        owner.ScheduleEvent(EventScheduler.EventType.AUDIO, "Fire1", Name);
    }

    public override void FrameAdvance()
    {
        base.FrameAdvance();
        if (frameCount == knockdownFrame && hitConnect)
        {
            owner.otherPlayer.ReceiveHit(finalAttack, finalAttack);
        }
    }
}
