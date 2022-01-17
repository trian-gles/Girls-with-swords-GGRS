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

	
	public enum ATTACKDIR
    {
		RIGHT,
		LEFT,
		EQUAL
    }
	
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
			var direction = ATTACKDIR.EQUAL;

			if (owner.OtherPlayerOnRight())
            {
				direction = ATTACKDIR.RIGHT;
            }
			else if(owner.OtherPlayerOnLeft())
			{
				direction = ATTACKDIR.LEFT;
			}

			owner.otherPlayer.ReceiveHit(direction, dmg, blockStun, hitStun, height, hitPush, opponentLaunch, knockdown, prorationLevel);
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


	public override void ReceiveHit(BaseAttack.ATTACKDIR attackDir, HEIGHT height, int hitPush, Vector2 launch, bool knockdown)
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
