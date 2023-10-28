using Godot;
using System;

public class BlackHolePlace : Hadouken
{

    public override void Enter()
    {
        var gl = ((GL)owner);
        base.Enter();
        if (gl.BlackHolesTotal > 1)
        {
            EmitSignal(nameof(StateFinished), "Fall");
            GD.Print("Too many black holes");
            return;
        }
            
        gl.BlackHolesTotal++;
        owner.ScheduleEvent(EventScheduler.EventType.AUDIO, "WarpSpawn", Name);
    }
    public override void FrameAdvance()
    {
        base.FrameAdvance();
        owner.velocity.y = 0;
    }
}