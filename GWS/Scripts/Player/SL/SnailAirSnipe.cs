using Godot;
using System;
using FixedMath.NET;

public class SnailAirSnipe : AirGrabStart
{
	[Export]
	public Vector2 launch = new Vector2();

	public override void Enter()
	{
		base.Enter();
		owner.grounded = false;
		owner.velocity = launch;

		if (!owner.facingRight)
		{
			owner.velocity.x *= -1;
		}
		
	}

	public override void FrameAdvance()
	{
		base.FrameAdvance();
		if (!owner.otherPlayer.grounded)
			Home();

		

	}

	private void Home()
	{
		

		int framesToReachOpponent = (int)Math.Floor((float)(new Fix64(owner.GetDistToOtherPlayer()) / new Fix64((int)launch.x)));
		int expectedOpponentHeight = (int)owner.otherPlayer.internalPos.y;
		int currVel = (int) owner.otherPlayer.velocity.y;
		int gravity = (int)owner.otherPlayer.gravity;
		for (int i = 0; i < framesToReachOpponent; i++)
		{
			expectedOpponentHeight += currVel;
			currVel += gravity;
		}
		

		int trajectoryY = framesToReachOpponent * (int)owner.velocity.y;
		if (trajectoryY > expectedOpponentHeight)
			owner.velocity.y -= 50;
		else
			owner.velocity.y += 20;
	}
}
