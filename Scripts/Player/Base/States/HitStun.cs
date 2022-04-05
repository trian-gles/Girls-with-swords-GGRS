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
	
	public override void ReceiveHit(Globals.AttackDetails details)
	{
		//GD.Print($"Received attack on side {rightAttack}");
		bool launchBool = false;
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
		//GD.Print($"Setting hitpush in hitstun to {owner.hitPushRemaining}");
		owner.velocity = details.opponentLaunch;
		if (!(details.opponentLaunch == Vector2.Zero))
		{
			owner.velocity = details.opponentLaunch;
			launchBool = true;
		}

		if (owner.velocity.y < 0) // make sure the player is registered as in the air if launched 
		{
			owner.grounded = false;
		}
		
		
		EnterHitState(details.knockdown, details.opponentLaunch, details.collisionPnt, details.effect);

	}
}

