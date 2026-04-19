using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics.SymbolStore;
public class Snail : HadoukenPart
{
	private const string SnailWalkString = "snail-walk";
	private const string SnailTypeString = "Snail";
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

	private bool overhead = false;
	private int activateFrame = 0;
	private int hitConnectFrame = 0;

	private static Color setupColor = new Color(0, 0, 255);
	private static Color readyColor = new Color(0, 255, 0);
	private static Color attackColor = new Color(255, 0, 0);

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

	private SL GetOwner()
	{
		return (SL)targetPlayer.otherPlayer;
	}

	private AnimatedSprite GetSprite()
	{
		return GetNode<AnimatedSprite>("AnimatedSprite");
	}

	public override string GetType()
	{
		return SnailTypeString;
	}

	public override void _Ready()
	{
		base._Ready();
		GetSprite().Rotation = 0;
	}

	public override void _EnterTree()
	{
		base._EnterTree();
		if (GetSprite() != null)
		{
			GetSprite().Rotation = 0;
			GetSprite().Visible = true;
		}
	}

	public override void Spawn(bool movingRight, Player targetPlayer)
	{
		base.Spawn(movingRight, targetPlayer);
		speed.x = 0;
		speed.y = 0;
		hitConnectFrame = 0;
		mode = SnailMode.GetInPosition;
		
		Visible = true;
		

		if (GetOwner().rightCornerSnail && GetOwner().leftCornerSnail)
			Destroy();
		else if ((movingRight && GetOwner().leftCornerSnail) || (!movingRight && GetOwner().rightCornerSnail))
		{
			this.movingRight = !movingRight;
		}
			

		if (this.movingRight)
			GetOwner().leftCornerSnail = true;
		else
			GetOwner().rightCornerSnail = true;

		GetNode<AnimatedSprite>("AnimatedSprite").FlipH = this.movingRight;
	}

	private void Destroy()
	{
		Visible = false;
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
		if (!GetSprite().Visible)
			return;
		switch (mode)
		{
			case SnailMode.GetInPosition:
				EmitSignal(nameof(SnailUpdate), GetOwner().Name, Position.x, setupColor); //ALLOCATION
				break;
			case SnailMode.Standby:
				EmitSignal(nameof(SnailUpdate), GetOwner().Name, Position.x, readyColor);
				break;
			case SnailMode.Inactive:
				break;
			default:
				EmitSignal(nameof(SnailUpdate), GetOwner().Name, Position.x, attackColor);
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

		if (movingRight)
			GetOwner().leftCornerSnailArrived = true;
		else
			GetOwner().rightCornerSnailArrived = true;
	}

	private void ExitStandby()
	{
		if (movingRight)
		{
			GetOwner().leftCornerSnail = false;
			GetOwner().leftCornerSnailArrived = false;
		}
		else
		{
			GetOwner().rightCornerSnail = false;
			GetOwner().rightCornerSnailArrived = false;
		}
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
			{
				EnterStandby();
				Position = new Vector2(Mathf.Floor((Globals.rightWall - 1000) / 100), Position.y);
			}

		}
		else
		{
			Position = new Vector2(Position.x - 4, Math.Min(Position.y, 245));
			if (Position.x * 100 < Globals.leftWall + 1000)
			{
				EnterStandby();
				Position = new Vector2(Mathf.Floor((Globals.leftWall + 1000) / 100), Position.y);
			}
		}
		
	}

	protected override void HurtPlayer(Vector2 collisionPnt)
	{
		if (frame - activateFrame < startup)
			return;

		if (mode == SnailMode.Attack || mode == SnailMode.JumpAttack || mode == SnailMode.Attack2)
		{
			base.HurtPlayer(collisionPnt);
		}
			

		if (mode == SnailMode.JumpAttack)
			GetSprite().Visible = false;

		if (mode == SnailMode.Attack)
		{
			mode = SnailMode.TurnAround;
			hitConnectFrame = frame;
		}
			
	}

	private void StandbyUpdate()
	{
		ApplyGravity();
		if (Position.y > 245)
			speed.y = 0;
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


		TryWalkSound();
	}
	
	private void TryWalkSound()
	{
		if (frame % 15 == 0 && GetSprite().Visible && Position.x * 100 < Globals.rightWall && Position.x * 100 > Globals.leftWall)
				targetPlayer.ForceEvent(EventScheduler.EventType.AUDIO, SnailWalkString);
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
		
		TryWalkSound();
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

		TryRide();

		TryWalkSound();

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

		GetSprite().Rotation = (float)Math.Atan2(speed.y, xMove) + (float)Math.PI;
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

		TryWalkSound();
	}

	private int MODEINDEX = 0;
	private int HITCONNECTFRAMEINDEX = 1;
	private int OVERHEADINDEX = 2;
	private int ACTIVATEFRAMEINDEX = 3;
	protected override int[] GetStateSpecific()
	{
		specificState[MODEINDEX] = (int)mode;
		specificState[HITCONNECTFRAMEINDEX] = (int)hitConnectFrame;
		specificState[OVERHEADINDEX] = Globals.BoolToInt(overhead);
		specificState[ACTIVATEFRAMEINDEX] = activateFrame;

		return base.GetStateSpecific();
	}

	protected override void SetStateSpecific(int[] dict)
	{
		mode = (SnailMode)dict[MODEINDEX];
		overhead = Globals.IntToBool(dict[OVERHEADINDEX]);
		hitConnectFrame = dict[HITCONNECTFRAMEINDEX];
		activateFrame = dict[ACTIVATEFRAMEINDEX];
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

	private void HandleKillCommand()
	{
		if (mode == SnailMode.GetInPosition)
		{
			if (movingRight)
				GetOwner().leftCornerSnail = false;
			else
				GetOwner().rightCornerSnail = false;
		}

		if (mode != SnailMode.Standby)
		{
			Destroy();
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
		else if (command == ProjectileCommand.Kill)
		{
			HandleKillCommand();
		}
			

	}

	public override void HandleOverlap()
	{
		// Snails are FOREVER
	}

	private void TryRide()
	{
		if (!GetOwner().grounded || GetOwner().currentState.tags.Contains(Globals.Tags.hitstate)) return;
		Rect2 myRect = GetRect(collisionShape2D, true);
		GetOwner().GetRects(targetPlayer.hitBoxes, playerRects, true);
		for (int i = 0; i < 3; i++)
		{
			var pRect = playerRects[i];
			if (myRect.Intersects(pRect))
			{
				GetOwner().SnailRide();
				Destroy();
			}
		}
	}

}
