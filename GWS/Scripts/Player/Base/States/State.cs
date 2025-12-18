using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using FixedMath.NET;
using System.Security.Cryptography.X509Certificates;

/// <summary>
/// Base class for all states
/// </summary>
public abstract class State : Node
{

	[Export]
	public bool hasGravity = true; // certain flying states need to ignore gravity, see ApplyGravity()
	public virtual HashSet<String> tags { get; set; } = new HashSet<String>();

	public Player owner;
	public int frameCount
	{ get; set; }

	/// <summary>
	/// if this is true, the character immediately stops on entering the state
	/// </summary>
	protected bool stop = true;

	protected int slowdownSpeed = 0;

	/// <summary>
	/// Whether this state receives counter hits
	/// </summary>
	public bool isCounter = false;

	/// <summary>
	/// If the character should change animations for this state
	/// </summary>
	public bool hasAntimation = true;

	/// <summary>
	/// The animation that should be called upon entering
	/// </summary>
	public virtual string animationName
	{
		get { return Name;  }
	}
	public bool shrinkOtherSprite = false;

	protected float animationLength;

	[Signal]
	public delegate void StateFinished(string nextStateName);

	[Signal]
	public delegate void PlayerFXEmitted(Vector2 pos, ParticleSprite particle, bool flipH);

	[Signal]
	public delegate void GhostEmitted(Player p);

	public int stunRemaining 
	{ get; set; }

	public virtual bool wasHit
	{ get { return false; } }

	public bool loop = false;

	public bool hitConnect = false;

	[Export]
	public bool turnAroundOnExit = true;


	public enum HEIGHT
	{
		LOW,
		MID,
		HIGH
	}

	public enum GFXStates
	{
		NONE,
		SHIELD,
		SHIELDACTIVE,
		CANTECH
	}

	protected List<NormalGatling> normalGatlings = new List<NormalGatling>();
	protected List<CommandGatling> commandGatlings = new List<CommandGatling>();
	protected List<KaraGatling> karaGatlings = new List<KaraGatling>();
	protected List<RhythmGatling> rhythmGatlings = new List<RhythmGatling>();
	protected delegate bool RequiredConditionCallback();
	protected delegate void PostInputCallback();
	public override void _Ready()
	{
		owner = GetOwner<Player>();
		animationLength = owner.GetAnimationLength(animationName);
	}

	public virtual void Load(Dictionary<string, int> loadData)
	{

	}

	public virtual Dictionary<string, int> Save()
	{
		return new Dictionary<string, int>();
	}

	/// <summary>
	/// Called right when switching into this state.  NOT called when a game state is loaded
	/// </summary>
	public virtual void Enter() 
	{
		frameCount = 0;
		if (stop)
		{
			owner.velocity.x = 0;
		}
	}

	/// <summary>
	/// Called right when exiting this state.  NOT called when a game state is loaded
	/// </summary>
	public virtual void Exit()
	{
		hitConnect = false;
	}

	protected virtual void ApplyGravity()
	{
		if (owner.counterStopFrames > 0 || !hasGravity)
		{
			return;
		}
		owner.velocity.y = Math.Min(owner.velocity.y + owner.gravity, CheckTerminalVelocity());
	}
	public virtual void AnimationFinished() 
	{
	}

	
	protected struct NormalGatling
	{
		public char[] input;
		public string state;
		public RequiredConditionCallback reqCall; //if this returns true, we can enter the specified state
		public PostInputCallback postCall;
	}

	/// <summary>
	/// Represents any action that requires a series of inputs
	/// </summary>
	protected struct CommandGatling
	{
		public List<char[]> inputs;
		public string state;
		public RequiredConditionCallback reqCall; //if this returns true, we can enter the specified state
		public PostInputCallback postCall;
		public bool preventMash;
		public bool flipInputs; // if this input should change depending on which way we are facing
	}

	protected struct KaraGatling
	{
		public char[] input;
		public string state;
		public RequiredConditionCallback reqCall; //if this returns true, we can enter the specified state
		public PostInputCallback postCall;
	}

	protected struct RhythmGatling
	{
		public List<char[]> inputs;
		public string state;
		public RequiredConditionCallback reqCall; //if this returns true, we can enter the specified state
		public PostInputCallback postCall;
		public bool preventMash;
		public bool flipInputs; // if this input should change depending on which way we are facing
	}

