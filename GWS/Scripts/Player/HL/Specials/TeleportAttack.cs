using Godot;
using System;

public class TeleportAttack : LaunchAttack
{


    [Export]
    public int teleFrame;

    public override void Enter()
    {
        base.Enter();
        owner.ScheduleEvent(EventScheduler.EventType.AUDIO, Name, Name);
    }
    public override void FrameAdvance()
    {

        if (frameCount == teleFrame)
        {
                ((HL)owner).WarpToHat();

                owner.CommandHadouken("Hat", HadoukenPart.ProjectileCommand.DeleteHat);

                owner.grounded = false;
        }
        owner.CheckTurnAround();
        base.FrameAdvance();
    }
}
