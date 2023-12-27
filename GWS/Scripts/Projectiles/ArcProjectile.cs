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
		Position = new Vector2(Position.x, (float)((Math.Pow(frame - 28, 2) / 2 - 196) + 270));

		// derivative of position
		float slope = frame - 28;
		float rotation = (float)(Math.Tanh(slope));
		// GD.Print(rotation);
		GetNode<AnimatedSprite>("AnimatedSprite").Rotation = rotation + (float)Math.PI / 3 + (float)Math.PI / 2;

		base.FrameAdvance();

	}
}
