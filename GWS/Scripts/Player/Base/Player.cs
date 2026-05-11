using FixedMath.NET;
using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Player : Node2D
{
	public State currentState; //The current state governs key aspects of input handling, movement, animation etc.
	public Player otherPlayer; //I know I shouldn't do this, but it makes my life so much easier...

	public const int MAXPLAYERDIST = 30000;

	private const string ExplosionGfxString = "Explosion";
	private const string PurpleGfxString = "Purple";
	private const string FireAnimString = "fire";
	private const string PurpleAnimString = "purple";

	[Export]
	public int speed = 400;

	[Export]
	public int dashSpeed = 700;

	[Export]
	public int backDashSpeed = 700;

	[Export]
	public int airDashSpeed = 800;

	[Export]
	public int airBackdashSpeed = 500;

	[Export]
	public int accel = 40;

	[Export]
	public int jumpForce = 800;

	[Export]
	public int superJumpForce = 1100;

	[Export]
	public int gravity = 50;

	public int standardTerminalVelocity = 1100;

	[Export]
	public bool dummy = false; //you can use this for testing with a dummy

	[Export]
	public int hitPushSpeed = 300;

	[Export]
	public int damageModInt = 10; // divide by ten to get true modifier

	[Export]
	public int defaultInitialProrate = 0; // applied on every first hit

	[Export]
	public int damageDealtModifierInt = 10; // divided by ten to get the actual modifier

	private Fix64 damageDealtMod;

	private Fix64 damageMod;

	[Export]
	public bool debugPress = false;

	[Export]
	public string debugKeys = "6";

	[Export]
	public Resource palette;

	[Export]
	public Resource greyPalette;

	protected string charName;

	[Export(PropertyHint.Range, "0,3,0")]
	public int colorScheme;

	private InputHandler inputHandler;

	/// <summary>
	/// stores states for which their is a specific object for this player.
	/// </summary>
	private HashSet<string> altState = new HashSet<string>();

	/// <summary>
	/// Certain states will automatically setup gatlings if they are in this list
	/// </summary>
	public List<CommandNormal> commandNormals = new List<CommandNormal>();
	public List<CommandNormal> airCommandNormals = new List<CommandNormal>();
	public List<Special> groundSpecials = new List<Special>();
	public List<Special> airSpecials = new List<Special>();
	public List<Special> dashSpecials = new List<Special>();
	public List<Special> groundExSpecials = new List<Special>();
	public List<Special> airExSpecials = new List<Special>();
	public List<CommandNormal> easyCommandSpecials = new List<CommandNormal>();
	public string easySpecial;
	public string easyAirSpecial;
	public string easySuper;

	/// <summary>
	/// States that cannot be cancelled into grab, for reasons...
	/// </summary>
	
	public HashSet<string> noGrabLastStates = new HashSet<string>() { "Jab", "Run", "PreRun", "CrouchA", "PostRun" };
	public HashSet<string> noGrabStates = new HashSet<string>() { "Super" };

	public delegate void NegEdgeCallback(char releasedkey);
	public NegEdgeCallback negEdgeCallback = (char c) => { };

	///
	// All of these will be stored in gamestate
	///

	
	public int hitPushRemaining = 0; // stores the hitpush yet to be applied
	public Vector2 internalPos; // this will be stored at 100x the actual rendered position, to allow greater resolution
	public int health = 1800;
	private int meter = 0;
	public Vector2 velocity = Vector2.Zero;
	public int terminalVelocity = 1100; // See CheckTerminalVelocity for details.  This is never directly accessed by state
	public bool facingRight = true;
	public bool grounded;
	public int combo = 0;
	public int proration = 24;
	public bool canDoubleJump;
	public bool canAirDash;
	public int invulnFrames = 0;
	public int airDashFrames = 0;
	public int grabInvulnFrames = 0;
	public string lastStateName = "Idle";
	public int counterStopFrames = 0;
	public bool canGroundbounce = true;
	public int specialBreakFramesRemaining = 0;
	public int landingRecoveryFramesRemaining = 0;
	public int lastPressedDownFrame = 0;
	public int lastPressedUpFrame = 0;
	public int lastPressedDashFrame = 0;
	public bool electrocuted = false;
	public bool wasOTGHit = false;
	public int burstMeter = 100;
	public int hadoukenCooldownRemaining = 0;
	public int backdashCooldownRemaining = 0;
	public int meterGainCooldownRemaining = 0;
	public bool hasBeenLaunched = false;
	public bool hasDoubleOrSuperJumped = false;
	public bool hasBeenSpiked = false;
	public bool hasHurtboxActive = false;
	protected int[] charSpecificData = new int[4];

	public PlayerState pState = new PlayerState();


	public bool trainingControlledPlayer;
	public bool aiControlled = false;
	private const int MAXPLUSFRAMES = 8;
	private int currPlusFrameIndex;
	private PlusFrames[] plusFrames = new PlusFrames[MAXPLUSFRAMES];
	private Random aiRng = new Random(); // ONLY FOR AI, NO RNG IN THE GAME PLEASE


	/// <summary>
	/// Contains all vital data for saving gamestate
	/// </summary>
	[Serializable]
	public unsafe struct PlayerState
	{
		public fixed char inBuf2[112];
		public int inBuf2Count;
		public fixed char hitStopInputs[40];
		public int hitStopInputsCount;
		public fixed char heldKeys[12];

		public int inBuf2Timer;
		public int currentStateIndex;
		public int lastStateIndex;
		public int animationIndex;
		public fixed int stateData[6];
		public bool canDoubleJump;
		public bool canAirDash;
		public bool hitConnect;
		public int frameCount;
		public int stunRemaining;
		public int hitPushRemaining;
		public bool flipH;
		public int health;
		public int meter;
		public int positionx;
		public int positiony;
		public int velocityx;
		public int velocityy;

		public int terminalVelocity;
		public bool facingRight;
		public bool touchingWall;
		public bool grounded;
		public int combo;
		public int proration;
		public int animationCursor;
		public int lastFrameInputs;
		public int invulnFrames;
		public int airDashFrames;
		public int grabInvulnFrames;
		public int counterStopFrames;
		public bool canGroundbounce;
		public int specialBreakFramesRemaining;
		public int landingRecoveryFramesRemaining;
		public int lastPressedDownFrame;
		public int lastPressedDashFrame;
		public int lastPressedUpFrame;
		public bool electrocuted;
		public bool wasOTGHit;
		public int burstMeter;
		public int backdashCooldownRemaining;
		public int hadoukenCooldownRemaining;
		public int meterGainCooldownRemaining;
		public bool hasBeenLaunched;
		public bool hasDoubleOrSuperJumped;
		public bool hasHurtboxActive;
		public bool hasBeenSpiked;
		public fixed int charSpecificData[4];
		public int safety;

	}

	/// <summary>
	/// Info about a special
	/// </summary>
	public struct Special
	{
		public InputContainer inputs;
		public string state;

		public Special(InputContainer inputsList, string newState) 
		{
			inputs = inputsList;
			state = newState;
		}
	}

	public struct CommandNormal
	{
		public List<char> heldKeys;
		public char input;
		public string state;
		public bool crouching;
		public bool mustHadoukenCooldown;

		public CommandNormal(List<char> heldKeys, char input, string newState, bool crouching=false, bool mustHadoukenCooldown=false)
		{
			this.heldKeys = heldKeys;
			this.input = input;
			this.state = newState;
			this.crouching = crouching;
			this.mustHadoukenCooldown = mustHadoukenCooldown;
		}
	}

	// components of a received attack
	public bool wasHitThisFrame = false;
	private Globals.AttackDetails receivedHit;
	private Globals.AttackDetails receivedCHit;

	// Box colors
	private Color hitColor = new Color(0, 0, 255, 0.5f);
	private Color hurtColor = new Color(255, 0, 0, 0.5f);
	private Color colColor = new Color(0, 255, 0, 0.5f);
	private Color grabColor = new Color(0, 0, 0, 0.5f);

	// States
	protected Godot.Collections.Dictionary<string, State> allStateDict = new Godot.Collections.Dictionary<string, State>();
	protected Godot.Collections.Array<string> allStates = new Godot.Collections.Array<string>();
	protected Godot.Collections.Array<string> allAnimations = new Godot.Collections.Array<string>();
	protected Godot.Collections.Dictionary<string, State> altStateDict = new Godot.Collections.Dictionary<string, State>();
	public string idleString = "Idle";
	public string grabString = "Grab";
	public string grabbedString = "Grabbed";
	private const string KnockdownString = "Knockdown";
	private const string AirKnockdownString = "AirKnockdown";
	private const string JiveString = "Jive";

	// Temp arrays
	public Rect2[] tempHurtboxArray = new Rect2[3];
	public Rect2[] tempHitboxArray = new Rect2[3];

	// Sub nodes
	public Position2D grabPos;
	public Godot.Collections.Array<CollisionShape2D> hitBoxes;
	public Godot.Collections.Array<CollisionShape2D> hurtBoxes;
	protected Area2D hitBoxParent;
	protected Area2D hurtBoxParent;
	private CollisionShape2D colBox;
	public AnimationPlayer animationPlayer;
	public Sprite sprite;
	private EventScheduler eventSched;
	private GFXHandler gfxHand;
	private Label debugPos;
	private Node2D electricity;
	private Node stateTree;
	private ShieldFX shield;
	private CPUParticles2D shieldEmission;

	[Export]
	public PackedScene plusFrameTextScene;

	// Sprites
	public Sprite mainSprite;
	public Godot.AnimationPlayer spriteAnim;
	public Sprite behindSprite;
	public Sprite frontSprite;

	private bool hasEnterTree = false;
	public override void _EnterTree()
	{
		base._EnterTree();
		if (hasEnterTree)
			return;
		hasEnterTree = true;
	}

	public override void _Ready()
	{
		stateTree = GetNode<Node>("StateTree");
		damageMod = new Fix64(damageModInt) / new Fix64(10);
		damageDealtMod = new Fix64(damageDealtModifierInt) / new Fix64(10);
		mainSprite = GetNode<Sprite>("Sprite");
		spriteAnim = GetNode<Godot.AnimationPlayer>("Sprite/SpriteModulations");
		behindSprite = GetNode<Sprite>("SpriteBehind");
		frontSprite = GetNode<Sprite>("SpriteFront");
		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		animationPlayer.Setup();

		grabPos = GetNode<Position2D>("GrabPos");
		hitBoxParent = GetNode<Area2D>("HitBoxes");
		hitBoxes = new Godot.Collections.Array<CollisionShape2D>();
		foreach (CollisionShape2D child in hitBoxParent.GetChildren()){
			hitBoxes.Add(child);
		}
		hurtBoxParent = GetNode<Area2D>("HurtBoxes");
		hurtBoxes = new Godot.Collections.Array<CollisionShape2D>();
		foreach (CollisionShape2D child in hurtBoxParent.GetChildren()){
			hurtBoxes.Add(child);
		}
		colBox = GetNode<CollisionShape2D>("CollisionBox");
		
		sprite = GetNode<Sprite>("Sprite");
		eventSched = GetNode<EventScheduler>("EventScheduler");
		gfxHand = GetNode<GFXHandler>("GFXHandler");
		debugPos = GetNode<Label>("DebugPos");

		shield = GetNode<ShieldFX>("Shield");
		shieldEmission = shield.GetNode<CPUParticles2D>("ShieldHit");

		electricity = (Node2D)GetNode("ElectricShock");
		foreach (CollisionShape2D box in hitBoxes)
		{
			box.Shape = new RectangleShape2D();
		}
		foreach (CollisionShape2D box in hurtBoxes)
		{
			box.Shape = new RectangleShape2D();
		}

		inputHandler = new InputHandler();
		Godot.Collections.Array allStates = stateTree.GetChildren();
		string none = "None";
		foreach (Node state in allStates) 
		{
			this.allStates.Add(state.Name);
			if (((State)state).animationName != none)
			{ 
				allAnimations.Add(((State)state).animationName);
			}
			allStateDict[state.Name] = (State)state;
			if (altState.Contains(state.Name))
				altStateDict.Add(state.Name, (State)state);
		}
		currentState = allStateDict[idleString];
		ChangeState(idleString);

		for (int i = 0; i < MAXPLUSFRAMES; i++)
		{
			plusFrames[i] = plusFrameTextScene.Instance() as PlusFrames;
			plusFrames[i].Visible = false;
			AddChild(plusFrames[i]);
		}

		if (debugPress)
		{
			foreach (char key in debugKeys)
			{
				inputHandler.heldKeys.Add(key);
			}
			
		}

		terminalVelocity = standardTerminalVelocity;

		ColorSprite();
		Globals.EmitSignal(Globals.PlayerSignal.MeterChanged, Name, 0);
	}

	public virtual void Init()
	{
		ColorSprite();
		Show();
		SetProcess(true);
	}

	public virtual void Reset()
	{
		foreach (Node state in allStateDict.Values)
			((State)state).Reset();
		ResetComboAndProration();
		ChangeState(idleString);
		velocity = Vector2.Zero;
		electrocuted = false;
		hitPushRemaining = 0;
		lastPressedDownFrame = 0;
		lastPressedDashFrame = 0;
		lastPressedUpFrame = 0;

		if (Globals.mode == Globals.Mode.TRAINING || Globals.mode == Globals.Mode.TUTORIAL)
		{
			meter = 10000;
			burstMeter = 100;
			Globals.EmitSignal(Globals.PlayerSignal.BurstSet, Name, burstMeter);
		}
		else
		{
			meter = 0;
		}
			
		inputHandler.Reset();
	}


	public unsafe PlayerState GetState()
	{
		fixed (char* p = pState.inBuf2)
		{
			pState.inBuf2Count = inputHandler.inBuf2.GetState(p);
		}
		fixed (char* p = pState.hitStopInputs)
		{
			pState.hitStopInputsCount = inputHandler.hitStopInputs.GetState(p);
		}
		fixed (char* p = pState.heldKeys)
		{
			inputHandler.heldKeys.GetState(p);
		}

		pState.inBuf2Timer = inputHandler.inBuf2Timer;

		pState.canDoubleJump = canDoubleJump;
		pState.canAirDash = canAirDash;

		// currentState.Name
		string name = currentState.Name;
		pState.currentStateIndex = allStates.IndexOf(name);

		// lastStateName
		pState.lastStateIndex = allStates.IndexOf(lastStateName);

		// animationPlayer.AssignedAnimation
		pState.animationIndex = allAnimations.IndexOf(animationPlayer.AssignedAnimation);

		
		var currentStateData = currentState.Save();
		fixed (int* p = pState.stateData)
		{
			for (int i = 0; i < currentStateData.Length; i++)
				p[i] = currentStateData[i];
		}
		pState.frameCount = currentState.frameCount;
		pState.hitConnect = currentState.hitConnect;
		pState.stunRemaining = currentState.stunRemaining;
		pState.flipH = sprite.FlipH;
		pState.hitPushRemaining = hitPushRemaining;
		pState.health = health;
		pState.meter = meter;
		
		pState.positionx = (int)internalPos.x;
		pState.positiony = (int)internalPos.y;
		pState.velocityx = (int)velocity.x;
		pState.velocityy = (int)velocity.y;
		
		pState.animationCursor = animationPlayer.cursor;
		pState.terminalVelocity = terminalVelocity;
		
		pState.facingRight = facingRight;
		pState.grounded = grounded;
		pState.combo = combo;
		pState.proration = proration;
		pState.lastFrameInputs = inputHandler.lastFrameInputs;
		pState.invulnFrames = invulnFrames;
		pState.airDashFrames = airDashFrames;
		pState.grabInvulnFrames = grabInvulnFrames;
		
		pState.counterStopFrames = counterStopFrames;
		pState.canGroundbounce = canGroundbounce;
		pState.electrocuted = electrocuted;
		var charSpecificData = GetStateCharSpecific();

		fixed (int* p = pState.charSpecificData)
		{
			for (int i = 0; i < charSpecificData.Length; i++)
				p[i] = charSpecificData[i];
		}
		pState.wasOTGHit = wasOTGHit;
		pState.burstMeter = burstMeter;

		pState.backdashCooldownRemaining = backdashCooldownRemaining;
		pState.hadoukenCooldownRemaining = hadoukenCooldownRemaining;
			
		pState.specialBreakFramesRemaining = specialBreakFramesRemaining;
		pState.landingRecoveryFramesRemaining = landingRecoveryFramesRemaining;
		pState.lastPressedDownFrame = lastPressedDownFrame;
		pState.lastPressedDashFrame = lastPressedDashFrame;
		pState.lastPressedUpFrame = lastPressedUpFrame;
		pState.hasBeenLaunched = hasBeenLaunched;
		pState.hasDoubleOrSuperJumped = hasDoubleOrSuperJumped;
		pState.hasBeenSpiked = hasBeenSpiked;
		pState.meterGainCooldownRemaining = meterGainCooldownRemaining;
		pState.hasHurtboxActive = hasHurtboxActive;
		pState.safety = 3;
		
		return pState;
	}

	protected virtual int[] GetStateCharSpecific()
	{
		return charSpecificData;
	}

	protected virtual void SetStateCharSpecific(int[] charSpecificData)
	{

	}

	private int[] tempStateData = new int[4];
	public unsafe void SetState(PlayerState pState)
	{
		if (pState.safety != 3)
			GD.Print("SAFETY BROKEN");

		inputHandler.inBuf2.SetState(pState.inBuf2Count, pState.inBuf2);
		inputHandler.hitStopInputs.SetState(pState.hitStopInputsCount, pState.hitStopInputs);
		inputHandler.heldKeys.SetState(pState.heldKeys);


		inputHandler.inBuf2Timer = pState.inBuf2Timer;

		string stateName = allStates[pState.currentStateIndex];
		currentState = allStateDict[stateName];
		lastStateName = allStates[pState.lastStateIndex];
		inputHandler.playerState = currentState;
		currentState.hitConnect = pState.hitConnect;
		currentState.frameCount = pState.frameCount;
		for (int i = 0; i < 4; i++)
			tempStateData[i] = pState.stateData[i];
		currentState.Load(tempStateData);
		string animation = allAnimations[pState.animationIndex];
		animationPlayer.SetAnimationAndFrame(animation, pState.animationCursor);
		currentState.stunRemaining = pState.stunRemaining;
		sprite.FlipH = pState.flipH;
		hitPushRemaining = pState.hitPushRemaining;
		canDoubleJump = pState.canDoubleJump;
		canAirDash = pState.canAirDash;
		health = pState.health;
		meter = pState.meter;
		terminalVelocity = pState.terminalVelocity;
		Globals.EmitSignal(Globals.PlayerSignal.HealthSet, Name, health);
		Globals.EmitSignal(Globals.PlayerSignal.MeterChanged, Name, meter);
		internalPos.x = pState.positionx;
		internalPos.y = pState.positiony;
		lastPressedDownFrame = pState.lastPressedDownFrame;
		lastPressedDashFrame = pState.lastPressedDashFrame;
		lastPressedUpFrame = pState.lastPressedUpFrame;
		hasDoubleOrSuperJumped = pState.hasDoubleOrSuperJumped;
		electrocuted = pState.electrocuted;
		wasOTGHit = pState.wasOTGHit;
		hasHurtboxActive = pState.hasHurtboxActive;
		
		velocity.x = pState.velocityx;
		velocity.y = pState.velocityy;
		facingRight = pState.facingRight;
		grounded = pState.grounded;
		combo = pState.combo;
		proration = pState.proration;
		inputHandler.lastFrameInputs = pState.lastFrameInputs;
		invulnFrames = pState.invulnFrames;
		airDashFrames = pState.airDashFrames;
		grabInvulnFrames = pState.grabInvulnFrames;
		Globals.EmitSignal(Globals.PlayerSignal.ComboSet, Name, combo);
		counterStopFrames = pState.counterStopFrames;
		canGroundbounce = pState.canGroundbounce;
		for (int i = 0; i < charSpecificData.Length; i++)
			charSpecificData[i] = pState.charSpecificData[i];
		SetStateCharSpecific(charSpecificData);
		if (pState.specialBreakFramesRemaining > 0 && specialBreakFramesRemaining == 0)
			GreySprite();
		else if (pState.specialBreakFramesRemaining == 0 && specialBreakFramesRemaining > 0)
			ColorSprite();
		specialBreakFramesRemaining = pState.specialBreakFramesRemaining;
		landingRecoveryFramesRemaining = pState.landingRecoveryFramesRemaining;
		burstMeter = pState.burstMeter;
		hasBeenSpiked = pState.hasBeenSpiked;
		hadoukenCooldownRemaining = pState.hadoukenCooldownRemaining;
		backdashCooldownRemaining = pState.backdashCooldownRemaining;
		meterGainCooldownRemaining = pState.meterGainCooldownRemaining;
		hasBeenLaunched = pState.hasBeenLaunched;
		Globals.EmitSignal(Globals.PlayerSignal.BurstSet, Name, burstMeter);
	}

	/// <summary>
	/// Called to delete graphic effects if necessitated by a rollback
	/// </summary>
	/// <param name="frame"></param>
	public void Rollback(int frame)
	{
		gfxHand.Rollback(frame);
	}

	public void ChangeIntPositionRel(int x, int y)
	{
		int oldx = (int)internalPos.x;
		int oldy = (int)internalPos.y;
		internalPos.x = oldx + x;
		internalPos.y = oldy + y;
	}

	public void ChangeIntPositionAbs(int x, int y)
	{
		internalPos.x = x;
		internalPos.y = y;
	}

	public void PrintBuffer()
	{
		GD.Print("Buffer ----");
		foreach (var inp in inputHandler.inBuf2)
		{
			GD.Print(string.Join(",", inp));
		}
		GD.Print("----");
		
	}

	public void ClearInputs()
	{
		inputHandler.clearUnhandled = true;
	}

	/// <summary>
	/// Deals with unhandled inputs, the input buffer, and a hitstop buffer.  Subject to constant change
	/// </summary>
	private class InputHandler 
	{
		public InputContainer inBuf2 = new InputContainer(56);
		public InputContainer hitStopInputs = new InputContainer(20);

		//private List<char> order = new List<char>() { 's', 'k', 'p', '6', '4', ''}; consider input priority later

		public int inBuf2TimerMax = 5;
		public int inBuf2Timer = 5;
		public HeldKeys heldKeys = new HeldKeys(12);
		public State playerState;
		/// <summary>
		/// Used for checking if a key has been pressed or released
		/// </summary>
		public int lastFrameInputs;
		/// <summary>
		/// We need to not evaluate further inputs after beginning some action
		/// </summary>
		public bool clearUnhandled;

		public void Reset()
		{
			heldKeys.Clear();
			hitStopInputs.Clear();
			inBuf2.Clear();
			inBuf2Timer = inBuf2TimerMax;
		}

		private void BufAddInput(InputContainer.CharPair input)
		{
			inBuf2Timer = inBuf2TimerMax;
			if (inBuf2.Count >= 56)
				inBuf2.Clear();
			inBuf2.Add(input);
			
		}

		private void BufTimerDecrement()
		{
			if (inBuf2Timer > 0)
			{
				inBuf2Timer--;
			}
			else
			{
				inBuf2.Clear();
			}
		}
		private void AddHitStopBuffer(InputContainer unhandledInputs)
		{
			for (int i = 0; i < unhandledInputs.Count; i++){
				var inputArr = unhandledInputs.Get(i);
				hitStopInputs.Add(inputArr);
				if (hitStopInputs.Count == hitStopInputs.Capacity)
					hitStopInputs.Clear();
			}
		}
		private InputContainer unhandledInputs = new InputContainer(40);
		private InputContainer ConvertInputs(int inputs)
		{
			unhandledInputs.Clear();
			if ((inputs & 1) != 0 && (lastFrameInputs & 1) == 0)
			{
				unhandledInputs.Add(Globals.UPPRESS);
				playerState.owner.lastPressedUpFrame = Globals.frame;
			}
			else if ((inputs & 1) == 0 && (lastFrameInputs & 1) != 0)
			{
				unhandledInputs.Add(Globals.UPREL);
			}

			if ((inputs & 2) != 0 && (lastFrameInputs & 2) == 0)
			{
				unhandledInputs.Add(Globals.DOWNPRESS);
				playerState.owner.lastPressedDownFrame = Globals.frame;
			}
			else if ((inputs & 2) == 0 && (lastFrameInputs & 2) != 0)
			{
				unhandledInputs.Add(Globals.DOWNREL);
			}

			if ((inputs & 4) != 0 && (lastFrameInputs & 4) == 0)
			{
				unhandledInputs.Add(Globals.RIGHTPRESS);
			}
			else if ((inputs & 4) == 0 && (lastFrameInputs & 4) != 0)
			{
				unhandledInputs.Add(Globals.RIGHTREL);
			}

			if ((inputs & 8) != 0 && (lastFrameInputs & 8) == 0)
			{
				unhandledInputs.Add(Globals.LEFTPRESS);
			}
			else if ((inputs & 8) == 0 && (lastFrameInputs & 8) != 0)
			{
				unhandledInputs.Add(Globals.LEFTREL);
			}

			if ((inputs & 16) != 0 && (lastFrameInputs & 16) == 0)
			{
				unhandledInputs.Add(Globals.JABPRESS);
			}
			else if ((inputs & 16) == 0 && (lastFrameInputs & 16) != 0)
			{
				unhandledInputs.Add(Globals.JABREL);
			}

			if ((inputs & 32) != 0 && (lastFrameInputs & 32) == 0)
			{
				unhandledInputs.Add(Globals.KICKPRESS);
			}
			else if ((inputs & 32) == 0 && (lastFrameInputs & 32) != 0)
			{
				unhandledInputs.Add(Globals.KICKREL);
			}

			if ((inputs & 64) != 0 && (lastFrameInputs & 64) == 0)
			{
				unhandledInputs.Add(Globals.SLASHPRESS);
			}
			else if ((inputs & 64) == 0 && (lastFrameInputs & 64) != 0)
			{
				unhandledInputs.Add(Globals.SLASHREL);
			}

			if ((inputs & 128) != 0 && (lastFrameInputs & 128) == 0)
			{
				unhandledInputs.Add(Globals.SPECIALPRESS);
			}
			else if ((inputs & 128) == 0 && (lastFrameInputs & 128) != 0)
			{
				unhandledInputs.Add(Globals.SPECIALREL);
			}

			if ((inputs & 256) != 0 && (lastFrameInputs & 256) == 0)
			{
				unhandledInputs.Add(Globals.STRINGPRESS);
			}
			else if ((inputs & 256) == 0 && (lastFrameInputs & 256) != 0)
			{
				unhandledInputs.Add(Globals.STRINGREL);
			}

			if ((inputs & 512) != 0 && (lastFrameInputs & 512) == 0)
			{
				unhandledInputs.Add(Globals.DASHPRESS);
				playerState.owner.lastPressedDashFrame = Globals.frame;
			}
			else if ((inputs & 512) == 0 && (lastFrameInputs & 512) != 0)
			{
				unhandledInputs.Add(Globals.DASHREL);
			}


			return unhandledInputs;
		}

		

		public virtual void FrameAdvance(int hitStop, int inputs, NegEdgeCallback negEdgeCallback)
		{
			InputContainer unhandledInputs = ConvertInputs(inputs);
			lastFrameInputs = inputs;
			
			for (int i = 0; i < unhandledInputs.Count; i++)
			{
				var inputArr = unhandledInputs.Get(i);
				if (inputArr.A == 'a')
				{
					playerState.TrySpecialBreak();
				}
				BufAddInput(inputArr);
			}
			
			for (int i = 0; i < unhandledInputs.Count; i++)
			{
				var inputArr = unhandledInputs.Get(i);
				if (inputArr.B == 'p')
				{
					heldKeys.Add(inputArr.A);

				}
				
				else if (inputArr.B == 'r')
				{
					negEdgeCallback(inputArr.A);
					heldKeys.Remove(inputArr.A);
				}
			}	
			if (hitStop > 0 || playerState.DelayInputs()) // delay the handling of inputs until after hitstop ends
			{
				AddHitStopBuffer(unhandledInputs);
				return;
			}
			if (unhandledInputs.Count == 0)
				BufTimerDecrement();
			if (hitStopInputs.Count > 0)
			{
				unhandledInputs.Prepend(hitStopInputs);
			}
			
			hitStopInputs.Clear();
			for (int i = 0; i < unhandledInputs.Count; i++)
			{
				playerState.HandleInput(unhandledInputs.Get(i));
				if (clearUnhandled)
					break;
			}
			clearUnhandled = false;
			unhandledInputs.Clear();
		}

		public InputContainer GetBuffer() 
		{
			return inBuf2;
		}

		public InputContainer GetHitStopBuffer()
		{
			return hitStopInputs;
		}

		public string DumpHitStopBuffer()
		{
			List<char> buf = new List<char>();
			foreach (var input in hitStopInputs)
			{
				buf.Add(input.A);
				buf.Add(input.B);
			}
			return String.Join("", buf);
		}
	}

	/// <summary>
	/// Call the Enter() and Exit() methods of the current state and go to a new one
	/// </summary>
	/// <param name="nextStateName"></param>
	public void ChangeState(string nextStateName) 
	{
		var previousState = currentState;
		currentState.Exit();
		hasHurtboxActive = false;
		lastStateName = currentState.Name;
		if (altState.Contains(nextStateName))
			currentState = altStateDict[nextStateName];
		else if (allStateDict.ContainsKey(nextStateName))
			currentState = allStateDict[nextStateName];
		else
			GD.PrintErr($"State {nextStateName} not found for player {Name}");
		
		
		if (Globals.logOn)
			Globals.Log($"{Name} changing state from {previousState.Name} > {currentState.Name}");
		if (currentState.animationName != "None")
			animationPlayer.NewAnimation(currentState.animationName);
		inputHandler.playerState = currentState;
		
		if (grounded && nextStateName != grabString && previousState.turnAroundOnExit)
		{
			CheckTurnAround();
		}
		currentState.Enter();
	}

	public float GetAnimationLength(string anim)
	{
		if (animationPlayer is null)
			animationPlayer = (AnimationPlayer)GetNode("AnimationPlayer");
		var foundAnim = animationPlayer.GetAnimation(anim);
		if (foundAnim is object)
			return foundAnim.Length;
		else
			return 0;
	}

	protected void AddAltState(string baseState)
	{ altState.Add(baseState); }

	public void AnimationFinished(string animName) 
	{
		if (currentState.loop) 
		{
			animationPlayer.Restart();
		}
		else
		{
			currentState.AnimationFinished();
		}
	}

	/// <summary>
	/// Called at the end of the match
	/// </summary>
	public void RemoveAllHeld()
	{
		inputHandler.heldKeys.Clear();
	}


	public bool CheckHeldKey(char key) 
	{
		return (inputHandler.heldKeys.Contains(key));
	}

	public bool CheckNoDirectionsHeld()
	{
		return !(inputHandler.heldKeys.Contains('2') || inputHandler.heldKeys.Contains('6') || inputHandler.heldKeys.Contains('4'));

	}

	public bool CheckHeldKeys(char[] keys)
	{
		for (int i = 0; i < keys.Length; i++)
		{
			if (!CheckHeldKey(keys[i]))
				return false;
		}
		return true;
	}

	public bool CheckHeldFlippableKeys(char[] keys)
	{
		for (int i = 0; i < keys.Length; i++)
		{
			if (!CheckFlippableHeldKey(keys[i]))
				return false;
		}
		return true;
	}

	public bool CheckFlippableHeldKey(char key)
	{
		if (!facingRight)
		{
			if (key == '6')
				key = '4';
			else if (key == '4')
				key = '6';
		}
		return (inputHandler.heldKeys.Contains(key));
	}

	public string LogHeldKeys()
	{
		return inputHandler.heldKeys.DumpTest();
	}

	public bool CheckLastBufInput(InputContainer.CharPair key)
	{
		var buf = inputHandler.GetBuffer();
		return (key == buf.Get(buf.Count - 2));
	}

	/// <summary>
	/// Checks if the key is in the input buffer
	/// </summary>
	/// <param name="key"></param>
	/// <returns></returns>
	public bool CheckBuffer(InputContainer.CharPair key)
	{
		
		return Globals.ArrayInList(inputHandler.GetBuffer(), key);
	}

	public bool CheckHitStopBuffer(InputContainer.CharPair key)
	{
		return Globals.ArrayInList(inputHandler.GetHitStopBuffer(), key);
	}

	/// <summary>
	/// Checks if the sequence of inputs in elements can be found in order in the buffer
	/// </summary>
	/// <param name="elements"></param>
	/// <returns></returns>C
	public bool CheckBufferComplex(InputContainer elements)
	{
		return Globals.ArrOfArraysComplexInList(inputHandler.GetBuffer(), elements);
	}

	public bool CanSuperJump()
	{
		return ((Globals.frame - lastPressedDownFrame < 15) && (lastPressedDownFrame < lastPressedUpFrame)) || (Globals.frame - lastPressedDashFrame < 5) || CheckHeldKey('c');
	}

	/// <summary>
	/// passes any new inputs since the past frame to the input handler for buffering, withholding and passing to the current state
	/// </summary>
	/// <param name="hitStop"></param>
	public void FrameAdvanceInputs(int hitStop,int unhandledInputs)
	{
		inputHandler.FrameAdvance(hitStop, unhandledInputs, negEdgeCallback);
	}

	/// <summary>
	/// Called even during hitstop
	/// </summary>
	public void AlwaysFrameAdvance()
	{
		eventSched.FrameAdvance();
		electricity.Visible = electrocuted;

		var direction = sprite.Scale.x / Math.Abs(sprite.Scale.x);
		if (otherPlayer == null || !Godot.Object.IsInstanceValid(otherPlayer))
			return; // Sloppy fix, I know...

		if (currentState.Name == grabbedString && otherPlayer.ShrinkOtherSprite())
		{
			var newScale = sprite.Scale;
			newScale.x = 1.5f * direction;
			newScale.y = 1.5f;
			sprite.Scale = newScale;
			var newOffset = sprite.Offset;
			newOffset.y = 20;
			sprite.Offset = newOffset;
		}
		else
		{
			var newScale = sprite.Scale;
			newScale.x = 3 * direction;
			newScale.y = 3;
			sprite.Scale = newScale;
		}
	}

	/// <summary>
	/// Called anytime outside of rollbacks
	/// </summary>
	public void TimeAdvance()
	{
		
	}

	protected virtual void CharSpecificFrameAdvance()
	{

	}

	/// <summary>
	/// Only called outside of hitstop
	/// </summary>
	public virtual void FrameAdvance() 
	{
		Update();
		if (counterStopFrames > 0)
		{
			counterStopFrames--;
			return;
		}
		bool wasHurtboxPreviouslyActive = false;

		for (int i = 0; i < hurtBoxes.Count; i++) //  record before the frame advance whether hurtBoxes are active
		{
			var box = hurtBoxes[i];
			if (!box.Disabled)
				wasHurtboxPreviouslyActive = true;
		}
		animationPlayer.FrameAdvance();
		if (wasHurtboxPreviouslyActive) // here we test to see if we have swtiched from active hurtboxes to inactive hurboxes
		{
			bool allHurtboxesInactive = true;
			for (int i = 0; i < hurtBoxes.Count; i++)
			{
				var box = hurtBoxes[i];
				if (!box.Disabled)
					allHurtboxesInactive = false;
			}
			if (allHurtboxesInactive)
			{
				hasHurtboxActive = true;
			}
		}
		
		if (!facingRight)
			sprite.RotationDegrees *= -1;

		currentState.FrameAdvance();
		CharSpecificFrameAdvance();
		
		if (invulnFrames > 0)
		{
			invulnFrames--;
		}
			
		if (grabInvulnFrames > 0)
			grabInvulnFrames--;

		if (backdashCooldownRemaining > 0)
			backdashCooldownRemaining--;

		if (hadoukenCooldownRemaining > 0)
			hadoukenCooldownRemaining--;

		if (meterGainCooldownRemaining > 0)
			meterGainCooldownRemaining--;

		if (specialBreakFramesRemaining > 0)
		{
			GreySprite();
			specialBreakFramesRemaining--;
			if (specialBreakFramesRemaining == 0)
			{
				EndSpecialBreak();
			}
		}

		GFXSpecialFrameAdvance(); // purely graphic, should be moved
		AdjustHitpush(); // make sure this is placed in the right spot...
		MoveSlideDeterministicOne();
	}

	private void GFXSpecialFrameAdvance()
	{
		shield.crouching = currentState.tags.Contains(Globals.Tags.crouching);

		switch (currentState.GetExtraGFXState())
		{
			case State.GFXStates.NONE:
				shield.Visible = false;
				break;
			case State.GFXStates.SHIELD:
				shield.Visible = true;
				shieldEmission.Visible = false;
				break;
			case State.GFXStates.SHIELDACTIVE:
				shield.Visible = true;
				shieldEmission.Visible = true;
				break;
			case State.GFXStates.CANTECH:
				break;


		}
	}

	/// <summary>
	/// First half of the integer based, deterministic collision detection system.
	/// </summary>
	private void MoveSlideDeterministicOne()
	{
		int xChange = (int)Math.Floor((velocity.x) / 2);
		int yChange = (int)Math.Floor(velocity.y / 2);
		int curDistBetween = (int)Math.Abs(otherPlayer.internalPos.x - internalPos.x);
		int distBetween = (int)Math.Abs(otherPlayer.internalPos.x - (internalPos.x + xChange));
		if (distBetween > MAXPLAYERDIST){
			int dir = -1;
			if (OtherPlayerOnLeft())
				dir = 1;

			xChange = (MAXPLAYERDIST - curDistBetween) * dir;
			currentState.HitWall();
		}
		ChangeIntPositionRel(xChange, yChange);
		
		CorrectPositionBounds();
	}

	private void MoveSlideDeterministic()
	{

	}

	/// <summary>
	/// Updates the remaining hitpush and adjusts the player accordingly.  does NOT use velocity
	/// </summary>
	private void AdjustHitpush()
	{
		if (hitPushRemaining != 0)
		{
			if ((hitPushRemaining > -hitPushSpeed) && (hitPushRemaining < hitPushSpeed))
			{
				hitPushRemaining = 0;
			}
			else
			{
				var speed = hitPushSpeed;
				if (currentState.tags.Contains(Globals.Tags.shield))
					speed *= 2;
				if (hitPushRemaining < 0)
				{
					internalPos.x -= speed;
					hitPushRemaining += speed;
				}
				else
				{
					internalPos.x += speed;
					hitPushRemaining -= speed;
				}
			}
		}
	}

	/// <summary>
	/// Finishes the movement system
	/// </summary>
	public void MoveSlideDeterministicTwo()
	{
		if (counterStopFrames > 0)
		{
			return;
		}
		int xChange = (int)Math.Ceiling((velocity.x) / 2);
		int yChange = (int)Math.Ceiling(velocity.y / 2);

		int curDistBetween = (int)Math.Abs(otherPlayer.internalPos.x - internalPos.x);
		int distBetween = (int)Math.Abs(otherPlayer.internalPos.x - (internalPos.x + xChange));
		if (distBetween > MAXPLAYERDIST)
		{
			int dir = -1;
			if (OtherPlayerOnLeft())
				dir = 1;

			xChange = (MAXPLAYERDIST - curDistBetween) * dir;
			currentState.HitWall();
		}
		
		ChangeIntPositionRel(xChange, yChange);
		CorrectPositionBounds();
	}

	/// <summary>
	/// Hitboxes are checked AFTER FrameAdvance()
	/// </summary>
	public void CheckHit()
	{
		if (!otherPlayer.IsInvuln())
			currentState.CheckHit();
	}

	/// <summary>
	/// Adapts the 100x position to the visualized position
	/// </summary>
	public void RenderPosition()
	{
		//debugPos.Text = $"{internalPos.x}, {internalPos.y}";
		Vector2 tempPos = Position;
		tempPos.x = (int)Math.Floor(internalPos.x / 100);
		tempPos.y = (int)Math.Floor(internalPos.y / 100);
		Position = tempPos;
	}

	/// <summary>
	/// Stay inside the bounds of the stage
	/// </summary>
	private void CorrectPositionBounds()
	{
		if (internalPos.y >= Globals.floor)
		{
			ChangeIntPositionAbs((int)internalPos.x, Globals.floor);
			grounded = true;
			currentState.Land();
		}

		if (internalPos.x > Globals.rightWall)
		{
			ChangeIntPositionAbs(Globals.rightWall, (int)internalPos.y);
			currentState.HitWall();
		}
		else if (internalPos.x < Globals.leftWall)
		{
			ChangeIntPositionAbs(Globals.leftWall, (int)internalPos.y);
			currentState.HitWall();
		}
	}

	public bool CheckTouchingWall()
	{
		if (internalPos.x > 46400 || internalPos.x < 1600)
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	public void CorrectGrounded()
	{
		grounded = !(internalPos.y < Globals.floor);
	}

	public void SlideAway() //MAKE SURE THIS WORKS
	{
		var mod = 1;

		if (internalPos.x < otherPlayer.internalPos.x) 
		{
			mod = -1;
		}
		ChangeIntPositionRel(4 * mod, (int)internalPos.y);
	}

	public void PushMovement(float xVel) 
	{
		currentState.PushMovement(xVel);
	}

	public bool OtherPlayerOnRight()
	{
		return internalPos.x < otherPlayer.internalPos.x;
	}

	public bool OtherPlayerOnLeft()
	{

		return internalPos.x > otherPlayer.internalPos.x;
	}

	public int GetDistToOtherPlayer()
	{
		return Math.Abs((int)internalPos.x - (int)otherPlayer.internalPos.x);
	}

	/// <summary>
	/// Called to check if the player should change directions.  Always called when changing states.  Some states call this in their FrameAdvance() methods.
	/// </summary>
	public void CheckTurnAround() 
	{
		if (otherPlayer == null) 
		{
			return;
		}
		if (OtherPlayerOnLeft() && facingRight)
		{
			TurnLeft();
		}
		else if (OtherPlayerOnRight() && !facingRight) 
		{
			TurnRight();
		}
	}

	private Vector2 facingRightScale = new Vector2(3, 3);
	private Vector2 facingLeftScale = new Vector2(-3, 3);
	private Vector2 facingLeftBoxScale = new Vector2(-1, 1);
	public void TurnRight()
	{
		facingRight = true;
		mainSprite.Scale = facingRightScale;
		frontSprite.Scale = facingRightScale;
		behindSprite.Scale = facingRightScale;
		
		hurtBoxParent.Scale = Vector2.One;
		hitBoxParent.Scale = Vector2.One;
	}

	public void TurnLeft()
	{
		facingRight = false;
		mainSprite.Scale = facingLeftScale;
		frontSprite.Scale = facingLeftScale;
		behindSprite.Scale = facingLeftScale;
		hitBoxParent.Scale = facingLeftBoxScale;
		hurtBoxParent.Scale = facingLeftBoxScale;
	}

	/// <summary>
	/// Checks if this player is not in a hitstate so they can be grabbed
	/// </summary>
	/// <returns></returns>
	public bool IsGrabbable()
	{
		return ((grabInvulnFrames == 0 && currentState.IsGrabbable() && grounded));
	}

	public bool IsAirGrabbable()
	{
		return (grabInvulnFrames == 0 && currentState.IsGrabbable() && !grounded);
	}

	/// <summary>
	/// Checks if we can grab the opposing player
	/// </summary>
	/// <returns></returns>
	public bool CanGrab()
	{
		return !noGrabStates.Contains(currentState.Name) && !noGrabLastStates.Contains(lastStateName);
	}

	/// <summary>
	/// Prevent kara cancelling into Shield
	/// </summary>
	/// <returns></returns>
	public bool CanShield()
	{
		return !allStateDict[lastStateName].tags.Contains(Globals.Tags.attack) && CheckFlippableHeldKey('4');
	}
	public void Prorate(int prorationLevel)
	{
		proration = Math.Max(1, proration - prorationLevel);
	}

	/// <summary>
	/// Receive a hit, but do not calculate the results yet
	/// </summary>
	/// <param name="rightAttack"></param>
	/// <param name="dmg"></param>
	/// <param name="blockStun"></param>
	/// <param name="hitStun"></param>
	/// <param name="height"></param>
	/// <param name="hitPush"></param>
	/// <param name="launch"></param>
	/// <param name="knockdown"></param>
	/// <param name="prorationLevel"></param>
	public void ReceiveHit(Globals.AttackDetails hitDetails, Globals.AttackDetails chDetails) 
	{
		receivedHit = hitDetails;
		if (hitDetails.removeOTG)
		{
			wasOTGHit = false;
		}

		if ((currentState.Name == KnockdownString || wasOTGHit) && !hitDetails.removeOTG)
		{
			wasOTGHit = true;
			receivedHit = Globals.otgHit;

		}
		if ((currentState.isCounter && (!hasHurtboxActive || currentState.isSpecial)) || 
		(Globals.alwaysCounter && combo == 0))
		{
			receivedHit = chDetails;
			Globals.EmitSignal(Globals.PlayerSignal.Counter, otherPlayer.Name);
			if (!grounded)
				counterStopFrames = 10;
			else if (currentState.tags.Contains(Globals.Tags.attack))
				counterStopFrames = Globals.attackLevels[((BaseAttack)currentState).level].counterStopFrames;
			else
				counterStopFrames = 2;
			
		}
	velocity = Vector2.Zero;
		wasHitThisFrame = true;
	}

	public void ClearHit()
	{
		wasHitThisFrame = false;
	}

	public virtual bool CalculateHit()
	{
		if (!wasHitThisFrame)
		{
			return false;
		}
		Globals.AttackDetails details = receivedHit;
		if (wasOTGHit)
		{
			details = Globals.otgHit;
			if (OtherPlayerOnLeft())
				details.dir = BaseAttack.ATTACKDIR.RIGHT;
			else
				details.dir = BaseAttack.ATTACKDIR.LEFT;
		}




		// I separate this into two pieces so that the next entered state can handle stun and damage
		currentState.ReceiveHit(details);
		currentState.ReceiveStunDamage(details);
		((HitState) currentState).PlayHitSound(details.hitSound);
		if (!details.projectile)
			Globals.EmitSignal(Globals.PlayerSignal.HitStop, Name, details.hitStop);
		PostHitCall();

		wasHitThisFrame = false;

		if (Globals.mode == Globals.Mode.TRAINING)
			otherPlayer.DisplayPlusFrames(currentState.stunRemaining);
		return true;
	}
	
	protected virtual void PostHitCall(){}

	public void DisplayPlusFrames(int opponentStun)
	{

		if (!currentState.tags.Contains(Globals.Tags.attack) || !grounded || otherPlayer.currentState.tags.Contains(Globals.Tags.knockdown))
			return;
		var diff = opponentStun - animationPlayer.GetRemainingFrames();
		var plusText = plusFrames[currPlusFrameIndex];
		currPlusFrameIndex = (currPlusFrameIndex + 1) % MAXPLUSFRAMES;
		plusText.SetPosition(Vector2.Zero);
		plusText.Init(diff);
		
	}

	public bool HurtboxesInactive()
	{
		for (int i = 0; i < hurtBoxes.Count; i++)
		{
			var hurtBox = hurtBoxes[i];
			if (!hurtBox.Disabled){
				return false;
			}
		}
		return true;
	}

	public void OnHitPush(int hitPush) 
	{
		
		if (otherPlayer.CheckTouchingWall() && hitPush > 0)
			{
				if (OtherPlayerOnRight())
				{
					hitPushRemaining = -hitPush;
				}
				else if (OtherPlayerOnLeft())
				{
					hitPushRemaining = hitPush;
				}
			}
	}

	public void EmitHadouken(HadoukenPart h)
	{
		GetParent<GameScene>().OnHadoukenEmitted(h); // this is really gross but I want to avoid the use of signals as much as possible
	}

	public void DeleteHadouken(HadoukenPart h)
	{
		var parent = GetParent<GameScene>();
		if (parent != null)
			parent.OnHadoukenRemoved(h);
	}

	public void CommandHadouken(string hadName, HadoukenPart.ProjectileCommand command)
	{
		GetParent<GameScene>().OnHadoukenCommand( Name, hadName, command); // this is really gross but I want to avoid the use of signals as much as possible
	}

	public void ResetComboAndProration()
	{
		combo = 0;
		hasBeenSpiked = false;
		proration = 24;
		canGroundbounce = true;
		terminalVelocity = standardTerminalVelocity;
		hasBeenLaunched = false;
		Globals.EmitSignal(Globals.PlayerSignal.ComboChanged, Name, combo);
	}

	public void ComboUp()
	{
		combo++;
		Globals.EmitSignal(Globals.PlayerSignal.ComboChanged, Name, combo);
	}

	
	public void DeductHealth(int dmg, bool chip = false)
	{
		if (otherPlayer.health <= 0)
			return;
		
		var fixDmg = new Fix64(dmg);

		dmg = (int)Math.Floor((float)(fixDmg * damageMod * otherPlayer.damageDealtMod));

		health -= dmg;
		if (chip && health <= 1)
			health = 1;
		GainBurst();

		if (health <= 0)
		{
			ChangeState(AirKnockdownString);
			velocity.y = -200;
		}
		
		Globals.EmitSignal(Globals.PlayerSignal.HealthChanged, Name, health);
	}

	public void GainMeter(int gains)
	{
		if (meterGainCooldownRemaining > 0) {
			gains = 1;
		}
		meter = Math.Min(meter + gains, 10000);
		Globals.EmitSignal(Globals.PlayerSignal.MeterChanged, Name, meter);
	}

	public void GainBurst()
	{
		if (burstMeter == 100) return;
		burstMeter += 2;
		Globals.EmitSignal(Globals.PlayerSignal.BurstSet, Name, burstMeter);
	}

	public bool TrySpendMeter(int cost = 5000)
	{
		if (meter >= cost)
		{
			meter -= cost;

			if (cost == 5000)
			{
				meterGainCooldownRemaining = 180;
			}
			Globals.EmitSignal(Globals.PlayerSignal.MeterChanged, Name, meter);
			return true;
		}
		else
		{
			return false;
		}
	}

	public void EmptyMeter()
	{
		meter = 0;
		Globals.EmitSignal(Globals.PlayerSignal.MeterChanged, Name, meter);
	}

	public bool TrySpendBurst()
	{
		if (burstMeter == 100)
		{
			burstMeter = 0;
			Globals.EmitSignal(Globals.PlayerSignal.BurstSet, Name, burstMeter);
			currentState.stunRemaining = 0;
			return true;
		}
		else
		{
			return false;
		}
	}

	public void SpecialBreak()
	{
		GreySprite();
		specialBreakFramesRemaining = 120;
	}

	private void GreySprite()
	{
		var shaderMaterial = sprite.Material as ShaderMaterial;
		shaderMaterial.SetShaderParam("palette", greyPalette);
	}

	private void ColorSprite()
	{
		var shaderMaterial = sprite.Material as ShaderMaterial;
		shaderMaterial.SetShaderParam("palette", palette);
		shaderMaterial.SetShaderParam("palette_index", colorScheme); // PASSABLE
	}

	public void EndSpecialBreak()
	{
		// may include more in future
		ColorSprite();
	}

	public bool ShrinkOtherSprite()
	{
		return currentState.shrinkOtherSprite;
	}

	public bool CheckOverrideBlock()
	{
		if (aiControlled)
		{
			if (Globals.aiDifficulty == Globals.AIDIFFICULTY.LO)
				return aiRng.Next(2) == 1;
			else
				return true;
		}
		else
			return (!trainingControlledPlayer && Globals.alwaysBlock);

	}

	/// <summary>
	/// Schedule an event.  Overloads depending on whether the current state name should be used or another name (such as an inherited state)
	/// </summary>
	/// <param name="type"></param>
	public void ScheduleEvent(EventScheduler.EventType type)
	{
		Type curType = currentState.GetType();
		string curStateName = curType.ToString();
		eventSched.ScheduleEvent(curStateName, curStateName, type);
	}

	public void ScheduleEvent(EventScheduler.EventType type, string name)
	{
		eventSched.ScheduleEvent(name, name, type);
	}

	public void ScheduleEvent(EventScheduler.EventType type, string name, string expectedStateName)
	{
		eventSched.ScheduleEvent(name, expectedStateName, type);
	}

	public void ForceEvent(EventScheduler.EventType type, string name)
	{
		eventSched.ForceEvent(type, name);
	}

	public void GFXEvent(string name)
	{
		if (Globals.DISABLEGFX)
			return;
		gfxHand.Effect(name, Position, facingRight);
		if (name == ExplosionGfxString)
			spriteAnim.Play(FireAnimString);
		else if (name == PurpleGfxString)
			spriteAnim.Play(PurpleAnimString);
	}

	public void GFXEvent(string name, Vector2 pos)
	{
		gfxHand.Effect(name, pos, facingRight);
	}

	private void ShowShield()
	{
		shield.Visible = true;
	}

	private void HideShield()
	{
		shield.Visible = false;
	}
	public bool AreHitboxesActive()
	{
		return GetRects(hitBoxes, tempHitboxArray);

		
	}

	public bool IsInvuln()
	{
		return invulnFrames > 0;
	}

	/// <summary>
	/// Checks if the opponent's collision box is in our hurtbox.  Used for grabs
	/// </summary>
	/// <returns></returns>
	public Vector2 CheckHurtRectGrab()
	{
		GetRects(hurtBoxes, tempHurtboxArray, true);
		Rect2 otherRect = otherPlayer.GetCollisionRect();
		for (int i = 0; i < 3; i++)
		{
			var hurtRect = tempHurtboxArray[i];
			if (hurtRect.Area == 0)
				continue; 
			if (hurtRect.Intersects(otherRect))
			{
				Rect2 clip = hurtRect.Clip(otherRect);
				Vector2 center = (clip.End - clip.Position) / 2 + clip.Position;
				return center;
			}
		}
		return Vector2.Inf;
	}
	public Vector2 CheckHurtRect()
	{
		GetRects(hurtBoxes, tempHurtboxArray, true);
		otherPlayer.GetRects(otherPlayer.hitBoxes, otherPlayer.tempHitboxArray, true);
		for (int i = 0; i < 3; i++)
		{
			var hurtRect = tempHurtboxArray[i];
			if (hurtRect == null)
				continue;
			for (int j = 0; j < 3; j++)
			{
				var hitRect = otherPlayer.tempHitboxArray[j];
				if (hitRect == null)
					continue;
				if (hurtRect.Intersects(hitRect))
				{
					Rect2 clip = hurtRect.Clip(hitRect);
					Vector2 center = (clip.End - clip.Position) / 2 + clip.Position;
					return center;
				}
			}
		}
		return Vector2.Inf;
	}

	public virtual bool GetRects(Godot.Collections.Array<CollisionShape2D> colShapes, Rect2[] array, bool globalPosition = false) 
	{
		bool active = false;
		for (int i = 0; i < colShapes.Count; i++) 
		{
			CollisionShape2D colShape = (CollisionShape2D)colShapes[i];
			if (!colShape.Disabled){
				array[i] = GetRect(colShape, globalPosition);
				active = true;
			}
			else
			{
				array[i] = new Rect2(-1000000, 100000, 0, 0);
			}
		}

		return active;
	}

	private Vector2 tempRectPos = new Vector2();
	private Rect2 tempRect = new Rect2();
	public Rect2 GetRect(CollisionShape2D colShape, bool globalPosition = false) 
	{
		RectangleShape2D shape = (RectangleShape2D)colShape.Shape;
		Vector2 extents = shape.Extents * 2;
		if (facingRight)
		{
			tempRectPos = colShape.Position - extents / 2;
		}
		else
		{
			tempRectPos.x = -colShape.Position.x - extents.x / 2;
			tempRectPos.y = colShape.Position.y - extents.y / 2;
		}
		if (globalPosition)
		{
			tempRectPos *= 100;
			tempRectPos += internalPos;
			extents *= 100;
		}
		return new Rect2(tempRectPos, extents);
	}

	private Vector2 tempStart = new Vector2();
	private Vector2 collisionRectSize = new Vector2(1400, 4800);
	public Rect2 GetCollisionRect()
	{
		tempStart.x = internalPos.x - 700;
		tempStart.y = internalPos.y - 900;
		
		
		return new Rect2(tempStart, collisionRectSize);
	}

	public bool CheckCollisionRectActive()
	{
		return currentState.CollisionActive();
	}

	/// <summary>
	/// Used for training mode
	/// </summary>
	public void ResetHealth()
	{
		health = 1800;
	}

	public void DebugDisplay()
	{
		GetNode<Label>("DebugPos").Text = Position.ToString();
	}

	/// <summary>
	/// You can use this if you want to draw all the boxes
	/// </summary>
	public override void _Draw()
	{

		if (Globals.mode == Globals.Mode.TRAINING || Globals.mode == Globals.Mode.SYNCTEST)
		{
			GetRects(hitBoxes, tempHitboxArray);
			GetRects(hurtBoxes, tempHurtboxArray);
			Rect2 colRect = GetRect(colBox);

			DrawRect(colRect, colColor);
			for (int i = 0; i < 3; i++)
			{
				DrawRect(tempHitboxArray[i], hitColor);
			}
			if (IsInvuln())
				return;
			for (int i = 0; i < 3; i++)
			{
				DrawRect(tempHurtboxArray[i], hurtColor);
			}
			
			
		}

		
	}
}
