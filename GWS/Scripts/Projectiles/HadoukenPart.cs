using Godot;
using System;
using System.Collections.Generic;
using static BaseAttack;

using System.Runtime.InteropServices;

public class HadoukenPart : Node2D
{
	private const string HitStunString = "HitStun";
	private const string HadoukenTypeString = "Hadouken";
	[Signal]
	public delegate void OnHadoukenOffscreen();


	[Export]
	protected int level = 0;

	protected Globals.AttackDetails hitDetails;
	protected Globals.AttackDetails chDetails;
	protected AnimatedSprite animatedSprite;
	protected CollisionShape2D collisionShape2D;

	private Color hurtColor = new Color(255, 0, 0, 0.9f);

	[Export]
	protected int startup = 0;

	[Export]
	protected int modifiedHitStun = 0;

	[Export]
	protected int modifiedCounterHitStun = 0;

	[Export]
	protected Vector2 opponentLaunch = Vector2.Zero;

	[Export]
	protected Vector2 chLaunch = Vector2.Zero;

	[Export]
	protected bool launchOnGrounded = true;

	[Export]
	protected int modifiedHitPush = 0;

	[Export]
	protected int hitPush = 0;

	[Export]
	protected State.HEIGHT height = State.HEIGHT.MID;

	[Export]
	protected BaseAttack.EXTRAEFFECT effect = BaseAttack.EXTRAEFFECT.NONE;

	[Export]
	protected BaseAttack.EXTRAEFFECT chEffect = BaseAttack.EXTRAEFFECT.NONE;

	[Export]
	public BaseAttack.GRAPHICEFFECT hitGfx = BaseAttack.GRAPHICEFFECT.NONE;

	[Export]
	protected bool knockdown = false;

	[Export]
	public bool removeOTG;

	[Export]
	public Vector2 speed;

	[Export]
	public Vector2 postHitSpeed;

	[Export]
	public int duration;

	[Export]
	public int totalHits = 1;

	[Export]
	public int breakBetweenHits = 8;

	[Export]
	public bool dieAfterHit = true;

	[Export]
	protected int slowTerminalVelocity = 0;

	[Export]
	protected bool hitOTG = false;

	[Export]
	protected bool isProjectile = true;

	[Signal]
	public delegate void OnHitConnected(int hitPush);

	[Export]
	public int modifiedProration = 0;

	protected int lastHitFrame = -20;
	public int id;

	protected int hits = 0;
	
	protected int frame = 0;

	protected bool movingRight;

	protected Player targetPlayer;
	protected int[] specificState = new int[6];

	public string ownerName;

	public bool active = false; // I use this so that when the hadouken collides with the other player it isn't yet deleted, it just turns invisible and inactive.  For rollback reasons.
	public bool freed = true;

	public int creationFrame;

	static protected HashSet<int> hadoukenNums = new HashSet<int>();  // TODO : should be fixed size

	protected int num;

	public virtual string hadoukenType { get; } = HadoukenTypeString;

	public enum ProjectileCommand
	{
		SnailAttack,
		RightSnailAttack,
		LeftSnailAttack,
		RightSnailJump,
		LeftSnailJump,
		SnailJump,
		SnailRide,
		BlackHolePowerUp,
		BlackHoleDeactivate,
		DeleteHat,
		StopHat,
		MoveHatRight,
		MoveHatLeft,
		Kill
	}

	public override void _Ready()
	{
		hadState = new HadoukenState();

		hitDetails = Globals.attackLevels[level].hit;
		chDetails = Globals.attackLevels[level].counterHit;

		hitDetails.chipDmg = true;

		hitDetails.projectile = true;
		chDetails.projectile = true;

		hitDetails.hitStop = 0;
		chDetails.hitStop = 0;

		hitDetails.opponentLaunch = opponentLaunch;
		if (chLaunch != Vector2.Zero)
			chDetails.opponentLaunch = chLaunch;

		hitDetails.effect = effect;
		chDetails.effect = chEffect;
		hitDetails.knockdown = knockdown;
		chDetails.knockdown = knockdown;
		hitDetails.height = height;
		chDetails.height = height;
		hitDetails.airBlockable = true;

		hitDetails.opponentLaunch = opponentLaunch;
		chDetails.opponentLaunch = chLaunch;
		if (modifiedProration != 0)
		{
			hitDetails.prorationLevel = modifiedProration;
			chDetails.prorationLevel = modifiedProration;
		}

		hitDetails.graphicFX = hitGfx;
		chDetails.graphicFX = hitGfx;

		if (removeOTG)
			hitDetails.removeOTG = removeOTG;

		if (modifiedHitStun != 0)
				hitDetails.hitStun = modifiedHitStun;
		if (modifiedCounterHitStun != 0)
			chDetails.hitStun = modifiedCounterHitStun;

		if (modifiedHitPush != 0)
		{
			hitDetails.hitPush = modifiedHitPush;
			chDetails.hitPush = modifiedHitPush;

		}
	}

