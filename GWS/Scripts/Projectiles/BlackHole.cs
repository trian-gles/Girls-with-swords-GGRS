using Godot;
using System;
using FixedMath.NET;

public class BlackHole : HadoukenPart
{
	

	[Export]
	private int startUp = 10;

	[Export]
	private int pullStrength = 5;

	public override string GetType()
	{
		return "BlackHole";
	}

	public override void Spawn(bool movingRight, Player targetPlayer)
	{
		var particles2D = GetNode<CPUParticles2D>("CPUParticles2D");
		particles2D.Emitting = true;
		particles2D.Color = Color.FromHsv((float)0.8, particles2D.Color.s, particles2D.Color.v);
		base.Spawn(movingRight, targetPlayer);
	}
	public override void FrameAdvance() // wait till the turn after it was created to move the hadouken
	{
		if (frame > duration + 12) // far past rollback limit
		{
			targetPlayer.DeleteHadouken(this);
		}
		if (frame > duration)
			MakeInactive();
		frame++;

		if (active && frame > startUp)
		{

			if (targetPlayer.grounded || targetPlayer.currentState.tags.Contains(Globals.Tags.tech))
			{
				return;
			}

			int yToPlayer = (int)Math.Abs(Position.y * 100 - targetPlayer.internalPos.y);
			int xToPlayer = (int)Math.Abs(Position.x * 100 - targetPlayer.internalPos.x);

			bool playerBelow = (Position.y * 100 < targetPlayer.internalPos.y);
			bool playerLeft = (Position.x * 100 < targetPlayer.internalPos.x);


			

			int distToPlayer = Globals.IntSqrt((int)(Math.Pow(xToPlayer, 2) + Math.Pow(yToPlayer, 2)));




			int adjustedPull = (int)Math.Floor((double)(pullStrength * 10000000 / distToPlayer));
			adjustedPull = Math.Min(adjustedPull, pullStrength * 6);



			Vector2 pushVec = new Vector2(adjustedPull, adjustedPull);
			if (distToPlayer < 20000000)
			{
				if (playerBelow)
					pushVec.y += targetPlayer.gravity;

				targetPlayer.velocity.x = (float)Math.Floor((double)targetPlayer.velocity.x * 2 / 3);
				targetPlayer.velocity.y = (float)Math.Floor((double)targetPlayer.velocity.y * 2 / 3);
			}


			if (playerBelow) { pushVec.y *= -1; }

			if (playerLeft) { pushVec.x *= -1; }



			targetPlayer.velocity += pushVec;

			Vector2 collisionPnt = CheckRect();
			if (collisionPnt != Vector2.Inf && hits < totalHits && !targetPlayer.currentState.IsProjectileInvuln())
			{
				targetPlayer.terminalVelocity = slowTerminalVelocity;
				HurtPlayer(collisionPnt);
				
				targetPlayer.counterStopFrames = 15;

			}
		}

		
	}

	protected override void MakeInactive()
	{
		base.MakeInactive();
		GetNode<CPUParticles2D>("CPUParticles2D").Emitting = false;
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
		base.SetState(newState);
		if (GetNode<CPUParticles2D>("CPUParticles2D").Emitting != active)
			GetNode<CPUParticles2D>("CPUParticles2D").Emitting = active;
	}
}
