using Godot;
using System;
using System.Collections.Generic;

public class BlackHole : HadoukenPart
{
	private CPUParticles2D particles2D;

	[Export]
	private int startUp = 10;

	[Export]
	private int pullStrength = 5;

	[Export]
	private int slowTerminalVelocity = 300;

	public override void _Ready()
	{
		base._Ready();
		particles2D = GetNode<CPUParticles2D>("CPUParticles2D");
	}
	public override void FrameAdvance() // wait till the turn after it was created to move the hadouken
	{

		if (active && frame > startUp)
		{
			if (frame > duration)
				MakeInactive();

			int yToPlayer = (int)Math.Abs(Position.y * 100 - targetPlayer.internalPos.y);
			int xToPlayer = (int)Math.Abs(Position.x * 100 - targetPlayer.internalPos.x);

			int distToPlayer = Globals.IntSqrt((int)(Math.Pow(xToPlayer, 2) + Math.Pow(yToPlayer, 2)));

			bool playerBelow = (Position.y * 100 < targetPlayer.internalPos.y);
			bool playerLeft = (Position.x * 100 < targetPlayer.internalPos.x);


			int adjustedPull = (int)Math.Floor((double)(pullStrength * 10000000 / distToPlayer));
			adjustedPull = Math.Min(adjustedPull, pullStrength * 6);

			Vector2 pushVec = new Vector2(adjustedPull, adjustedPull);
			if (targetPlayer.grounded) {
				pushVec.y *= 0;
			}

			if (adjustedPull > pullStrength)
				targetPlayer.currentState.stunRemaining += 1;

			if (playerBelow) { pushVec.y *= -1; }

			if (playerLeft) { pushVec.x *= -1; }

			targetPlayer.velocity += pushVec;
			
			//GD.Print("Setting velocity to " + targetPlayer.velocity + " on frame " + frame);


			if (CheckRect())
			{
				HurtPlayer();
				targetPlayer.terminalVelocity = slowTerminalVelocity;
				Globals.Log("Hurting player on frame " + frame);
			}
		}

		if (frame > duration + 12) // far past rollback limit
			targetPlayer.DeleteHadouken(this);
		frame++;
		//GD.Print(frame);
	}

	protected override void MakeInactive()
	{
		base.MakeInactive();
		particles2D.Emitting = false;
		
	}

	public override void SetState(HadoukenState newState)
	{
		active = newState.active;
		GetNode<AnimatedSprite>("AnimatedSprite").Visible = active;
		frame = newState.frame;
		if (particles2D.Emitting != active)
			particles2D.Emitting = active;
	}
}
