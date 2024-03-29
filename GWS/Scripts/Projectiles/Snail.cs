using Godot;
using System;
using System.Collections.Generic;
public class Snail : HadoukenPart
{
	[Export]
	public int gravity;

	[Export]
	public Vector2 jumpVel = new Vector2(4, 10);

	[Export]
	public int turnAroundGap = 21;

	[Export]
	public int startup = 10;

	[Signal]
	public delegate void SnailUpdate(int x, Color color);

	public override string hadoukenType { get; } = "Snail";

	private bool overhead = false;
	private int activateFrame = 0;


	private int hitConnectFrame = 0;

	private AnimatedSprite sprite;

	private SL snailOwner;

	private Color setupColor = new Color(0, 0, 255);
	private Color readyColor = new Color(0, 255, 0);
	private Color attackColor = new Color(255, 0, 0);

	private enum SnailMode
	{
		GetInPosition,
		Standby,
		Attack,
		AttackWillJump,
		JumpAttack,
		Attack2,
		Inactive,
		TurnAround
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

		if (snailOwner.rightCornerSnail && snailOwner.leftCornerSnail)
			Destroy();
		else if ((movingRight && snailOwner.leftCornerSnail) || (!movingRight && snailOwner.rightCornerSnail))
		{
			this.movingRight = !movingRight;
		}
			

		if (this.movingRight)
			snailOwner.leftCornerSnail = true;
		else
			snailOwner.rightCornerSnail = true;

		GetNode<AnimatedSprite>("AnimatedSprite").FlipH = this.movingRight;
	}

	private void Destroy()
	{
		GetNode<AnimatedSprite>("AnimatedSprite").Visible = false;
		if (mode == SnailMode.Standby)
			ExitStandby();

		mode = SnailMode.Inactive;
		MakeInactive();
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
			case SnailMode.Inactive:
				InactiveUpdate();
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
			case SnailMode.TurnAround:
				TurnAroundUpdate();
				break;
		}
	}

	public override void AlwaysUpdate()
	{
		base.AlwaysUpdate();
		switch (mode)
		{
			case SnailMode.GetInPosition:
				EmitSignal("SnailUpdate", snailOwner.Name, Position.x, setupColor);
				break;
			case SnailMode.Standby:
				EmitSignal("SnailUpdate", snailOwner.Name, Position.x, readyColor);
				break;
			case SnailMode.Inactive:
				break;
			default:
				EmitSignal("SnailUpdate", snailOwner.Name, Position.x, attackColor);
				break;
		}
	}

	private void EnterAttack2()
	{
		
		mode = SnailMode.Attack2;
		active = true;
		var animSprite = GetNode<AnimatedSprite>("AnimatedSprite");
		animSprite.FlipH = !animSprite.FlipH;
		hits = 0;
		hitDetails.hitPush = - Math.Abs(hitDetails.hitPush);
		chDetails.hitPush = -Math.Abs(chDetails.hitPush);
	}

	private void EnterStandby()
	{
		mode = SnailMode.Standby;
		GetNode<AnimatedSprite>("AnimatedSprite").FlipH = !movingRight;
	}

	private void ExitStandby()
	{
		if (movingRight)
			snailOwner.leftCornerSnail = false;
		else
			snailOwner.rightCornerSnail = false;

		activateFrame = frame;
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
		if (frame - activateFrame < startup)
			return;

		if (mode == SnailMode.Attack || mode == SnailMode.JumpAttack || mode == SnailMode.Attack2)
			base.HurtPlayer();

		if (mode == SnailMode.JumpAttack)
			sprite.Visible = false;

		if (mode == SnailMode.Attack)
		{
			mode = SnailMode.TurnAround;
			hitConnectFrame = frame;
		}
			
	}

	private void StandbyUpdate()
	{
		ApplyGravity();
		Position = new Vector2(Position.x, Math.Min(Position.y, 245));
		
	}

	private void InactiveUpdate()
	{
		Position = new Vector2(Position.x + 4, Position.y);
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

	private void TurnAroundUpdate()
	{
		if (movingRight)
		{
			Position = new Vector2(Position.x + 4, Position.y);
		}
		else
		{
			Position = new Vector2(Position.x - 4, Position.y);
		}

		if (frame - hitConnectFrame > turnAroundGap)
		{
			EnterAttack2();
		}
	}

	protected override Dictionary<string, int> GetStateSpecific()
	{
		return new Dictionary<string, int>() {
			{ "mode", (int) mode},
			{"hitConnectFrame", hitConnectFrame},
			{"overhead", Globals.BoolToInt(overhead)},
			{"activateFrame", activateFrame}
		};
	}

	protected override void SetStateSpecific(Dictionary<string, int> dict)
	{
		mode = (SnailMode)dict["mode"];
		overhead = Globals.IntToBool(dict["overhead"]);
		hitConnectFrame = dict["hitConnectFrame"];
		activateFrame = dict["activateFrame"];
	}

	private void HandleAttackCommand()
	{
		if (mode == SnailMode.Standby)
		{
			speed.y = 0;
			mode = SnailMode.Attack;
			ExitStandby();
		}
	}

	private void HandleJumpCommand()
	{
		if (mode == SnailMode.Standby)
		{
			speed.y = 0;
			mode = SnailMode.AttackWillJump;
			ExitStandby();
		}
	}

	public override void ReceiveCommand(ProjectileCommand command)
	{
		if (command == ProjectileCommand.SnailAttack)
		{
			HandleAttackCommand();
		}
		else if (command == ProjectileCommand.SnailJump)
		{
			HandleJumpCommand();
		}
		else if (((command == ProjectileCommand.LeftSnailAttack) && movingRight) || ((command == ProjectileCommand.RightSnailAttack) && !movingRight))
		{
			HandleAttackCommand();
		}
		else if (((command == ProjectileCommand.LeftSnailJump) && movingRight) || ((command == ProjectileCommand.RightSnailJump) && !movingRight))
		{
			HandleJumpCommand();
		}
		else if (command == ProjectileCommand.SnailRide && mode != SnailMode.Inactive)
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
				Destroy();
			}
		}
	}

}
