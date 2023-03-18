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

			bool playerBelow = (Position.y * 100 < targetPlayer.internalPos.y);
			bool playerLeft = (Position.x * 100 < targetPlayer.internalPos.x);

			

			Vector2 pushVec = new Vector2(pullStrength, pullStrength);
			if (targetPlayer.grounded) {
				pushVec.y *= 0;
			}

			if (playerBelow) { pushVec.y *= -1; }

			if (playerLeft) { pushVec.x *= -1; }

			targetPlayer.velocity += pushVec;
			
			//GD.Print("Setting velocity to " + targetPlayer.velocity + " on frame " + frame);


			if (CheckRect())
			{
				HurtPlayer();
				GD.Print("Hurting player on frame " + frame);
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