	protected char[] ReverseInput(char[] inp)
	{
		char[] newInp = new char[2];

		inp.CopyTo(newInp, 0);

		if (inp[0] == '4')
		{
			newInp[0] = '6';

		}

		else if (inp[0] == '6')
		{
			newInp[0] = '4';
		}

		return newInp;
	}

	protected List<char[]> ReverseInputs(List<char[]> origInputs)
	{
		var newInputs = new List<char[]>();
		foreach (char[] inp in origInputs)
		{
			newInputs.Add(ReverseInput(inp));
		}

		return newInputs;
	}


	//////////
	/// GATLINGS
	//////////
	///


	protected void AddGatling(char[] input, string state)
	{
		var newGatling = new NormalGatling
		{
			input = input,
			state = state
		};
		normalGatlings.Add(newGatling);
	}

	protected void AddGatling(char[] input, RequiredConditionCallback reqCall, string state)
	{
		var newGatling = new NormalGatling
		{
			input = input,
			state = state,
			reqCall = reqCall
		};
		normalGatlings.Add(newGatling);
	}

	protected void AddGatling(char[] input, string state, PostInputCallback postCall)
	{
		var newGatling = new NormalGatling
		{
			input = input,
			state = state,
			postCall = postCall
		};
		normalGatlings.Add(newGatling);
	}

	protected void AddGatling(char[] input, RequiredConditionCallback reqCall, string state, PostInputCallback postCall)
	{
		var newGatling = new NormalGatling
		{
			input = input,
			state = state,
			postCall = postCall,
			reqCall = reqCall
		};
		normalGatlings.Add(newGatling);
	}

	protected void AddKara(char[] input, string state)
	{
		var newGatling = new KaraGatling
		{
			input = input,
			state = state
		};
		karaGatlings.Add(newGatling);
	}

	protected void AddKara(char[] input, RequiredConditionCallback reqCall, string state)
	{
		var newGatling = new KaraGatling
		{
			input = input,
			state = state,
			reqCall = reqCall
		};
		karaGatlings.Add(newGatling);
	}

	protected void AddGatling(List<char[]> inputs, string state, bool preventMash = true, bool flipInputs = true)
	{
		var newGatling = new CommandGatling
		{
			inputs = inputs,
			state = state,
			preventMash = preventMash,
			flipInputs = flipInputs
		};
		commandGatlings.Add(newGatling);
	}

	protected void AddGatling(List<char[]> inputs, string state, PostInputCallback postCall, bool preventMash = true, bool flipInputs = true)
	{
		var newGatling = new CommandGatling
		{
			inputs = inputs,
			state = state,
			postCall = postCall,
			preventMash = preventMash,
			flipInputs = flipInputs
		};
		commandGatlings.Add(newGatling);
	}

	protected void AddGatling(List<char[]> inputs, RequiredConditionCallback reqCall, string state, PostInputCallback postCall, bool preventMash = true, bool flipInputs = true)
	{
		var newGatling = new CommandGatling
		{
			inputs = inputs,
			state = state,
			postCall = postCall,
			preventMash = preventMash,
			reqCall = reqCall,
			flipInputs = flipInputs
		};
		commandGatlings.Add(newGatling);
	}

	protected void AddRhythmGatling(List<char[]> inputs, string state)
	{
		var newGatling = new RhythmGatling
		{
			inputs = inputs,
			state = state
		};
		rhythmGatlings.Add(newGatling);
	}

	internal static List<List<char>> Permutations(List<char> chars)
	{
		var result = new List<List<char>>();
		foreach (char c in chars)
		{
			// move c from basket to current result
			var currRes = new List<char>() { c };
			var currBasket = chars.Where((char ch) =>  ch != c ).ToList();
			Helper(currRes, currBasket, result);
			
		}
		return result;
	}

	internal static void Helper(List<char> currRes, List<char> currBasket, List<List<char>> result)
	{
		if (currBasket.Count == 0)
		{
			result.Add(currRes.Select(x => x).ToList());
			return;
		}

		foreach (char c in currBasket)
		{
			currRes.Add(c);
			var nextBasket = currBasket.Where((char ch) => ch != c).ToList();
			Helper(currRes, nextBasket, result);
			currRes.RemoveAt(currRes.Count - 1);
		}
	}

