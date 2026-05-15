using Godot;
using System;
using System.Collections.Generic;

public class HatPart : HadoukenPart
{
	[Export]
	public Vector2 startSpeed;

	private const string HatHadoukenTypeString = "Hat";
	public Vector2 targetPos = Vector2.Zero;

	public override string GetType()
	{
		return HatHadoukenTypeString;
	}
	public override void Spawn(bool movingRight, Player targetPlayer)
	{
		base.Spawn(movingRight, targetPlayer);
		speed = startSpeed;
	}
	protected override void HurtPlayer(Vector2 collisionPnt)
	{
		base.HurtPlayer(collisionPnt);
		speed = new Vector2(0, 0);
		((HL)targetPlayer.otherPlayer).hatCoors = Position;
	}

	public override void ReceiveCommand(ProjectileCommand command)
	{
		if (command == ProjectileCommand.DeleteHat)
		{
			MakeInactive();
			Visible = false;
			speed.y = 4;
		}
		else if (command == ProjectileCommand.MoveHatRight)
			Right();
		else if (command == ProjectileCommand.MoveHatLeft)
			Left();
		else if (command == ProjectileCommand.StopHat)
			Arrive();
	}

	private void Right()
	{
		Position += Vector2.Right * 2;
		((HL)targetPlayer.otherPlayer).hatCoors = Position;
	}
	
	private void Left()
	{
		Position += Vector2.Left * 2;
		((HL)targetPlayer.otherPlayer).hatCoors = Position;
	}

	private void Arrive()
	{
		if (!active)
			return;
		speed = new Vector2(0, 0);
		((HL)targetPlayer.otherPlayer).hatCoors = Position;
		MakeInactive();
	}

	private int SPEEDXINDEX = 0;
	private int SPEEDYINDEX = 1;
	protected override int[] GetStateSpecific()
	{
		specificState[SPEEDXINDEX] = (int)speed.x;
		specificState[SPEEDYINDEX] = (int)speed.y;

		return base.GetStateSpecific();
	}

	protected override void SetStateSpecific(int[] dict)
	{
		speed.x = dict[SPEEDXINDEX];
		speed.y = dict[SPEEDYINDEX];
	}

	public override void FrameAdvance()
	{
		base.FrameAdvance();
		if (active && speed.x != 0)
			((HL)targetPlayer.otherPlayer).hatCoors = Position;

		if (movingRight && Position.x > targetPos.x)
			Arrive();
		
		if (!movingRight && Position.x < targetPos.x)
			Arrive();

		if (Position.x * 100 > Globals.rightWall || Position.x * 100 < Globals.leftWall)
			Arrive();
	}
}
