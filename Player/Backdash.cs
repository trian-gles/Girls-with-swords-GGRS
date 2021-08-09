using Godot;
using System;

public class Backdash : State
{
    [Export]
    public int len = 20;

    public override void FrameAdvance()
    {
        base.FrameAdvance();
        if (frameCount == len)
        {
            EmitSignal(nameof(StateFinished), "Idle");
        }
    }
}
