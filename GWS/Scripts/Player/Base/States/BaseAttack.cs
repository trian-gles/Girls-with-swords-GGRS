using Godot;
using System;
using System.Collections.Generic;
using System.Linq;


public abstract class BaseAttack : State
{
	public override HashSet<string> tags { get; set; } = new HashSet<string>() {"attack" };

	[Export]
	protected int level = 0;

	protected Globals.AttackDetails hitDetails;
	protected Globals.AttackDetails chDetails;


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

	public enum EXTRAEFFECT
	{
		NONE,
		GROUNDBOUNCE,
		WALLBOUNCE,
		STAGGER
	}

	public enum GRAPHICEFFECT
	{
		NONE,
		EXPLOSION
	}


	public enum ATTACKDIR
	{
		RIGHT,
		LEFT,
		EQUAL
	}
	
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
		if (chLaunch != Vector2.Zero)
			chDetails.opponentLaunch = chLaunch;
		else
			chDetails.opponentLaunch = opponentLaunch;

		hitDetails.effect = effect;
		chDetails.effect = chEffect;
		hitDetails.knockdown = knockdown;
		chDetails.knockdown = knockdown;
		hitDetails.height = height;
		chDetails.height = height;
		hitDetails.graphicFX = hitGfx;
		chDetails.graphicFX = hitGfx;

		if (modifiedHitStun != 0)
			hitDetails.hitStun = modifiedHitStun;
		
		if (modifiedCounterHitStun != 0)
			chDetails.hitStun = modifiedCounterHitStun;

		if (modifiedDmg >0)
		{
			hitDetails.dmg = modifiedDmg;
			chDetails.dmg = modifiedDmg;
		}

		if (modifiedHitPush != 0)
		{
			hitDetails.hitPush = modifiedHitPush;
			chDetails.hitPush = modifiedHitPush;

		}

		if (superKaraButton.Length > 0)
			AddKara(new char[] { superKaraButton[0], 'p' }, () => owner.grounded && owner.TrySpendMeter(), owner.easySuper);

		AddRhythmSpecials(owner.rhythmSpecials);
		if (selfGatlingInp != " ")
        {
			AddGatling(new char[] { selfGatlingInp[0], 'p' }, Name);
			GD.Print($"Adding gatling for {Name} upon press of {selfGatlingInp}");
        }

	}

	protected virtual void AddJumpCancel()
	{
		AddGatling(new char[] { '8', 'p' }, () => owner.CheckHeldKey('6'), "PreJump", () => owner.velocity.x = owner.speed);
		AddGatling(new char[] { '8', 'p' }, () => owner.CheckHeldKey('4'), "PreJump", () => owner.velocity.x = -owner.speed);
		AddGatling(new char[] { '8', 'p' }, "PreJump");
	}
	public override void Enter()
	{
		owner.ZIndex = 1;
		base.Enter();
		hitConnect = false;
		owner.grabInvulnFrames = grabInvulnFrames;
		owner.ScheduleEvent(EventScheduler.EventType.AUDIO, whiffSound, Name);
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
			owner.EmitSignal("SuperFlash", owner.Name);
			owner.GFXEvent("SuperPowerUp");
		} 

		if (restoreHitFrames != null && restoreHitFrames.Contains(frameCount))
		{
			hitConnect = false;
		}
			
	}
	public override void AnimationFinished()
	{
		if (owner.grounded)
			EmitSignal(nameof(StateFinished), "Idle");
		else
			EmitSignal(nameof(StateFinished), "Fall");
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
		Globals.Log($"Hit connect at point {collisionPnt}");

		var hitDetails = this.hitDetails;
		var chDetails = this.chDetails;

		if (owner.otherPlayer.grounded && !launchOnGrounded)
		{
			hitDetails.opponentLaunch = Vector2.Zero;
			chDetails.opponentLaunch = Vector2.Zero;
		}

		owner.GainMeter(400);
		EmitSignal(nameof(OnHitConnected), hitDetails.hitPush);
		var direction = ATTACKDIR.EQUAL;

		if (owner.OtherPlayerOnRight())
		{
			direction = ATTACKDIR.RIGHT;
		}
		else if(owner.OtherPlayerOnLeft())
		{
			direction = ATTACKDIR.LEFT;
		}
		
		hitDetails.dir = direction;
		chDetails.dir = direction;
		hitDetails.collisionPnt = collisionPnt;
		chDetails.collisionPnt = collisionPnt;
		owner.otherPlayer.ReceiveHit(hitDetails, chDetails);
		owner.otherPlayer.currentState.ResetTerminalVelocity();
		if (slowTerminalVelocity != 0)
		{
			owner.otherPlayer.terminalVelocity = slowTerminalVelocity;
		}
		hitConnect = true;
		owner.ScheduleEvent(EventScheduler.EventType.AUDIO, hitSound, Name);

		if (exitOnHit)
		{
			if (owner.grounded)
				EmitSignal(nameof(StateFinished), "Idle");
			else
				EmitSignal(nameof(StateFinished), "Fall");
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


					EmitSignal(nameof(StateFinished), karaGat.state);

					return;
				}
			}
		}

		if (!hitConnect)
		{
			return;
		}
		if (gatlingWinEnd == 0 || frameCount < gatlingWinEnd)
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



	//protected override void EnterHitState(bool knockdown, Vector2 launch, Vector2 collisionPnt, BaseAttack.EXTRAEFFECT effect)
	//{
	//	GetNode<Node>("/root/Globals").EmitSignal(nameof(PlayerFXEmitted), collisionPnt, "hit", false);
	//	bool launchBool = false;
	//	owner.ComboUp();
	//	if (!(launch == Vector2.Zero)) // LAUNCH NEEDS MORE WORK
	//	{
	//		GD.Print("Launch is not zero!");
	//		owner.velocity = launch;
	//		launchBool = true;
	//	}

	//	bool airState = (launchBool || !owner.grounded);

	//	if (effect == BaseAttack.EXTRAEFFECT.GROUNDBOUNCE)
	//	{
	//		EmitSignal(nameof(StateFinished), "GroundBounce");
	//	}
	//	else if (effect == BaseAttack.EXTRAEFFECT.WALLBOUNCE)
	//	{
	//		EmitSignal(nameof(StateFinished), "WallBounce");
	//	}
	//	else if (launchBool && !knockdown)
	//	{
	//		GD.Print("Entering counterfloat from attack");
	//		EmitSignal(nameof(StateFinished), "CounterFloat");
	//	}
	//	else if (airState && knockdown)
	//	{
	//		GD.Print("Entering airknockdown from attack");
	//		EmitSignal(nameof(StateFinished), "AirKnockdown");
	//	}
	//	else if (!airState && knockdown)
	//	{
	//		GD.Print("Entering knockdown from attack");
	//		EmitSignal(nameof(StateFinished), "Knockdown");

	//	}
	//	else
	//	{
	//		EmitSignal(nameof(StateFinished), "CounterHit");
	//	}
	//}


}
