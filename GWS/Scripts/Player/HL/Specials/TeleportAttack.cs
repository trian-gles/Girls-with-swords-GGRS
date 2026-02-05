using Godot;
using System;

public class TeleportAttack : LaunchAttack
{


	[Export]
	public int teleFrame;
	private const string HatString = "Hat";

	public override void Enter()
	{
		base.Enter();
		owner.ScheduleEvent(EventScheduler.EventType.AUDIO, Name, Name);
	}
	public override void FrameAdvance()
	{

		if (frameCount == teleFrame)
		{
				((HL)owner).WarpToHat();

				owner.CommandHadouken(HatString, HadoukenPart.ProjectileCommand.DeleteHat);

				owner.grounded = false;
		}
		owner.CheckTurnAround();
		base.FrameAdvance();
	}
}
