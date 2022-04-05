using Godot;
using System;

public class Grabbed : State
{
	public override void FrameAdvance()
	{
		base.FrameAdvance();
		owner.velocity = new Vector2(0, 0);
	}

	/// <summary>
	/// This is a little bit weird that I'm using ReceiveHit here!  This essentially damages the defender and triggers the release
	/// </summary>
	/// <param name="rightAttack"></param>
	/// <param name="height"></param>
	/// <param name="hitPush"></param>
	/// <param name="launch"></param>
	/// <param name="knockdown"></param>
	public override void ReceiveHit(Globals.AttackDetails details)
	{
		switch (details.dir)
		{
			case BaseAttack.ATTACKDIR.RIGHT:
				break;
			case BaseAttack.ATTACKDIR.LEFT:
				details.opponentLaunch.x *= -1;
				details.hitPush *= -1;
				break;
			case BaseAttack.ATTACKDIR.EQUAL:
				details.opponentLaunch.x = 0;
				details.hitPush = 0;
				break;
		}
		owner.velocity = details.opponentLaunch;
		owner.ComboUp();
		owner.grounded = false;

		EmitSignal(nameof(StateFinished), "AirKnockdown");
	}
}
