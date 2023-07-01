using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;
class BlackholePower : AmbigAttack
{

    [Export]
    public int powerUpFrame;

    [Export]
    public int powerUpDuration;

    public override void FrameAdvance()
    {
        base.FrameAdvance();

        if (frameCount == powerUpFrame)
        {
            if (owner.otherPlayer.AreHitboxesActive())
            {
                InHurtbox(owner.otherPlayer.internalPos);
                ((GL)owner).PoweredBlackHoleFramesRemaining = powerUpDuration;
            }
            
        }
    }
}
