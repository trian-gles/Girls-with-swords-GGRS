using Godot;
using System;
using System.Collections.Generic;
class Snail : HadoukenPart
{

    public override void Spawn(bool movingRight, Player targetPlayer)
    {
        base.Spawn(movingRight, targetPlayer);

    }
    public override void FrameAdvance()
	{
		if (frame > 0 && (hits == 0)) {
			Position = new Vector2(Position.x, (float)(( Math.Pow(frame - 28, 2) / 2 - 196) + 270));
		}
		
		base.FrameAdvance();
		
	}
}
