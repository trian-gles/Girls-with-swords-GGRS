using Godot;
using System;
using System.Collections.Generic;


public abstract class GrabStart : State
{
    public override void _Ready()
    {
        base._Ready();
		stop = false;
		isCounter = true;
		slowdownSpeed = 30;
	}

	public override void Enter()
    {
		base.Enter();
		CheckHit();
		
		//if ((Mathf.Abs(owner.internalPos.x - owner.otherPlayer.internalPos.x) < 2000) && owner.otherPlayer.IsGrabbable())
        //{
		//	EmitSignal(nameof(StateFinished), "Grab");
		//}

	}

    public override void AnimationFinished()
	{
		EmitSignal(nameof(StateFinished), "Idle");
	}

	public override void CheckHit()
	{
		Vector2 collisionPnt = owner.CheckHurtRectGrab();
		if (collisionPnt != Vector2.Inf && owner.otherPlayer.IsGrabbable())
		{
			EmitSignal(nameof(StateFinished), "Grab");
		}
	}


	public override void ReceiveHit(Globals.AttackDetails details)
	{
		ReceiveHitNoBlock(details);
	}
}
