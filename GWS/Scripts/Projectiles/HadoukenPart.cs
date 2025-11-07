using Godot;
using System;
using System.Collections.Generic;
using static BaseAttack;

public class HadoukenPart : Node2D
{
	[Signal]
	public delegate void OnHadoukenOffscreen();


	[Export]
	protected int level = 0;

	protected Globals.AttackDetails hitDetails;
	protected Globals.AttackDetails chDetails;
	protected AnimatedSprite animatedSprite;

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

	[Signal]
	public delegate void OnHitConnected(int hitPush);

	protected int lastHitFrame = -20;

	protected int hits = 0;
	
	protected int frame = 0;

	protected bool movingRight;

	protected Player targetPlayer;

	public string ownerName;

	public bool active = true; // I use this so that when the hadouken collides with the other player it isn't yet deleted, it just turns invisible and inactive.  For rollback reasons.

	public int creationFrame;

	static protected HashSet<int> hadoukenNums = new HashSet<int>();

	protected int num;

	public virtual string hadoukenType { get; } = "Hadouken";

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
		StopHat
	}


	public override void _Ready()
	{
		animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");

		hitDetails = Globals.attackLevels[level].hit;
		chDetails = Globals.attackLevels[level].counterHit;

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
		GetNode<AnimatedSprite>("AnimatedSprite").Playing = true;
		this.movingRight = movingRight;
		this.targetPlayer = targetPlayer;

		// this is a bit lazy...
		this.ownerName = targetPlayer.otherPlayer.Name;

		if (!movingRight) 
		{
			GetNode<AnimatedSprite>("AnimatedSprite").FlipH = true;
		}

		int i = 0;

		while (hadoukenNums.Contains(i))
			i++;

		hadoukenNums.Add(i);
		Name = targetPlayer.Name + Globals.frame; // provides a unique name for each hadouken that can be accessed by the gamestateobj
		num = i;
	}

	public void RemoveNum()
	{
		hadoukenNums.Remove(num);
	}

	[Serializable]
	public struct HadoukenState
	{
		public int[] pos { get; set; }
		public int[] speed { get; set; }
		public bool active { get; set; }
		public string name { get; set; }
		public int frame { get; set; }
		public int lastHitFrame { get; set; }
		public int hits { get; set; }
		public bool visible { get; set; }

		public Dictionary<string, int> dict { get; set; }
	}

	public virtual void AlwaysUpdate()
	{

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
			Globals.Log($"Deleting hadouken {Name}");
			targetPlayer.DeleteHadouken(this); // this shouldn't be done this way, but every possible solution is very inelegant...
		}

		if (active && hits == 0  && frame >= startup)
		{
			Vector2 collisionPnt = CheckRect();
			if (collisionPnt != Vector2.Inf && (frame < duration | duration == 0) && (!targetPlayer.currentState.IsProjectileInvuln()))
			{
				HurtPlayer(targetPlayer.GlobalPosition);
			}
		}

		if ((hits > 0) && (hits < totalHits) && ((frame - lastHitFrame) == breakBetweenHits))
		{
			if (!(targetPlayer.currentState.tags.Contains("hitstate") || targetPlayer.currentState.tags.Contains("block"))) {
				hits = totalHits;
				return;
			}
			
			HurtPlayer(targetPlayer.GlobalPosition);
		}
			
		frame++;
	}

	/// <summary>
	/// checks if the targeted player is inside the collision box
	/// </summary>
	/// <returns></returns>
	protected Vector2 CheckRect()
	{
		Rect2 myRect = GetRect(GetNode<CollisionShape2D>("CollisionShape2D"), true);
		List<Rect2> otherRects = targetPlayer.GetRects(targetPlayer.hitBoxes, true);
		foreach (Rect2 pRect in otherRects)
		{
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
		return GetRect(GetNode<CollisionShape2D>("CollisionShape2D"), true);
	}

	protected virtual void HurtPlayer(Vector2 collisionPnt)
	{
		// fill this with harmful stuff!!!!
		if (targetPlayer.IsInvuln()) // must be a better way to do this.  for now, hadoukens go through knocked down opponent
		{
			return;
		}
		var hitDetailsCopy = hitDetails;
		var chHitDetailsCopy = chDetails;
		hits++;

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
				hitDetailsCopy.hitStun += 10;
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

	public HadoukenState GetState()
	{
		HadoukenState hadState = new HadoukenState();
		hadState.pos = new int[] {(int) Position.x, (int) Position.y};
		hadState.speed = new int[] { (int)speed.x, (int)speed.y };
		hadState.active = active;
		hadState.name = Name;
		hadState.frame = frame;
		hadState.hits = hits;
		hadState.lastHitFrame = lastHitFrame;
		hadState.dict = GetStateSpecific();
		hadState.visible = Visible;
		return hadState;
	}

	protected virtual Dictionary<string, int> GetStateSpecific()
	{
		return new Dictionary<string, int>();
	}

	protected virtual void SetStateSpecific(Dictionary<string, int> dict)
	{

	}

	public virtual void ReceiveCommand(ProjectileCommand command)
	{

	}

	public virtual void SetState(HadoukenState newState) 
	{
		Position = new Vector2(newState.pos[0], newState.pos[1]);
		speed = new Vector2(newState.speed[0], newState.speed[1]);
		active = newState.active;
		Visible = newState.visible;
		frame = newState.frame;
		hits = newState.hits;
		lastHitFrame = newState.lastHitFrame;
		SetStateSpecific(newState.dict);
		Globals.Log($"Rolling back hadouken {Name}, setting hits to {hits}");
	}

	public virtual void ShouldNotExist()
	{

	}

	
}
