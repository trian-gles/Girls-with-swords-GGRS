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

	private SL snailOwner;

	private enum SnailMode
	{
		GetInPosition,
		Standby,
		Attack,
		AttackWillJump,
		JumpAttack,
		Attack2
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
		
		snailOwner = (SL)targetPlayer.otherPlayer;

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
			case SnailMode.AttackWillJump:
				AttackWillJumpUpdate();
				break;
			case SnailMode.Attack2:
				Attack2Update();
				break;
		}
	}

	private void EnterAttack2()
	{
		GD.Print("Entering second attack");
		mode = SnailMode.Attack2;
		active = true;
		var animSprite = GetNode<AnimatedSprite>("AnimatedSprite");
		animSprite.FlipH = !animSprite.FlipH;
		hits = 0;
	}

	private void EnterStandby()
	{
		mode = SnailMode.Standby;
	}
	/// <summary>
	/// Note that directions are flipped!
	/// </summary>
	private void GetInPositionUpdate()
	{
		
		ApplyGravity();
		if (!movingRight)
		{
			Position = new Vector2(Position.x + 4, Math.Min(Position.y, 245));
			if (Position.x * 100 > Globals.rightWall - 1000)
				EnterStandby();
		}
		else
		{
			Position = new Vector2(Position.x - 4, Math.Min(Position.y, 245));
			if (Position.x * 100 < Globals.leftWall + 1000)
				EnterStandby();
		}
		
	}

	protected override void HurtPlayer()
	{
		if (mode == SnailMode.Attack || mode == SnailMode.JumpAttack || mode == SnailMode.Attack2)
			base.HurtPlayer();
	}

	private void StandbyUpdate()
	{
		ApplyGravity();
		Position = new Vector2(Position.x, Math.Min(Position.y, 245));
	}

	private void AttackUpdate()
	{
		if (movingRight)
		{
			Position = new Vector2(Position.x + 4, Position.y);
			if (Position.x * 100 > Globals.rightWall - 1000)
				EnterAttack2();
		}
		else
		{
			Position = new Vector2(Position.x - 4, Position.y);
			if (Position.x * 100 < Globals.leftWall + 1000)
				EnterAttack2();
		}
	}

	private void Attack2Update()
	{
		if (movingRight)
		{
			Position = new Vector2(Position.x - 4, Position.y);
		}
		else
		{
			Position = new Vector2(Position.x + 4, Position.y);
		}
	}

	private void AttackWillJumpUpdate()
	{
		if (movingRight)
		{
			Position = new Vector2(Position.x + 4, Position.y);
		}
		else
		{
			Position = new Vector2(Position.x - 4, Position.y);
		}
		if (Math.Abs(targetPlayer.internalPos.x / 100 - Position.x) < 65)
			Jump();
	}

	private void ApplyGravity()
	{
		if (frame % 2 == 0)
			speed.y += gravity;
	}

	private void Jump()
	{
		speed.y = jumpVel.y;
		mode = SnailMode.JumpAttack;
	}

	private void JumpAttackUpdate()
	{
		int xMove = (int)jumpVel.x;

		if (!movingRight)
		{
			xMove *= -1;
		}

		Position = new Vector2(Position.x + xMove, Position.y);

		ApplyGravity();

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
		{
			speed.y = 0;
			mode = SnailMode.Attack;

		}
			
		else if (mode == SnailMode.Standby && command == ProjectileCommand.SnailJump)
		{
			speed.y = 0;
			mode = SnailMode.AttackWillJump;
		}
		else if (command == ProjectileCommand.SnailRide)
		{
			TryRide();
			
		}
			

	}

	private void TryRide()
	{
		Rect2 myRect = GetRect(GetNode<CollisionShape2D>("CollisionShape2D"), true);
		List<Rect2> otherRects = snailOwner.GetRects(targetPlayer.hitBoxes, true);
		foreach (Rect2 pRect in otherRects)
		{
			if (myRect.Intersects(pRect))
			{
				snailOwner.SnailRide();
				MakeInactive();
			}
		}
	}

}
