using Godot;
using System;
using System.Collections.Generic;
using System.Linq;


public abstract class BaseAttack : State
{
	public override HashSet<Globals.Tags> tags { get; set; } = new HashSet<Globals.Tags>() { Globals.Tags.attack };
	private const string PrejumpString = "PreJump";
	private const string IdleString = "Idle";
	private const string OhshitString = "OHSHIT";
	private const string FallString = "Fall";
	private const string SuperFlashString = "SuperFlash";
	private const string SuperPowerUpString = "SuperPowerUp";
	private const string SpikeString = "Spike";
	[Export]
	public int level = 0;

	[Export]
	protected bool chip = false;

	[Export]
	protected int modifiedProration = 0;

	protected Globals.AttackDetails hitDetails;
	protected Globals.AttackDetails chDetails;

	[Export]
	protected int modifiedHitStop = 0;

	[Export]
	protected int modifiedHitStun = 0;

	[Export]
	protected int modifiedCounterHitStun = 0;

	[Export]
	protected Vector2 opponentLaunch = Vector2.Zero;

	[Export]
	protected Vector2 chLaunch = Vector2.Zero;

	[Export]
	protected int slowTerminalVelocity = 0;

	[Export]
	protected int modifiedHitPush = 0;

	[Export]
	protected int modifiedDmg = 0;

	[Export]
	protected int hitPush = 0;

	[Export]
	protected HEIGHT height = HEIGHT.MID;

	[Export]
	protected bool jumpCancelable = false;


	[Export]
	protected EXTRAEFFECT effect = EXTRAEFFECT.NONE;

	[Export]
	protected EXTRAEFFECT chEffect = EXTRAEFFECT.NONE;

	[Export]
	protected GRAPHICEFFECT hitGfx = GRAPHICEFFECT.NONE;

	[Export]
	protected bool knockdown = false;

	[Export]
	protected bool launchOnGrounded = true;

	[Export]
	protected string whiffSound = "Whiff";

	[Export]
	protected string hitSound = "Hit";

	[Export]
	protected bool turnAroundOnEnter = false;

	/// <summary>
	/// Gatlings must be input before this window closes
	/// </summary>
	[Export]
	protected int gatlingWinEnd = 0;

	[Export]
	public int[] restoreHitFrames;

	[Signal]
	public delegate void OnHitConnected(int hitPush);

	[Export]
	public int superFrame = 0;

	[Export]
	public int grabInvulnFrames = 0;

	[Export]
	public int projectileInvulnFrames = 0;

	[Export]
	public bool exitOnHit = false;

	[Export]
	public string selfGatlingInp = " ";

	[Export]
	public string superKaraButton = "";

	[Export]
	public bool specialBurstKara = false;

	[Export]
	public int lastHitFrame = 0;

	[Export]
	public bool lastHitKnockdown = false;

	[Export]
	public Vector2 lastHitLaunch = new Vector2(0, 0);

	[Export]
	public bool spike = false;

	[Export]
	public int pullInHitFrame = 0;


	public enum EXTRAEFFECT
	{
		NONE,
		GROUNDBOUNCE,
		WALLBOUNCE,
		STAGGER,
		LAUNCHER
	}

	public enum GRAPHICEFFECT
	{
		NONE,
		EXPLOSION,
		PURPLE,
		SNAIL,
		SLASH,
		ELECTROCUTE,
		SPARKS
	}


	public enum ATTACKDIR
	{
		RIGHT,
		LEFT,
		EQUAL
	}

	protected List<NormalGatling> whiffGatlings = new List<NormalGatling>();

