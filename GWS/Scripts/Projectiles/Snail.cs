using Godot;
using System;
using System.Collections.Generic;
class Snail : HadoukenPart
{
	[Export]
	public int gravity;

	[Export]
	public Vector2 jumpVel = new Vector2(4, 10);

	public override string hadoukenType { get; } = "Snail";

	private bool overhead = false;

	private AnimatedSprite sprite;

	private enum SnailMode
	{
		GetInPosition,
		Standby,
		Attack,
		JumpAttack
	}

	private SnailMode mode = SnailMode.GetInPosition;

	public override void _Ready()
	{
		base._Ready();
		sprite = GetNode<AnimatedSprite>("AnimatedSprite");
	}

	public override void Spawn(bool movingRight, Player targetPlayer)
	{
		base.Spawn(movingRight, targetPlayer);
		mode = SnailMode.GetInPosition;
		Position = new Vector2(Position.x, 269);
		

	}
	public override void FrameAdvance()
	{
		base.FrameAdvance();
		switch (mode)
		{
			case SnailMode.GetInPosition:
				GetInPositionUpdate();
				break;
			case SnailMode.Standby:
				StandbyUpdate();
				break;
			case SnailMode.Attack:
				AttackUpdate();
				break;
			case SnailMode.JumpAttack:
				JumpAttackUpdate();
				break;
		}
	}

	/// <summary>
	/// Note that directions are flipped!
	/// </summary>
	private void GetInPositionUpdate()
	{
		if (!movingRight)
		{
			Position = new Vector2(Position.x + 2, Position.y);
			if (Position.x * 100 > Globals.rightWall - 1000)
				mode = SnailMode.Standby;
		}
		else
		{
			Position = new Vector2(Position.x - 2, Position.y);
			if (Position.x * 100 < Globals.leftWall + 1000)
				mode = SnailMode.Standby;
		}
	}

	protected override void HurtPlayer()
	{
		if (mode == SnailMode.Attack || mode == SnailMode.JumpAttack)
			base.HurtPlayer();
	}

	private void StandbyUpdate()
	{

	}

	private void AttackUpdate()
	{
		if (movingRight)
		{
			Position = new Vector2(Position.x + 4, Position.y);
		}
		else
		{
			Position = new Vector2(Position.x - 4, Position.y);
		}
	}

	private void JumpAttackUpdate()
	{
		int xMove = (int)jumpVel.x;

		if (!movingRight)
		{
			xMove *= -1;
		}

		Position = new Vector2(Position.x + xMove, Position.y);

		if (frame % 2 == 0)
			speed.y += gravity;

		sprite.Rotation = (float)Math.Atan2(speed.y, xMove) + (float)Math.PI;
		if (speed.y > 0)
			overhead = true;
	}

	protected override Dictionary<string, int> GetStateSpecific()
	{
		return new Dictionary<string, int>() { 
			{ "mode", (int) mode},


			{"overhead", Globals.BoolToInt(overhead)}
		};
	}

	protected override void SetStateSpecific(Dictionary<string, int> dict)
	{
		mode = (SnailMode)dict["mode"];
		overhead = Globals.IntToBool(dict["overhead"]);
	}

	public override void ReceiveCommand(ProjectileCommand command)
	{
		if (mode == SnailMode.Standby && command == ProjectileCommand.SnailAttack)
			mode = SnailMode.Attack;
		if (mode == SnailMode.Attack && command == ProjectileCommand.SnailJump)
		{
			speed.y = jumpVel.y;
			mode = SnailMode.JumpAttack;
		}
			

	}

}