	/// <summary>
	/// Method to be called right after instantiation by the player
	/// </summary>
	/// <param name="movingRight"></param>
	/// <param name="targetPlayer"> the targeted player </param>
	public virtual void Spawn(bool movingRight, Player targetPlayer)
	{
		if (animatedSprite == null)
			animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
		if (collisionShape2D == null)
			collisionShape2D = GetNode<CollisionShape2D>("CollisionShape2D");
		animatedSprite.Frame = 0;
		animatedSprite.Playing = true;
		this.movingRight = movingRight;
		var tempScale = collisionShape2D.Scale;
		tempScale.x = movingRight ? 1 : -1;
		collisionShape2D.Scale = tempScale;
		this.targetPlayer = targetPlayer;

		// this is a bit lazy...
		this.ownerName = targetPlayer.otherPlayer.Name;
		freed = false;
		active = true;
		lastHitFrame = -20;

		hits = 0;
		
		frame = 0;
		Visible = true;

		animatedSprite.FlipH = !movingRight;

		int i = 0;

		while (hadoukenNums.Contains(i))
			i++;

		hadoukenNums.Add(i);
		id = Globals.frame;
		if (targetPlayer.Name[1] == '2')
			id += 10000;
		
		num = i;
	}

	public void RemoveNum()
	{
		hadoukenNums.Remove(num);
	}

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public unsafe struct HadoukenState
	{
		public int posx; // 4 bytes
		public int posy;  // 4 bytes
		public int speedx; // 4 bytes
		public int speedy; // 4 bytes
		public bool active; // 1 byte
		public int id; // 2 * 7 = 14 bytes
		public int frame; // 4 bytes
		public int lastHitFrame; // 4 bytes
		public int hits; // 4 bytes
		public bool visible; // 1 byte

		public fixed int dict[6]; // 4 * 6 = 24 bytes
	}

	public virtual void AlwaysUpdate()
	{

	}

	public override void _Process(float delta)
	{
		base._Process(delta);
		if (Globals.mode == Globals.Mode.TRAINING || Globals.mode == Globals.Mode.SYNCTEST)
			Update();
	}

	public virtual void FrameAdvance() // wait till the turn after it was created to move the hadouken
	{
		
		if (frame > 0)
		{
			
			Vector2 trueSpeed = new Vector2(speed);
			if (hits > 0 && postHitSpeed != Vector2.Zero)
				trueSpeed = new Vector2(postHitSpeed);

			if (!movingRight)
			{
				trueSpeed.x *= -1;
			}


			Position += trueSpeed;
			
			//Globals.Log($"Moving {Name} to position {Position} with rect {GetRect(GetNode<CollisionShape2D>("CollisionShape2D"), true)}, player at position {targetPlayer.internalPos}");
		}
		

		
		if (Position.x > 1900 || Position.x < -1600 || Position.y > 1800) // To ensure the fireball isn't deleted before it could be potentially rolled back, these values are quite high.
		{
			//Globals.Log($"Deleting hadouken {Name}, out of bounds");
			targetPlayer.DeleteHadouken(this); // this shouldn't be done this way, but every possible solution is very inelegant...
		}
		
		if (active && hits == 0  && frame >= startup)
		{
			Vector2 collisionPnt = CheckRect();
			if (collisionPnt != Vector2.Inf && (frame < duration | duration == 0) && (!targetPlayer.currentState.IsProjectileInvuln() || !isProjectile))
			{
				HurtPlayer(targetPlayer.GlobalPosition);
			}
		}
		

		if ((hits > 0) && (hits < totalHits) && ((frame - lastHitFrame) == breakBetweenHits))
		{
			if (!(targetPlayer.currentState.tags.Contains(Globals.Tags.hitstate) || targetPlayer.currentState.tags.Contains(Globals.Tags.block))) {
				hits = totalHits;
				return;
			}
			
			HurtPlayer(targetPlayer.GlobalPosition);
		}
			
		frame++;
		
	}

	protected Rect2[] playerRects = new Rect2[3];
	/// <summary>
	/// checks if the targeted player is inside the collision box
	/// </summary>
	/// <returns></returns>
	protected Vector2 CheckRect()
	{
		Rect2 myRect = GetRect(collisionShape2D, true);
		targetPlayer.GetRects(targetPlayer.hitBoxes, playerRects, true);

		for (int i = 0; i < 3; i++)
		{
			var pRect = playerRects[i];
			if (myRect.Intersects(pRect))
			{

				//Globals.Log($"Hadouken hitbox intersection! Hadouken {Name} at position {Position} with rect {myRect} player at position {targetPlayer.internalPos} with rect {pRect}");
				Rect2 clip = myRect.Clip(pRect);
				Vector2 center = (clip.End - clip.Position) / 2 + clip.Position;
				return center;
			}
		}
		return Vector2.Inf;
	}

	public virtual void HandleOverlap()
	{
		hits--;
		if (hits <= 0)
		{
			MakeInactive();
		}
			
	}

	public Rect2 GetCollisionRect()
	{
		return GetRect(collisionShape2D, true);
	}

