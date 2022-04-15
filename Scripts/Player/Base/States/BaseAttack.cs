using Godot;
using System;
using System.Collections.Generic;


public abstract class BaseAttack : State
{
	[Export]
	protected int level = 0;

	protected Globals.AttackDetails hitDetails;
	protected Globals.AttackDetails chDetails;


	[Export]
	protected int modifiedHitStun = 0;

	[Export]
	protected int modifiedCounterHitStun = 0;

	[Export]
	protected Vector2 opponentLaunch = Vector2.Zero;

	[Export]
	protected Vector2 chLaunch = Vector2.Zero;

	[Export]
	protected int modifiedHitPush = 0;

	[Export]
	protected int hitPush = 0;

	[Export]
	protected HEIGHT height = HEIGHT.MID;

	[Export]
	protected EXTRAEFFECT effect = EXTRAEFFECT.NONE;

	[Export]
	protected EXTRAEFFECT chEffect = EXTRAEFFECT.NONE;

	[Export]
	protected bool knockdown = false;

	[Signal]
	public delegate void OnHitConnected(int hitPush);

	public enum EXTRAEFFECT
	{
		NONE,
		GROUNDBOUNCE,
		WALLBOUNCE,
		STAGGER
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
		isCounter = true;
		slowdownSpeed = 30;
		Connect("OnHitConnected", owner, nameof(owner.OnHitConnected));
		hitDetails = Globals.attackLevels[level].hit;
		chDetails = Globals.attackLevels[level].counterHit;

		hitDetails.opponentLaunch = opponentLaunch;
		if (chLaunch != Vector2.Zero)
			chDetails.opponentLaunch = chLaunch;
		else
			chDetails.opponentLaunch = opponentLaunch;

		hitDetails.effect = effect;
		chDetails.effect = chEffect;
		hitDetails.knockdown = knockdown;
		chDetails.knockdown = knockdown;
		hitDetails.height = height;
		chDetails.height = height;

		if (modifiedHitStun != 0)
			hitDetails.hitStun = modifiedHitStun;
		GD.Print($"{Name} modified hitstun is {modifiedHitStun}");
		if (modifiedCounterHitStun != 0)
			chDetails.hitStun = modifiedCounterHitStun;

		if (modifiedHitPush != 0)
		{
			hitDetails.hitPush = modifiedHitPush;
			chDetails.hitPush = modifiedHitPush;

		}
			
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

	public override void CheckHit()
	{
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
		EmitSignal(nameof(OnHitConnected), hitDetails.hitPush);
		var direction = ATTACKDIR.EQUAL;

		if (owner.OtherPlayerOnRight())
		{
			direction = ATTACKDIR.RIGHT;
		}
		else if(owner.OtherPlayerOnLeft())
		{
			direction = ATTACKDIR.LEFT;
		}
		
		hitDetails.dir = direction;
		chDetails.dir = direction;
		hitDetails.collisionPnt = collisionPnt;
		chDetails.collisionPnt = collisionPnt;
		owner.otherPlayer.ReceiveHit(hitDetails, chDetails);
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

	

	//protected override void EnterHitState(bool knockdown, Vector2 launch, Vector2 collisionPnt, BaseAttack.EXTRAEFFECT effect)
	//{
	//	GetNode<Node>("/root/Globals").EmitSignal(nameof(PlayerFXEmitted), collisionPnt, "hit", false);
	//	bool launchBool = false;
	//	owner.ComboUp();
	//	if (!(launch == Vector2.Zero)) // LAUNCH NEEDS MORE WORK
	//	{
	//		GD.Print("Launch is not zero!");
	//		owner.velocity = launch;
	//		launchBool = true;
	//	}

	//	bool airState = (launchBool || !owner.grounded);

	//	if (effect == BaseAttack.EXTRAEFFECT.GROUNDBOUNCE)
	//	{
	//		EmitSignal(nameof(StateFinished), "GroundBounce");
	//	}
	//	else if (effect == BaseAttack.EXTRAEFFECT.WALLBOUNCE)
	//	{
	//		EmitSignal(nameof(StateFinished), "WallBounce");
	//	}
	//	else if (launchBool && !knockdown)
	//	{
	//		GD.Print("Entering counterfloat from attack");
	//		EmitSignal(nameof(StateFinished), "CounterFloat");
	//	}
	//	else if (airState && knockdown)
	//	{
	//		GD.Print("Entering airknockdown from attack");
	//		EmitSignal(nameof(StateFinished), "AirKnockdown");
	//	}
	//	else if (!airState && knockdown)
	//	{
	//		GD.Print("Entering knockdown from attack");
	//		EmitSignal(nameof(StateFinished), "Knockdown");
			
	//	}
	//	else
	//	{
	//		EmitSignal(nameof(StateFinished), "CounterHit");
	//	}
	//}


}
