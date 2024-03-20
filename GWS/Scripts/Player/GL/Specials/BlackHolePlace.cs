using Godot;
using System;

public class BlackHolePlace : Hadouken
{
    GL gl;
    public override void _Ready()
    {
        base._Ready();
        gl = ((GL)owner);
    }
    public override void Enter()
    {
        base.Enter();
        if (gl.BlackHolesTotal > 1)
        {
            EmitSignal(nameof(StateFinished), "Fall");
            return;
        }
            
        owner.ScheduleEvent(EventScheduler.EventType.AUDIO, "WarpSpawn", Name);
    }
    public override void FrameAdvance()
    {
        base.FrameAdvance();
        owner.velocity.y = 0;

        if (frameCount == releaseFrame)
            gl.BlackHolesTotal++;
    }
}