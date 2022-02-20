using Godot;
using System;
using System.Collections.Generic;


public abstract class BaseAttack : State
{
	[Export]
	protected int hitStun = 10;

	[Export]
	protected int blockStun = 11;

	[Export]
	protected Vector2 opponentLaunch = new Vector2();

	[Export]
	protected int hitPush = 0;

	[Export]
	protected HEIGHT height = HEIGHT.MID;

	[Export]
	protected EXTRAEFFECT effect = EXTRAEFFECT.NONE;

	[Export]
	protected int dmg = 1;

	[Export]
	protected bool knockdown = false;

	[Export]
	protected int prorationLevel = 0;

	[Signal]
	public delegate void OnHitConnected(int hitPush);

	public enum EXTRAEFFECT
	{
		NONE,
		GROUNDBOUNCE,
		WALLBOUNCE
	}

	
	public enum ATTACKDIR
	{
		RIGHT,
		LEFT,
		EQUAL
	}
	
	public override void _Ready()
	{
		base._Ready();
		stop = false;
		slowdownSpeed = 30;
		Connect("OnHitConnected", owner, nameof(owner.OnHitConnected));
	}

	protected virtual void AddJumpCancel()
	{
		AddGatling(new char[] { '8', 'p' }, () => owner.CheckHeldKey('6'), "Jump", () => owner.velocity.x = owner.speed);
		AddGatling(new char[] { '8', 'p' }, () => owner.CheckHeldKey('4'), "Jump", () => owner.velocity.x = -owner.speed);
		AddGatling(new char[] { '8', 'p' }, "Jump");
	}
	public override void Enter()
	{
		base.Enter();
		hitConnect = false;
		owner.ScheduleEvent(EventScheduler.EventType.AUDIO, "Whiff", Name);
	}
	public override void AnimationFinished()
	{
		EmitSignal(nameof(StateFinished), "Idle");
	}

	public override void FrameAdvance()
	{
		base.FrameAdvance();
		if (!hitConnect)
		{
			Vector2 collisionPnt = owner.CheckHurtRect();
			if (collisionPnt != Vector2.Inf)
			{
				InHurtbox(collisionPnt);
			}

		}
		
	}

	public override void InHurtbox(Vector2 collisionPnt)
	{
		//GD.Print($"Hit connect at point {collisionPnt}");
		EmitSignal(nameof(OnHitConnected), hitPush);
		var direction = ATTACKDIR.EQUAL;

		if (owner.OtherPlayerOnRight())
		{
			direction = ATTACKDIR.RIGHT;
		}
		else if(owner.OtherPlayerOnLeft())
		{
			direction = ATTACKDIR.LEFT;
		}

		owner.otherPlayer.ReceiveHit(collisionPnt, direction, dmg, blockStun, hitStun, height, hitPush, opponentLaunch, knockdown, prorationLevel, effect);
		hitConnect = true;
	}

	public override void HandleInput(char[] inputArr)
	{
		if (!hitConnect)
		{
			return;
		}
		base.HandleInput(inputArr);
	}


	public override void ReceiveHit(BaseAttack.ATTACKDIR attackDir, HEIGHT height, int hitPush, Vector2 launch, bool knockdown, Vector2 collisionPnt, BaseAttack.EXTRAEFFECT effect)
	{
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

		if (owner.velocity.y < 0)
		{
			owner.grounded = false;
		}

		EnterHitState(knockdown, launch, collisionPnt, effect);
	}

	

	protected override void EnterHitState(bool knockdown, Vector2 launch, Vector2 collisionPnt, BaseAttack.EXTRAEFFECT effect)
	{
		GetNode<Node>("/root/Globals").EmitSignal(nameof(PlayerFXEmitted), collisionPnt, "hit", false);
		bool launchBool = false;
		owner.ComboUp();
		if (!(launch == Vector2.Zero)) // LAUNCH NEEDS MORE WORK
		{
			//GD.Print("Launch is not zero!");
			owner.velocity = launch;
			launchBool = true;
		}

		bool airState = (launchBool || !owner.grounded);

		if (launchBool && !knockdown)
		{
			//GD.Print("Entering counterfloat from attack");
			EmitSignal(nameof(StateFinished), "CounterFloat");
		}
		else if (airState && knockdown)
		{
			//GD.Print("Entering airknockdown from attack");
			EmitSignal(nameof(StateFinished), "AirKnockdown");
		}
		else if (!airState && knockdown)
		{
			//GD.Print("Entering knockdown from attack");
			EmitSignal(nameof(StateFinished), "Knockdown");
			
		}
		else
		{
			EmitSignal(nameof(StateFinished), "CounterHit");
		}
	}


}
