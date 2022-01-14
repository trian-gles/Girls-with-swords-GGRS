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
	protected int dmg = 1;

	[Export]
	protected bool knockdown = false;

	[Export]
	protected int prorationLevel = 0;

	[Signal]
	public delegate void OnHitConnected(int hitPush);

	

	
	public override void _Ready()
	{
		base._Ready();
		Connect("OnHitConnected", owner, nameof(owner.OnHitConnected));
	}

	protected void AddJumpCancel()
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

	public override void InHurtbox()
	{
		if (!hitConnect)
		{
			//GD.Print($"Hit connect on frame {frameCount}");
			EmitSignal(nameof(OnHitConnected), hitPush);
			owner.otherPlayer.ReceiveHit(owner.OtherPlayerOnRight(), dmg, blockStun, hitStun, height, hitPush, opponentLaunch, knockdown, prorationLevel);
			hitConnect = true;
		}

	}

	public override void HandleInput(char[] inputArr)
	{
		if (!hitConnect)
		{
			return;
		}
		base.HandleInput(inputArr);
	}


	public override void ReceiveHit(bool rightAttack, HEIGHT height, int hitPush, Vector2 launch, bool knockdown)
	{
		if (!rightAttack)
		{
			launch.x *= -1;
			hitPush *= -1;
		}

		owner.hitPushRemaining = hitPush;

		if (owner.velocity.y < 0)
		{
			owner.grounded = false;
		}

		EnterHitState(knockdown, launch);
	}

	

	protected override void EnterHitState(bool knockdown, Vector2 launch)
	{
		bool launchBool = false;

		if (!(launch == Vector2.Zero)) // LAUNCH NEEDS MORE WORK
		{
			//GD.Print("Launch is not zero!");
			owner.velocity = launch;
			launchBool = true;
		}

		bool airState = (launchBool || !owner.grounded);

		if (launchBool && !knockdown)
		{
			GD.Print("Entering counterfloat from attack");
			EmitSignal(nameof(StateFinished), "CounterFloat");
		}
		else if (airState && knockdown)
		{
			GD.Print("Entering airknockdown from attack");
			EmitSignal(nameof(StateFinished), "AirKnockdown");
		}
		else if (!airState && knockdown)
		{
			GD.Print("Entering knockdown from attack");
			EmitSignal(nameof(StateFinished), "Knockdown");
			
		}
		else
		{
			EmitSignal(nameof(StateFinished), "CounterHit");
		}
	}


}
