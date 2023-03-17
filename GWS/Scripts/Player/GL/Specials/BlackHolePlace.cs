using Godot;
using System;

public class BlackHolePlace : Hadouken
{
    public override void FrameAdvance()
    {
        base.FrameAdvance();
        owner.velocity.y = 0;
    }
}