	public override void _Ready()
	{
		base._Ready();
		stop = false;
		isCounter = true;
		slowdownSpeed = 80;
		Connect("OnHitConnected", owner, nameof(owner.OnHitConnected));
		hitDetails = Globals.attackLevels[level].hit;
		chDetails = Globals.attackLevels[level].counterHit;

		hitDetails.opponentLaunch = opponentLaunch;
		if (chLaunch.y != 0)
			chDetails.opponentLaunch = chLaunch;
		else
			chDetails.opponentLaunch = opponentLaunch;

		if (modifiedProration != 0)
		{
			hitDetails.prorationLevel = modifiedProration;
			chDetails.prorationLevel = modifiedProration;
		}


		hitDetails.chipDmg = chip;

		hitDetails.effect = effect;
		chDetails.effect = chEffect;
		hitDetails.knockdown = knockdown;
		chDetails.knockdown = knockdown;
		hitDetails.height = height;
		chDetails.height = height;
		hitDetails.graphicFX = hitGfx;
		chDetails.graphicFX = hitGfx;

		if (modifiedHitStop != 0)
		{
			hitDetails.hitStop = modifiedHitStop;
		}

		if (modifiedHitStun != 0)
			hitDetails.hitStun = modifiedHitStun;

		if (modifiedCounterHitStun != 0)
			chDetails.hitStun = modifiedCounterHitStun;

		if (modifiedDmg > 0)
		{
			hitDetails.dmg = modifiedDmg;
			chDetails.dmg = modifiedDmg;
		}

		if (modifiedHitPush != 0)
		{
			hitDetails.hitPush = modifiedHitPush;
			chDetails.hitPush = modifiedHitPush;

		}

		hitDetails.spike = spike;

		if (superKaraButton.Length > 0)
			AddKara(new char[] { superKaraButton[0], 'p' }, () => owner.grounded && owner.TrySpendMeter() && owner.specialBreakFramesRemaining <= 0, owner.easySuper);

		if (specialBurstKara)
		{
			AddBurstKara('k', 'p');
		}

		if (selfGatlingInp[0] != ' ')
		{
			AddGatling(new char[] { selfGatlingInp[0], 'p' }, Name);
		}

	}

	protected virtual void AddJumpCancel()
	{
		AddGatling(new char[] { '8', 'p' }, () => owner.CheckHeldKey('6'), PrejumpString, () => owner.velocity.x = owner.speed);
		AddGatling(new char[] { '8', 'p' }, () => owner.CheckHeldKey('4'), PrejumpString, () => owner.velocity.x = -owner.speed);
		AddGatling(new char[] { '8', 'p' }, PrejumpString);
	}
	public override void Enter()
	{
		owner.ZIndex = 1;
		base.Enter();
		hitConnect = false;
		owner.grabInvulnFrames = grabInvulnFrames;
		owner.ScheduleEvent(EventScheduler.EventType.AUDIO, whiffSound, Name);
		if (superFrame != 0)
			owner.ScheduleEvent(EventScheduler.EventType.AUDIO, OhshitString, Name);
		if (turnAroundOnEnter)
			owner.CheckTurnAround();
	}

	/// <summary>
	/// WARNING!  If you modify this you must also modify LaunchAttack.cs as it does NOT inherit
	/// </summary>
	public override void FrameAdvance()
	{
		base.FrameAdvance();
		if (frameCount > 0 && frameCount == superFrame)
		{
			Globals.EmitSignal(Globals.PlayerSignal.SuperFlash, owner.Name);
			owner.GFXEvent(SuperPowerUpString);
		}

		if (restoreHitFrames != null && restoreHitFrames.Contains(frameCount))
		{
			hitConnect = false;
		}

	}
	public override void AnimationFinished()
	{
		if (owner.grounded)
			owner.ChangeState(IdleString);
		else
			owner.ChangeState(FallString);
	}

	public override void TryBurst()
	{
		// No bursting while attacking!
	}

	public override void CheckHit()
	{
		if (!hitConnect)
		{
			Vector2 collisionPnt = owner.CheckHurtRect();
			if (collisionPnt != Vector2.Inf)
			{
				InHurtbox(collisionPnt);
			}

		}
	}

