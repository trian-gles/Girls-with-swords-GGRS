using Godot;
using System;

public class HojogiriCharged : Hojogiri
{
    public override void _Ready()
    {
        base._Ready();
        animationName = "Hojogiri";
    }

    public override void FrameAdvance()
    {
        base.FrameAdvance();
        if (frameCount > 10)
            EmitSignal(nameof(StateFinished), "HojogiriChargedSlash");
    }

}
