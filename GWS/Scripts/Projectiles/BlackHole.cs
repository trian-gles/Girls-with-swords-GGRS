using Godot;
using System;
using System.Collections.Generic;
using FixedMath.NET;

public class BlackHole : HadoukenPart
{
	private CPUParticles2D particles2D;

	[Export]
	private int startUp = 10;

	[Export]
	private int pullStrength = 5;

	[Export]
	private int slowTerminalVelocity = 250;

	protected GL createdByPlayer;

	public override string hadoukenType { get; } = "BlackHole";

	public override void _Ready()
	{
		base._Ready();
		particles2D = GetNode<CPUParticles2D>("CPUParticles2D");
		createdByPlayer = (GL)targetPlayer.otherPlayer;
	}
	public override void FrameAdvance() // wait till the turn after it was created to move the hadouken
	{

		if (active && frame > startUp)
		{
			

			if (frame > duration)
				MakeInactive();

			if (targetPlayer.grounded)
			{
				return;
			}

			int yToPlayer = (int)Math.Abs(Position.y * 100 - targetPlayer.internalPos.y);
			int xToPlayer = (int)Math.Abs(Position.x * 100 - targetPlayer.internalPos.x);

			bool playerBelow = (Position.y * 100 < targetPlayer.internalPos.y);
			bool playerLeft = (Position.x * 100 < targetPlayer.internalPos.x);

			if (createdByPlayer.PoweredBlackHoleFramesRemaining > 0)
			{
				particles2D.Color = Color.FromHsv((float)0.55, particles2D.Color.s, particles2D.Color.v);
			}
			else
			{
				particles2D.Color = Color.FromHsv((float)0.8, particles2D.Color.s, particles2D.Color.v);
			}

			if (createdByPlayer.PoweredBlackHoleFramesRemaining > 0 && !targetPlayer.grounded)
			{
				Fix64 pullForce = new Fix64(pullStrength * 30);

				var fixedXtoPlayer = new Fix64(xToPlayer);
				var fixedYtoPlayer = new Fix64(yToPlayer);
				var angle = Fix64.Tan(fixedYtoPlayer / fixedXtoPlayer);
				var xForce = pullForce * Fix64.Cos(angle);
				var yForce = pullForce * Fix64.Sin(angle);
				int xForceInt = (int)xForce;
				int yForceInt = (int)yForce;
				if (playerBelow)
					yForceInt *= -1;

				if (playerLeft)
					xForceInt *= -1;

				targetPlayer.velocity = new Vector2(xForceInt, yForceInt);

			}
			else
			{
				int distToPlayer = Globals.IntSqrt((int)(Math.Pow(xToPlayer, 2) + Math.Pow(yToPlayer, 2)));

				


				int adjustedPull = (int)Math.Floor((double)(pullStrength * 10000000 / distToPlayer));
				adjustedPull = Math.Min(adjustedPull, pullStrength * 6);



				Vector2 pushVec = new Vector2(adjustedPull, adjustedPull);
				if (distToPlayer < 20000000)
				{
					if (playerBelow)
						pushVec.y += targetPlayer.gravity;

					targetPlayer.velocity.x = (float)Math.Floor((double)targetPlayer.velocity.x * 2/ 3);
					targetPlayer.velocity.y = (float)Math.Floor((double)targetPlayer.velocity.y * 2 / 3);
				}

				if (adjustedPull > pullStrength && targetPlayer.currentState.tags.Contains("hitstate"))
				{
					//targetPlayer.currentState.stunRemaining += 1; this causes desyncs...
				}
					

				if (playerBelow) { pushVec.y *= -1; }

				if (playerLeft) { pushVec.x *= -1; }



				targetPlayer.velocity += pushVec;
			}


			if (CheckRect() && hits < totalHits)
			{
				//Globals.Log("Hurting player on frame " + frame);
				HurtPlayer();
				targetPlayer.terminalVelocity = slowTerminalVelocity;
				targetPlayer.counterStopFrames = 15;
				
			}
		}

		if (frame > duration + 12) // far past rollback limit
			targetPlayer.DeleteHadouken(this);
		frame++;
	}

	protected override void MakeInactive()
	{
		base.MakeInactive();
		particles2D.Emitting = false;
		createdByPlayer.BlackHolesTotal--;
		
	}

	public override void ReceiveCommand(ProjectileCommand command)
	{
		if (command == ProjectileCommand.BlackHoleDeactivate)
		{
			MakeInactive();
		}
	}

	public override void SetState(HadoukenState newState)
	{
		active = newState.active;
		GetNode<AnimatedSprite>("AnimatedSprite").Visible = active;
		frame = newState.frame;
		if (particles2D.Emitting != active)
			particles2D.Emitting = active;

		hits = newState.hits;
		lastHitFrame = newState.lastHitFrame;
	}
}