	protected void AddNormals()
	{
		AddGatling(new[] { 'p', 'p' }, () => owner.CheckHeldKey('2'), "CrouchA");
		AddGatling(new[] { 'k', 'p' }, () => owner.CheckHeldKey('2'), "CrouchB");
		AddGatling(new[] { 's', 'p' }, () => owner.CheckHeldKey('2'), "CrouchC");
		AddGatling(new[] { 'p', 'p' }, "Jab");
		AddGatling(new[] { 'k', 'p' }, "Kick");
		AddGatling(new[] { 's', 'p' }, "Slash");
		AddGatling(new[] { 'b', 'p' }, "Jab");
	}

	protected void AddAirCommandNormals(List<Player.CommandNormal> commandNormals)
	{
		foreach (var cn in commandNormals)
		{
			AddGatling(new[] { cn.input, 'p' }, () => owner.facingRight && owner.CheckHeldKey(cn.heldKeys[0]), cn.state);
			AddGatling(new[] { cn.input, 'p' }, () => !owner.facingRight && owner.CheckHeldKey(cn.heldKeys[1]), cn.state);
		}
	}

	protected void AddCommandNormal(Player.CommandNormal cn)
	{
		if (!cn.crouching)
		{
			AddGatling(new[] { cn.input, 'p' },
				() => owner.facingRight && owner.CheckHeldKey(cn.heldKeys[0]) && !owner.CheckHeldKey('2') && (!cn.mustHadoukenCooldown || owner.hadoukenCooldownRemaining <= 0)
				, cn.state);
			AddGatling(new[] { cn.input, 'p' }, () => !owner.facingRight && owner.CheckHeldKey(cn.heldKeys[1]) && !owner.CheckHeldKey('2') && (!cn.mustHadoukenCooldown || owner.hadoukenCooldownRemaining <= 0), 
				cn.state);
		}
		else
		{
			AddGatling(new[] { cn.input, 'p' }, () => owner.facingRight && owner.CheckHeldKey(cn.heldKeys[0]) && owner.CheckHeldKey('2') && (!cn.mustHadoukenCooldown || owner.hadoukenCooldownRemaining <= 0), 
				cn.state);
			AddGatling(new[] { cn.input, 'p' }, () => !owner.facingRight && owner.CheckHeldKey(cn.heldKeys[1]) && owner.CheckHeldKey('2') && (!cn.mustHadoukenCooldown || owner.hadoukenCooldownRemaining <= 0), 
				cn.state);
		}

		
	}

	protected void AddCommandNormals(List<Player.CommandNormal> commandNormals)
	{
		foreach (var cn in commandNormals)
		{
			AddCommandNormal(cn);
		}
	}

	protected void AddEasyGroundSpecials()
	{
		
		AddGatling(new[] { 'a', 'p' },
		() => owner.CheckFlippableHeldKey('6') && owner.CheckHeldKey('s') && owner.TrySpendMeter() && owner.specialBreakFramesRemaining <= 0,
		owner.easySuper);
		AddGatling(new[] { 's', 'p' }, () =>
		owner.CheckFlippableHeldKey('6') && owner.CheckHeldKey('a') && owner.TrySpendMeter() && owner.specialBreakFramesRemaining <= 0,
		owner.easySuper);
		AddCommandNormals(owner.easyCommandSpecials);
		AddGatling(new[] { 'a', 'p' }, () => (owner.CheckNoDirectionsHeld()), owner.easySpecial);
	}

	protected void AddEasyAirSpecials()
	{
		AddGatling(new[] { 'a', 'p' }, () => owner.internalPos.y < Globals.MAXAIRDASHDEPTH, owner.easyAirSpecial);
	}

	protected void AddBurstKara(char key1, char key2)
	{
		AddKara(new char[] { key1, 'p' }, () => owner.CheckHeldKey(key2) && owner.TrySpendBurst(), "Burst");
		AddKara(new char[] { key2, 'p' }, () => owner.CheckHeldKey(key1) && owner.TrySpendBurst(), "Burst");
	}
	protected void AddSpecials(List<Player.Special> specials)
	{
		foreach (var special in specials)
		{
			AddGatling(special.inputs, special.state);
		}
	}

