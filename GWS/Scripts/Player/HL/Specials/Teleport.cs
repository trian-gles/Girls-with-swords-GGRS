using Godot;
using System;

public class Teleport : BaseAttack
{
    [Export]
    public int teleFrame;
    public override void FrameAdvance()
    {
        
        if (frameCount == teleFrame)
        {
            if (((HL)owner).hatted)
            {
                EmitSignal(nameof(StateFinished), "Idle");
            }
            else
            {
                ((HL)owner).WarpToHat();
                
                owner.CommandHadouken("Hat", HadoukenPart.ProjectileCommand.DeleteHat);
                
                owner.grounded = false;
            }
        }
        if (frameCount == teleFrame + 1)
        {
            EmitSignal(nameof(StateFinished), "Fall");
        }
        base.FrameAdvance();
    }
}
