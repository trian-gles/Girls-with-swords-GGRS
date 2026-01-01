using Godot;
using System;
using System.Collections.Generic;
using FixedMath.NET;


class StrikeSnail : HadoukenPart
{

	public override void Spawn(bool movingRight, Player targetPlayer)
	{
		base.Spawn(movingRight, targetPlayer);
		if (!movingRight)
		{
			GetNode<Sprite>("Sprite").RotationDegrees = -45;
			GetNode<Sprite>("Sprite").Position = new Vector2(24, -28);
			GetNode<Sprite>("Sprite").FlipH = true;
			GetNode<AnimatedSprite>("AnimatedSprite").RotationDegrees = -45;
		}
	}

}