	protected void AddExSpecials(List<Player.Special> specials)
	{
		foreach (var special in specials)
		{
			AddGatling(special.inputs, () => owner.TrySpendMeter(), special.state, () => { }); // last function does nothing, I'm lazy...
		}
	}

	protected void AddRhythmSpecials(List<Player.Special> specials)
	{
		foreach (var special in specials)
		{
			AddRhythmGatling(special.inputs, special.state);
		}
	}



	protected void AddCancel(string cancelState)
	{
		foreach (var perm in Permutations(new List<char>() { 'p', 'k', 's' }))
		{
			AddGatling(new char[] { perm[0], 'p' },
				() => owner.CheckHeldKey(perm[1]) && owner.CheckHeldKey(perm[2]) && owner.TrySpendMeter(),
				cancelState,
				() => {
					owner.landingRecoveryFramesRemaining = 0;
					owner.GFXEvent("Cancel");
					owner.ScheduleEvent(EventScheduler.EventType.AUDIO, "RC", cancelState);
				});
		}
		
	}

	public virtual void HandleInput(char[] inputArr)
	{
		if (owner.health <= 0)
			return;
		foreach (CommandGatling comGat in commandGatlings)
		{

			char[] firstInp = comGat.inputs[comGat.inputs.Count - 1];
			if (!owner.facingRight && comGat.flipInputs)
			{
				firstInp = ReverseInput(firstInp);
			}

			if (Enumerable.SequenceEqual(firstInp, inputArr))
			{
				List<char[]> testedInputs = comGat.inputs;

				if (!owner.facingRight && comGat.flipInputs)
				{
					testedInputs = ReverseInputs(testedInputs);
				}


				if (owner.CheckBufferComplex(testedInputs))
				{
					if (comGat.reqCall != null) // check the required callback
					{
						if (!comGat.reqCall())
						{
							continue;
						}
					}

					if (comGat.preventMash && owner.CheckLastBufInput(firstInp)) // don't alow mashing the final input
					{
						continue;
					}

					if (comGat.postCall != null)
					{
						comGat.postCall();
					}

					// this gatling doesn't actually lead to a state (confusing, I know)
					if (comGat.state != "")
						EmitSignal(nameof(StateFinished), comGat.state);

					return;
				}
			}
		}
		foreach (NormalGatling normGat in normalGatlings)
		{
			if (normGat.input[0] == 'a' && owner.specialBreakFramesRemaining > 0)
				continue;
			char[] testInp = normGat.input;
			testInp = ReverseInput(testInp);
			if (Enumerable.SequenceEqual(normGat.input, inputArr))
			{
				if (normGat.reqCall != null)
				{
					if (!normGat.reqCall())
					{
						continue;
					}
				}

				normGat.postCall?.Invoke();

				if (normGat.state != "")
					EmitSignal(nameof(StateFinished), normGat.state);
				
				return;
			}
		}
	}

	/// <summary>
	/// Rhythm inputs need to be handled during hitstop
	/// </summary>
	/// <param name="inputArr"></param>
	public void HandleRhythmInput(char[] inputArr)
	{

		if (frameCount < 4 || owner.rhythmState != "") // better way to handle this probs
			return;

		foreach (RhythmGatling rhythmGatling in rhythmGatlings)
		{
			char[] firstInp = rhythmGatling.inputs[rhythmGatling.inputs.Count - 1];
			if (!owner.facingRight)
			{
				firstInp = ReverseInput(firstInp);
			}

			

			if (Enumerable.SequenceEqual(firstInp, inputArr))
			{

				List<char[]> testedInputs = rhythmGatling.inputs;

				if (!owner.facingRight)
				{
					testedInputs = ReverseInputs(testedInputs);
				}

				if (owner.CheckRhythmHeldKey(testedInputs[0][0]))
				{
					Globals.Log($"Properly holding key {testedInputs[0][0]}");

					if (rhythmGatling.reqCall != null) // check the required callback
					{
						if (!rhythmGatling.reqCall())
						{
							continue;
						}
					}

					if (rhythmGatling.preventMash && owner.CheckLastBufInput(firstInp)) // don't alow mashing the final input, fix this!!
					{
						continue;
					}
					

					if (rhythmGatling.postCall != null)
					{
						rhythmGatling.postCall();
					}
					owner.rhythmState = rhythmGatling.state;
					owner.EmitSignal(nameof(Player.RhythmHitTry), owner.Name);
					
					return;
				}
			}
		}
	}

