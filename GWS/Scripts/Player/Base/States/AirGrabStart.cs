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
		ReceiveHitNoBlock(details);
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
