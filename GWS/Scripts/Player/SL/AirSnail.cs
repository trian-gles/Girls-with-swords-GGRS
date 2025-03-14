using Godot;
using System;

public class AirSnail : Hadouken
{
	[Export]
	public Vector2 launch = new Vector2();

	[Export]
	public int launchFrame = 0;

	public override void _Ready()
	{
		base._Ready();
		tags.Add("aerial");
		slowdownSpeed = 0;
	}
	public override void Enter()
	{
		base.Enter();
		owner.ScheduleEvent(EventScheduler.EventType.AUDIO, "WarpSpawn", Name);
	}
	public override void FrameAdvance()
	{
		base.FrameAdvance();
		if (frameCount == launchFrame)
		{
			
			owner.velocity = launch;
			if (!owner.facingRight)
			{
				//GD.Print("Flipping launch x coor");
				owner.velocity.x *= -1;
			}
			owner.grounded = false;
		}
		else if (frameCount > launchFrame)
		{
			ApplyGravity();
			if (owner.grounded)
			{
				owner.velocity.x = 0;
			}
		}
	}


	public override void AnimationFinished()
	{
		base.AnimationFinished();
	}
}
