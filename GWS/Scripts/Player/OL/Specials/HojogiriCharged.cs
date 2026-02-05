using Godot;
using System;

public class HojogiriCharged : Hojogiri
{
    private const string HojogiriAnimString = "Hojogiri";
    private const string HojogiriChargedSlashString = "HojogiriChargedSlash";
    public override string animationName { get { return HojogiriAnimString; } }

    public override void FrameAdvance()
    {
        base.FrameAdvance();
        if (frameCount > 10)
            owner.ChangeState(HojogiriChargedSlashString);
    }

}
