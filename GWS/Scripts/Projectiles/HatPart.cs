using Godot;
using System;
using System.Collections.Generic;

public class HatPart : HadoukenPart
{

	public override string hadoukenType { get; } = "Hat";
	protected override void HurtPlayer()
	{
		base.HurtPlayer();
		Arrive();
	}

	public override void ReceiveCommand(ProjectileCommand command)
	{
		if (command == ProjectileCommand.DeleteHat)
		{
			MakeInactive();
			GetNode<AnimatedSprite>("AnimatedSprite").Visible = false;
		}

		else if (command == ProjectileCommand.StopHat)
		{
			Arrive();
			
		}
	}

	private void Arrive()
	{
		if (!active)
			return;
		speed = new Vector2(0, 0);
		((HL)targetPlayer.otherPlayer).hatCoors = Position;
		MakeInactive();
	}

	protected override Dictionary<string, int> GetStateSpecific()
	{
		return new Dictionary<string, int>() {
			{ "speedx", (int) speed.x},
			{"speedy", (int) speed.y}
		};
	}

	protected override void SetStateSpecific(Dictionary<string, int> dict)
	{
		speed.x = dict["speedx"];
		speed.y = dict["speedy"];
	}

	public override void FrameAdvance()
	{
		base.FrameAdvance();
		if (Position.x * 100 > Globals.rightWall || Position.x * 100 < Globals.leftWall)
			Arrive();

	}
}
