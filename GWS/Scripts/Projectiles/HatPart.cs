using Godot;
using System;
using System.Collections.Generic;

public class HatPart : HadoukenPart
{

	public override string hadoukenType { get; } = "Hat";
	public Vector2 targetPos = Vector2.Zero;
	protected override void HurtPlayer(Vector2 collisionPnt)
	{
		Arrive();
		base.HurtPlayer(collisionPnt);
		
	}

	public override void ReceiveCommand(ProjectileCommand command)
	{
		if (command == ProjectileCommand.DeleteHat)
		{
			MakeInactive();
			Visible = false;
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
		if (movingRight && Position.x > targetPos.x)
			Arrive();
		
		if (!movingRight && Position.x < targetPos.x)
			Arrive();

		if (Position.x * 100 > Globals.rightWall || Position.x * 100 < Globals.leftWall)
			Arrive();

	}
}