	protected virtual void HurtPlayer(Vector2 collisionPnt)
	{
		// fill this with harmful stuff!!!!
		if (targetPlayer.IsInvuln())
		{
			return;
		}
		var hitDetailsCopy = hitDetails;
		var chHitDetailsCopy = chDetails;
		hits++;

		targetPlayer.ForceEvent(EventScheduler.EventType.AUDIO, HitStunString);
		if (!launchOnGrounded && targetPlayer.currentState.Name != "Knockdown" && targetPlayer.grounded)
		{
			hitDetailsCopy.opponentLaunch = Vector2.Zero;
			chHitDetailsCopy.opponentLaunch = Vector2.Zero;
			hitDetailsCopy.effect = EXTRAEFFECT.STAGGER;
			chHitDetailsCopy.effect = EXTRAEFFECT.STAGGER;
		}
		else
		{
			hitDetailsCopy.opponentLaunch = opponentLaunch;
			chHitDetailsCopy.opponentLaunch = chLaunch;
			if (!launchOnGrounded)
			{
				hitDetailsCopy.hitStun += 15;
				chHitDetailsCopy.hitStun += 15;
			}
				
		}

		hitDetailsCopy.dir = BaseAttack.ATTACKDIR.RIGHT;
		chHitDetailsCopy.dir = BaseAttack.ATTACKDIR.RIGHT;
		if (!movingRight)
		{
			hitDetailsCopy.dir = BaseAttack.ATTACKDIR.LEFT;
			chDetails.dir = BaseAttack.ATTACKDIR.LEFT;
		}
		hitDetailsCopy.collisionPnt = collisionPnt * 100;
		chHitDetailsCopy.collisionPnt = collisionPnt * 100;

		if (slowTerminalVelocity != 0)
		{
			targetPlayer.terminalVelocity = slowTerminalVelocity;
		}

		EmitSignal(nameof(OnHitConnected), 0); // don't push ourselves if the opponent is in the corner eating a hadouken!
		targetPlayer.ReceiveHit(hitDetailsCopy, chHitDetailsCopy);
		lastHitFrame = frame;
		

		if (hits == totalHits)
			MakeInactive();
	}

	protected virtual void MakeInactive()
	{
		if (dieAfterHit)
			Visible = false;
		active = false;
	}

	protected Rect2 GetRect(CollisionShape2D colShape, bool globalPosition = false)
	{
		RectangleShape2D shape = (RectangleShape2D)colShape.Shape;
		Vector2 extents = shape.Extents * 200;
		Vector2 position;
		if (movingRight)
		{
			position = colShape.Position * 100 - extents / 2;
		}
		else
		{
			position = new Vector2(-colShape.Position.x * 100 - extents.x / 2, colShape.Position.y * 100 - extents.y / 2);
		}
		if (globalPosition)
		{
			position += Position * 100;
		}

		return new Rect2(position, extents);
	}

	HadoukenState hadState = new HadoukenState();
	private int[] tempDict = new int[6];
	public unsafe HadoukenState GetState()
	{
		hadState.posx = (int) Position.x;

		hadState.posy = (int) Position.y;
		hadState.speedx = (int)speed.x;
		hadState.speedy = (int)speed.y;
		hadState.active = active;
		hadState.id = id;
		hadState.frame = frame;
		hadState.hits = hits;
		hadState.lastHitFrame = lastHitFrame;
		tempDict = GetStateSpecific();
		for (int i = 0; i < tempDict.Length; i++)
			hadState.dict[i] = tempDict[i];
		hadState.visible = Visible;
		return hadState;
	}

	protected virtual int[] GetStateSpecific()
	{
		return specificState;
	}

	protected virtual void SetStateSpecific(int[] dict)
	{

	}

	public virtual void ReceiveCommand(ProjectileCommand command)
	{
		if (command == ProjectileCommand.Kill)
			MakeInactive();
	}

	
	public unsafe virtual void SetState(HadoukenState newState) 
	{
		Position = new Vector2(newState.posx, newState.posy);
		speed = new Vector2(newState.speedx, newState.speedy);
		
		active = newState.active;
		Visible = newState.visible;
		frame = newState.frame;
		hits = newState.hits;
		lastHitFrame = newState.lastHitFrame;
		for (int i = 0; i < tempDict.Length; i++)
			tempDict[i] = newState.dict[i];
		SetStateSpecific(tempDict);
		if (Globals.logOn)
			Globals.Log($"Rolling back hadouken {Name}, setting hits to {hits}");
	}

	public virtual void ShouldNotExist()
	{

	}



	


	public override void _Draw()
	{
		
		if (Globals.mode == Globals.Mode.TRAINING || Globals.mode == Globals.Mode.SYNCTEST)
		{
			Rect2 myRect = GetRect(collisionShape2D, false);
			var tinyExtents = myRect.Size / 100;
			var tinyPos = myRect.Position / 100;
			var tinyRect = new Rect2(tinyPos, tinyExtents);
			if (active)
				DrawRect(tinyRect, hurtColor);
		}
	}
}
