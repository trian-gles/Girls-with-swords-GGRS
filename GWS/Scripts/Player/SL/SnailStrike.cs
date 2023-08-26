using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;

class SnailStrike : Hadouken
{

	[Export]
	public int xOffset = 0;

    [Export]
    public int successiveXOffset = 20;

	[Export]
	public int gapBetweenStrikes = 20;

    public override void FrameAdvance()
    {
        base.FrameAdvance();
		if (frameCount > releaseFrame)
        {
			int strikeCycle = (frameCount - releaseFrame) % gapBetweenStrikes;
			if (strikeCycle == 0)
            {
				EmitHadouken();
            }
        }
    }

    protected override void EmitHadouken()
    {
        int strikeNum = (frameCount - releaseFrame) / gapBetweenStrikes;


        var h = hadoukenScene.Instance() as HadoukenPart;

        h.Spawn(owner.facingRight, owner.otherPlayer);
        owner.EmitHadouken(h);
        GD.Print(strikeNum);

        int displacement = strikeNum * successiveXOffset + xOffset;
        if (!owner.facingRight)
            displacement *= -1;
        GD.Print(displacement);
        h.Position = new Vector2(owner.Position.x  + displacement, owner.Position.y + yOffset);
    }


}
