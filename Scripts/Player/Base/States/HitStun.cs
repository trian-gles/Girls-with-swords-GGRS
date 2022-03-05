using Godot;
using System;

public class HitStun : HitState
{
	public override void _Ready()
	{
		base._Ready();
		loop = false;
	}
	public override void Enter()
	{
		base.Enter();
		
		owner.ScheduleEvent(EventScheduler.EventType.AUDIO, "HitStun", Name);
		owner.GFXEvent("Blood");
	}

	public override void AnimationFinished()
	{
		//GD.Print("Animation for HitStun finished");
	}


	public override void FrameAdvance()
	{
		base.FrameAdvance();
		stunRemaining--;

		if (stunRemaining == 0)
		{
			EmitSignal(nameof(StateFinished), "Idle");
		}

	}

	public override void PushMovement(float _xVel)
	{
		if (owner.grounded)
		{
			base.PushMovement(_xVel);
		}
	}
	
	public override void ReceiveHit(BaseAttack.ATTACKDIR attackDir, HEIGHT height, int hitPush, Vector2 launch, bool knockdown, Vector2 collisionPnt, BaseAttack.EXTRAEFFECT effect)
	{
		//GD.Print($"Received attack on side {rightAttack}");
		bool launchBool = false;
		switch (attackDir)
		{
			case BaseAttack.ATTACKDIR.RIGHT:
				break;
			case BaseAttack.ATTACKDIR.LEFT:
				launch.x *= -1;
				hitPush *= -1;
				break;
			case BaseAttack.ATTACKDIR.EQUAL:
				launch.x = 0;
				hitPush = 0;
				break;
		}
		owner.hitPushRemaining = hitPush;
		//GD.Print($"Setting hitpush in hitstun to {owner.hitPushRemaining}");
		owner.velocity = launch;
		if (!(launch == Vector2.Zero))
		{
			owner.velocity = launch;
			launchBool = true;
		}

		if (owner.velocity.y < 0) // make sure the player is registered as in the air if launched 
		{
			owner.grounded = false;
		}
		
		
		EnterHitState(knockdown, launch, collisionPnt, effect);

	}
}

