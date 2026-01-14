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
                owner.ChangeState("Idle");
            }
            else
            {
                ((HL)owner).WarpToHat();

                owner.CommandHadouken("Hat", HadoukenPart.ProjectileCommand.DeleteHat);

                owner.grounded = false;
            }
        }
        owner.CheckTurnAround();
        if (frameCount == teleFrame + 1)
        {
            owner.ChangeState("Fall");
        }
        base.FrameAdvance();
    }

    public override bool DelayInputs()
    {
        return true;
    }
}
