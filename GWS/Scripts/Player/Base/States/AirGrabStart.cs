using Godot;
using System;
using System.Collections.Generic;


public abstract class AirGrabStart : AirState
{
	public override void AnimationFinished()
	{
		EmitSignal(nameof(StateFinished), "Fall");
	}

	public override void CheckHit()
	{
		Vector2 collisionPnt = owner.CheckHurtRectGrab();
		if (collisionPnt != Vector2.Inf && owner.otherPlayer.IsAirGrabbable())
		{
			EmitSignal(nameof(StateFinished), "AirGrab");
		}
	}


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

		owner.hitPushRemaining = details.hitPush;

		if (owner.velocity.y < 0)
		{
			owner.grounded = false;
		}

		EnterHitState(details.knockdown, details.opponentLaunch, details.collisionPnt, details.effect);
	}

    public override void FrameAdvance()
    {
        base.FrameAdvance();
		if (owner.grounded && frameCount > 1)
		{
			EmitSignal(nameof(StateFinished), "Landing");
		}
		ApplyGravity();
	}
}