	/// <summary>
	/// Called at the end of hitstop.  Stored in state because the input manager has access to it
	/// </summary>
	public void TryEnterRhythmState()
	{
		if (owner.rhythmStateConfirmed)
		{
			string enterState = String.Copy(owner.rhythmState);
			owner.rhythmState = "";
			owner.rhythmStateConfirmed = false;
			owner.CorrectGrounded(); // We may be in the air from a launching attack
			EmitSignal(nameof(StateFinished), enterState);

		}
	}

	/// <summary>
	/// If the current state should keep inputs in the unhandled buffer
	/// </summary>
	/// <returns></returns>
	public virtual bool DelayInputs()
	{
		return false;
	}

	public virtual bool CollisionActive()
	{
		return true;
	}

	public virtual bool IsProjectileInvuln()
	{
		return false;
	}

	public virtual bool IsGrabbable()
	{
		return true;
	}

	/// <summary>
	/// Just advances the frameCount, please make a base. call anyways though!
	/// </summary>
	public virtual void FrameAdvance()
	{
		frameCount++;
		if (slowdownSpeed != 0) SlowDown();

		if (frameCount >= 1)
		{
			TryBurst();
			
		}
	}

	public virtual void TryBurst()
	{
		if (owner.CheckHeldKeys(new[] { 'p', 'k', 'a' }))
		{
			if (!owner.TrySpendBurst()) return;
			owner.EmitSignal("Recovery", owner.Name);
			EmitSignal(nameof(StateFinished), "Burst");
		}
	}

	/// <summary>
	/// Called by parent
	/// </summary>
	public virtual void CheckHit()
	{

	}
	
	public void TryRhythm(){
		owner.EmitSignal("RhythmHitTry", owner.Name);
	}

	/// <summary>
	/// Get pushed by the opposing player from pure movement
	/// </summary>
	/// <param name="xVel"></param>
	public virtual void PushMovement(float xVel) 
	{
		owner.velocity.x = xVel / 2;
	}

	protected virtual void SlowDown()
	{
		if (Math.Abs(owner.velocity.x) <= slowdownSpeed)
		{
			owner.velocity.x = 0;
		}
		else
		{
			int mod = (owner.velocity.x < 0) ? -1 : 1;
			owner.velocity = new Vector2(owner.velocity.x - slowdownSpeed * mod, owner.velocity.y);

		}
	}

	/// <summary>
	/// Called if the other player is found in this hurtbox
	/// </summary>
	public virtual void InHurtbox(Vector2 collisionPnt)
	{


	}

	/// <summary>
	/// Determines which hitconfirm state to enter.  Note that Float.cs overrides this
	/// </summary>
	/// <param name="knockdown"></param>
	/// <param name="launch"></param>
	protected virtual void EnterHitState(bool knockdown, Vector2 launch, Vector2 collisionPnt, BaseAttack.EXTRAEFFECT effect, BaseAttack.GRAPHICEFFECT gfx)
	{
		GetNode<Node>("/root/Globals").EmitSignal(nameof(PlayerFXEmitted), collisionPnt, "hit", owner.OtherPlayerOnLeft());
		bool launchBool = false;

		if (effect == BaseAttack.EXTRAEFFECT.LAUNCHER)
		{
			owner.hasBeenLaunched = true;
			owner.EmitSignal(nameof(Player.GenericGFX), "Launch", owner.otherPlayer.Name);
		}

		owner.ComboUp();
		if (!(launch == Vector2.Zero))
		{
			owner.velocity = launch;
			launchBool = true;
		}

		HandleHitGFX(gfx);

		bool airState = (launchBool || !owner.grounded);

		if (effect == BaseAttack.EXTRAEFFECT.GROUNDBOUNCE)
		{
			EmitSignal(nameof(StateFinished), "GroundBounce");
		}
		else if (effect == BaseAttack.EXTRAEFFECT.WALLBOUNCE)
		{
			EmitSignal(nameof(StateFinished), "WallBounce");
		}

		else if (airState && !knockdown)
		{
			if (launch.y == 0)
			{
				owner.velocity.y = -400;
			}
			EmitSignal(nameof(StateFinished), "Float");
		}
		else if (airState && knockdown)
		{
			EmitSignal(nameof(StateFinished), "AirKnockdown");
		}
		else if (!airState && knockdown)
		{
			EmitSignal(nameof(StateFinished), "HitStun");

		}
		else if (!airState && effect == BaseAttack.EXTRAEFFECT.STAGGER)
		{
			EmitSignal(nameof(StateFinished), "Stagger");

		}
		else
		{
			EmitSignal(nameof(StateFinished), "HitStun");
		}
	}

