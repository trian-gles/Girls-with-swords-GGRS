using Godot;
using System;
using System.Collections.Generic;
class ArcProjectile : HadoukenPart
{
	public override void FrameAdvance()
	{
		if (frame > 0) {
			Position = new Vector2(Position.x, (float)(( Math.Pow(frame - 28, 2) / 2 - 196) + 270));
		}
		
		base.FrameAdvance();
		
	}
}
