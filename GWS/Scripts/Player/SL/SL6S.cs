using Godot;
using System;

public class SL6S : SixK
{
    [Export]
    public string holdState = "6CH";

    [Export]
    public int holdFrame = 15;

    [Export]
    public string holdKey = "s";
    public override void FrameAdvance()
    {
        base.FrameAdvance();
        if (frameCount == holdFrame && owner.CheckHeldKey(holdKey[0]))
        {
            owner.ChangeState(holdState);
        }
    }
}