	protected void HandleHitGFX(BaseAttack.GRAPHICEFFECT gfx)
	{
		if (gfx == BaseAttack.GRAPHICEFFECT.EXPLOSION)
		{
			owner.GFXEvent("Explosion");
		}
		else if (gfx == BaseAttack.GRAPHICEFFECT.PURPLE)
		{
			owner.GFXEvent("Purple");
		}
		else if (gfx == BaseAttack.GRAPHICEFFECT.SPARKS)
		{
			owner.ForceEvent(EventScheduler.EventType.AUDIO, "shock");
			owner.GFXEvent("Sparks");
		}
		else if (gfx == BaseAttack.GRAPHICEFFECT.SLASH)
		{
			owner.GFXEvent("Slash", owner.otherPlayer.CheckHurtRect() / 100);
		}
		else if (gfx == BaseAttack.GRAPHICEFFECT.ELECTROCUTE)
		{
			owner.ForceEvent(EventScheduler.EventType.AUDIO, "electricity");
			owner.electrocuted = true;
		}
	}

	public virtual GFXStates GetExtraGFXState()
	{
		return GFXStates.NONE;
	}

	public virtual void HitWall()
	{

	}


	protected virtual void EnterBlockState(string stateName, Vector2 collisionPnt, int blockStop)
	{
		
		GetNode<Node>("/root/Globals").EmitSignal(nameof(PlayerFXEmitted), collisionPnt, "block", owner.OtherPlayerOnLeft());
		
		EmitSignal(nameof(StateFinished), stateName);
		owner.EmitSignal("HitConfirm", blockStop);

	}

	protected virtual void ReceiveHighBlock(Globals.AttackDetails details, bool leftBlock, bool rightBlock, bool anyBlock)
    {
        if (owner.CheckOverrideBlock())
				EnterBlockState("Block", details.collisionPnt, details.hitStop);
		else if (!owner.CheckHeldKey('2'))
		{
			if (rightBlock || leftBlock || anyBlock)
			{
				EnterBlockState("Block", details.collisionPnt, details.hitStop);
			}
			else
			{
				EnterHitState(details.knockdown, details.opponentLaunch, details.collisionPnt, details.effect, details.graphicFX);
			}
		}
		else
		{
			if (owner.CheckFlippableHeldKey('4'))
				owner.EmitSignal("Mixup", owner.Name);
			EnterHitState(details.knockdown, details.opponentLaunch, details.collisionPnt, details.effect, details.graphicFX);
		}
    }

	protected virtual void ReceiveMidBlock(Globals.AttackDetails details, bool leftBlock, bool rightBlock, bool anyBlock)
    {
        if (owner.CheckOverrideBlock())
			EnterBlockState("Block", details.collisionPnt, details.hitStop);

		else if (rightBlock || leftBlock || anyBlock)
		{
			if (owner.CheckHeldKey('2') && owner.grounded)
				EnterBlockState("CrouchBlock", details.collisionPnt, details.hitStop);
			else
				EnterBlockState("Block", details.collisionPnt, details.hitStop);
		}
		else 
		{
			EnterHitState(details.knockdown, details.opponentLaunch, details.collisionPnt, details.effect, details.graphicFX);
		}
    }

