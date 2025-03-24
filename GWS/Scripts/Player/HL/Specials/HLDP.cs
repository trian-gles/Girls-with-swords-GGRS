using Godot;
using System;

public class HLDP : DP
{
    [Export]
    public int throwFrame = 20;
    public override void FrameAdvance()
    {
        base.FrameAdvance();
        if (frameCount == throwFrame && ((HL)owner).hatted && owner.CheckHeldKey('a'))
            EmitSignal(nameof(StateFinished), "AirHat");
    }
}