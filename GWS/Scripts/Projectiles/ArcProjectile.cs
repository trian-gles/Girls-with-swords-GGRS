using Godot;
using System;
using System.Collections.Generic;
class ArcProjectile : HadoukenPart
{

	public override void _Ready()
	{
		base._Ready();

	}
	public override void FrameAdvance()
	{
		if (frame > 0 && (hits == 0)) {
			Position = new Vector2(Position.x, (float)(( Math.Pow(frame - 28, 2) / 2 - 196) + 270));
		}

		// derivative of position
		float slope = frame - 28;

		GetNode<AnimatedSprite>("AnimatedSprite").Rotation = (float)Math.Tan(slope);

		base.FrameAdvance();
		
	}
}
