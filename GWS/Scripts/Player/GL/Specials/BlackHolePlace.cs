using Godot;
using System;

public class BlackHolePlace : Hadouken
{
    GL gl;
    private const string WarpSpawnString = "WarpSpawn";
    private const string FallString = "Fall";
    public override void _Ready()
    {
        base._Ready();
        gl = ((GL)owner);
    }
    public override void Enter()
    {
        base.Enter();
        owner.velocity.y = 0;
        
        owner.landingRecoveryFramesRemaining = 7;
        owner.ScheduleEvent(EventScheduler.EventType.AUDIO, WarpSpawnString, Name);
    }

    public override void FrameAdvance()
    {
        base.FrameAdvance();
        if (frameCount == 1)
        {
            if (gl.BlackHolesTotal > 1)
            {
                if (Globals.logOn)
                    Globals.Log($"Too many black holes for {owner.Name}, total black holes = {gl.BlackHolesTotal}");
                owner.ChangeState(FallString);
                return;
            }

        }
    }

    protected override HadoukenPart EmitHadouken()
    {
        var h =  base.EmitHadouken();
        gl.BlackHolesTotal++;
        if (Globals.logOn)
            Globals.Log($"Emitting black hole for {owner.Name}, Total black holes now = {gl.BlackHolesTotal}");
        return h;
    }
}