	public override void InHurtbox(Vector2 collisionPnt)
	{

		var hitDetails = this.hitDetails;
		var chDetails = this.chDetails;

		if (pullInHitFrame > 0 && frameCount > pullInHitFrame)
		{
			hitDetails.hitPush *= -1;
			chDetails.hitPush *= -1;
		}

		if (owner.hasDoubleOrSuperJumped && hitDetails.spike && owner.otherPlayer.combo > 2)
		{
			hitDetails.ignoreProration = true;
			owner.EmitSignal(nameof(Player.GenericGFX), SpikeString, owner.Name);
		}

		if ((owner.otherPlayer.grounded && owner.otherPlayer.currentState.Name != "Knockdown") && !launchOnGrounded)
		{
			hitDetails.opponentLaunch = Vector2.Zero;
			chDetails.opponentLaunch = Vector2.Zero;
			hitDetails.effect = EXTRAEFFECT.STAGGER;
			chDetails.effect = EXTRAEFFECT.STAGGER;
		}
		else
		{
			hitDetails.opponentLaunch = opponentLaunch;
			chDetails.opponentLaunch = chLaunch.y > 0 ? chLaunch : opponentLaunch;
			if (!launchOnGrounded)
			{
				hitDetails.hitStun += 10;
				chDetails.hitStun += 10;
			}


			if (lastHitFrame != 0 && frameCount >= lastHitFrame)
			{
				hitDetails.knockdown = lastHitKnockdown;

				hitDetails.opponentLaunch = lastHitLaunch;

			}
		}

		owner.GainMeter(hitDetails.dmg * 50);
		EmitSignal(nameof(OnHitConnected), hitDetails.hitPush);
		var direction = ATTACKDIR.EQUAL;

		if (owner.OtherPlayerOnRight())
		{
			direction = ATTACKDIR.RIGHT;
		}
		else if (owner.OtherPlayerOnLeft())
		{
			direction = ATTACKDIR.LEFT;
		}

		hitDetails.dir = direction;
		chDetails.dir = direction;
		hitDetails.collisionPnt = collisionPnt;
		chDetails.collisionPnt = collisionPnt;
		if (slowTerminalVelocity != 0)
		{
			owner.otherPlayer.terminalVelocity = slowTerminalVelocity;
		}
		else
		{
			owner.otherPlayer.currentState.ResetTerminalVelocity();
		}
		owner.otherPlayer.ReceiveHit(hitDetails, chDetails);
		
		
		hitConnect = true;
		owner.ScheduleEvent(EventScheduler.EventType.AUDIO, hitSound, Name);

		if (exitOnHit)
		{
			if (owner.grounded)
				owner.ChangeState(IdleString);
			else
				owner.ChangeState(FallString);
		}


	}

	/// <summary>
	/// short input buffer for links and microdashes
	/// </summary>
	/// <returns></returns>
	public override bool DelayInputs()
	{
		return (frameCount > animationLength - 5);
	}

	public override void HandleInput(char[] inputArr)
	{
		if (frameCount < 3)
		{
			foreach (KaraGatling karaGat in karaGatlings)
			{
				char[] testInp = karaGat.input;
				testInp = ReverseInput(testInp);
				if (Enumerable.SequenceEqual(karaGat.input, inputArr))
				{
					if (karaGat.reqCall != null)
					{
						if (!karaGat.reqCall())
						{
							continue;
						}
					}

					karaGat.postCall?.Invoke();


					owner.ChangeState(karaGat.state);

					return;
				}
			}
		}

		if (!hitConnect && frameCount > 8)
		{
			foreach (var whiffGat in whiffGatlings)
			{
				if (Enumerable.SequenceEqual(whiffGat.input, inputArr))
					owner.ChangeState(whiffGat.state);
			}
		}
		if ((gatlingWinEnd == 0 || frameCount < gatlingWinEnd) && hitConnect)
			base.HandleInput(inputArr);
	}


	public override void ReceiveHit(Globals.AttackDetails details)
	{
		ReceiveHitNoBlock(details);
	}

	public override void Exit()
	{
		base.Exit();
		owner.ZIndex = 0;
	}

	public override bool IsProjectileInvuln()
	{
		return frameCount <= projectileInvulnFrames;
	}
}
