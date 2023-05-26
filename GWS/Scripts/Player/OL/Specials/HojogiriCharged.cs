using Godot;
using System;

public class HojogiriCharged : Hojogiri
{
    public override string animationName { get { return "Hojogiri"; } }

    public override void FrameAdvance()
    {
        base.FrameAdvance();
        if (frameCount > 10)
            EmitSignal(nameof(StateFinished), "HojogiriChargedSlash");
    }

}