	public virtual void ReceiveHit(Globals.AttackDetails details)
	{
		owner.velocity = new Vector2(0, 0);
		switch (details.dir)
		{
			case BaseAttack.ATTACKDIR.RIGHT:
				break;
			case BaseAttack.ATTACKDIR.LEFT:
				details.opponentLaunch.x *= -1;
				details.hitPush *= -1;
				break;
			case BaseAttack.ATTACKDIR.EQUAL:
				details.opponentLaunch.x = 0;
				details.hitPush = 0;
				break;
		}
		

		owner.hitPushRemaining = details.hitPush;

		if (owner.velocity.y < 0)
		{
			owner.grounded = false;
		}

		bool rightBlock = details.dir == BaseAttack.ATTACKDIR.RIGHT && owner.CheckHeldKey('6');
		bool leftBlock = details.dir == BaseAttack.ATTACKDIR.LEFT && owner.CheckHeldKey('4');
		bool anyBlock = details.dir == BaseAttack.ATTACKDIR.EQUAL && (owner.CheckHeldKey('4') || owner.CheckHeldKey('6'));

		if (details.height == HEIGHT.HIGH) 
		{
			ReceiveHighBlock(details, rightBlock, leftBlock, anyBlock);
			
		}
		else if (details.height == HEIGHT.LOW) 
		{
			if (owner.CheckOverrideBlock() && owner.grounded)
				EnterBlockState("CrouchBlock", details.collisionPnt, details.hitStop);
			else if (owner.CheckHeldKey('2') && owner.grounded)
			{
				if (rightBlock || leftBlock || anyBlock)
				{
					EnterBlockState("CrouchBlock", details.collisionPnt, details.hitStop);
				}
				else
				{
					EnterHitState(details.knockdown, details.opponentLaunch, details.collisionPnt, details.effect, details.graphicFX);
				}
			}
			else
			{
				if (owner.CheckFlippableHeldKey('4'))
					owner.EmitSignal("Mixup", owner.Name);
				EnterHitState(details.knockdown, details.opponentLaunch, details.collisionPnt, details.effect, details.graphicFX);
			}
		}
		else
		{
			ReceiveMidBlock(details, rightBlock, leftBlock, anyBlock);
		}
	}


	protected virtual void ReceiveHitNoBlock(Globals.AttackDetails details)
	{


		bool launchBool = false;
		switch (details.dir)
		{
			case BaseAttack.ATTACKDIR.RIGHT:
				break;
			case BaseAttack.ATTACKDIR.LEFT:
				details.opponentLaunch.x *= -1;
				details.hitPush *= -1;
				break;
			case BaseAttack.ATTACKDIR.EQUAL:
				details.opponentLaunch.x = 0;
				details.hitPush = 0;
				break;
		}
		owner.hitPushRemaining = details.hitPush;
		owner.velocity = details.opponentLaunch;
		if (!(details.opponentLaunch == Vector2.Zero))
		{
			owner.velocity = details.opponentLaunch;
			launchBool = true;
		}

		if (owner.velocity.y < 0) // make sure the player is registered as in the air if launched 
		{
			owner.grounded = false;
		}


		EnterHitState(details.knockdown, details.opponentLaunch, details.collisionPnt, details.effect, details.graphicFX);

	}

	public virtual void ReceiveStunDamage(Globals.AttackDetails details)
	{
		Globals.Log($"Receiving damage {details.dmg}");
		int hitProration = details.prorationLevel;
		if (owner.combo == 1)
        {
			if (hitProration > 0)
				hitProration *= 3;
			else
				hitProration = 0;
        }


		stunRemaining = details.hitStun;

		var prorActual = details.ignoreProration ? 24 : owner.proration;
		var fixDmg = new Fix64(details.dmg * prorActual);
		var comboPror = new Fix64(1) + new Fix64(owner.combo) / new Fix64(5);;

		if (!details.ignoreProration)
        {
            fixDmg /= comboPror;
        }
			
		
		owner.DeductHealth((int)fixDmg + 10);
		owner.Prorate(hitProration);
	}

	public virtual void receiveStun(int hitStun, int blockStun)
	{
		stunRemaining = hitStun;
	}


	public virtual bool LevelUp()
	{
		return false;
	}

	public void ResetTerminalVelocity()
	{
		owner.terminalVelocity = owner.standardTerminalVelocity;
	}

	/// <summary>
	/// Certain states will ignore changed terminal velocities at times, such as groundbounce
	/// </summary>
	public virtual int CheckTerminalVelocity()
	{
		return owner.terminalVelocity;
	}

	public virtual void TrySpecialBreak()
	{

	}
}
