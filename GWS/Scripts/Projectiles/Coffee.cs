using Godot;
using System;
using System.Collections.Generic;
using FixedMath.NET;


class Coffee : HadoukenPart
{
	private const string CoffeeString = "coffee";
	private const string CoffeeExplosionString = "CoffeeExplosion";
	Fix64 offset = new Fix64(210);
	Fix64 mult = new Fix64(32);
	Fix64 div = new Fix64(8);
	public override void FrameAdvance()
	{
		if (active)
		{
			var sloFrame = (Fix64)(frame + 2) / div;
			var y = Fix64.Sin(sloFrame) * mult + offset;
			Position = new Vector2(Position.x, (float)Math.Floor((float)y));
		}
		

		base.FrameAdvance();

	}

	protected override void HurtPlayer(Vector2 collisionPnt)
	{
		base.HurtPlayer(collisionPnt);
		Globals.EmitPlayerFXEmitted(Position * 100,
			CoffeeString, movingRight);
		targetPlayer.ScheduleEvent(EventScheduler.EventType.GRAPHIC, CoffeeExplosionString);
	}
}
