using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Player : Node2D
{
	public State currentState; //The current state governs key aspects of input handling, movement, animation etc.
	public Player otherPlayer; //I know I shouldn't do this, but it makes my life so much easier...

	[Signal]
	public delegate void HealthChanged(string name, int health);
	[Signal]
	public delegate void ComboChanged(string name, int combo);
	[Signal]
	public delegate void ComboSet(string name, int combo);
	[Signal]
	public delegate void HitConfirm();
	[Signal]
	public delegate void LevelUp();
	[Signal]
	public delegate void HadoukenEmitted(HadoukenPart h);
	[Signal]
	public delegate void HadoukenRemoved(HadoukenPart h);

	[Export]
	public int speed = 400;

	[Export]
	public int dashSpeed = 700;

	[Export]
	public int accel = 40;

	[Export]
	public int jumpForce = 800;

	[Export]
	public int gravity = 50; 

	[Export]
	public bool dummy = false; //you can use this for testing with a dummy

	[Export]
	public int hitPushSpeed = 300;

	[Export]
	public bool debugPress = false;

	[Export]
	public string debugKeys = "6";

	[Export]
	protected Resource[] shaders;

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
	public List<Special> groundSpecials = new List<Special>();
	public List<Special> airSpecials = new List<Special>();

	// All of these will be stored in gamestate
	public int hitPushRemaining = 0; // stores the hitpush yet to be applied
	public Vector2 internalPos; // this will be stored at 100x the actual rendered position, to allow greater resolution
	private int health = 1600;
	public Vector2 velocity = new Vector2(0, 0);
	public bool facingRight = true;
	public bool grounded;
	private int combo = 0;
	public int proration = 32;
	public bool canDoubleJump;
	public int invulnFrames = 0;
	public int airDashFrames = 0;

	/// <summary>
	/// Contains all vital data for saving gamestate
	/// </summary>
	[Serializable]
	public struct PlayerState
	{
		public List<char[]> inBuf2 { get; set; }
		public int inBuf2Timer { get; set; }
		public List<char[]> hitStopInputs { get; set; }
		public List<char> heldKeys { get; set; }
		public string currentState { get; set; }
		public Dictionary<string, int> stateData { get; set; }
		public bool canDoubleJump { get; set; }
		public bool hitConnect { get; set; }
		public int frameCount { get; set; }
		public int stunRemaining { get; set; }
		public int hitPushRemaining { get; set; }
		public bool flipH { get; set; }
		public int health { get; set; }
		public int[] position { get; set; }
		public int[] velocity { get; set; }
		public bool facingRight { get; set; }
		public bool touchingWall { get; set; }
		public bool grounded { get; set; }
		public int combo { get; set; }
		public int proration { get; set; }
		public string animationName { get; set; }
		public int animationCursor { get; set; }
		public int lastFrameInputs { get; set; }
		public int invulnFrames { get; set; }
		public int airDashFrames { get; set; }

	}

	/// <summary>
	/// Info about a special
	/// </summary>
	public struct Special
	{
		public List<char[]> inputs;
		public string state;

		public Special(List<char[]> inputsList, string newState) 
		{
			inputs = inputsList;
			state = newState;
		}
	}

	// components of a received attack
	private bool wasHit = false;
	private Globals.AttackDetails receivedHit;
	private Globals.AttackDetails receivedCHit;

	private BaseAttack.ATTACKDIR hit_rightAttack;
	private int hit_dmg;
	private int hit_blockStun;
	private int hit_hitStun;
	private State.HEIGHT hit_height;
	private int hit_hitPush;
	private Vector2 hit_launch;
	private bool hit_knockdown;
	private int hit_prorationLevel;
	private BaseAttack.EXTRAEFFECT hit_effect;
	private Vector2 hit_collisionPnt;

	// Box colors
	private Color hitColor = new Color(0, 0, 255, 0.5f);
	private Color hurtColor = new Color(255, 0, 0, 0.5f);
	private Color colColor = new Color(0, 255, 0, 0.5f);
	private Color grabColor = new Color(0, 0, 0, 0.5f);

	// Sub nodes
	public Position2D grabPos;
	public Area2D hitBoxes;
	public Area2D hurtBoxes;
	private CollisionShape2D colBox;
	public AnimationPlayer animationPlayer;
	public Sprite sprite;
	private EventScheduler eventSched;
	private GFXHandler gfxHand;
	private Label debugPos;
	

	public override void _Ready()
	{
		grabPos = GetNode<Position2D>("GrabPos");
		hitBoxes = GetNode<Area2D>("HitBoxes");
		hurtBoxes = GetNode<Area2D>("HurtBoxes");
		colBox = GetNode<CollisionShape2D>("CollisionBox");
		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		sprite = GetNode<Sprite>("Sprite");
		eventSched = GetNode<EventScheduler>("EventScheduler");
		gfxHand = GetNode<GFXHandler>("GFXHandler");
		debugPos = GetNode<Label>("DebugPos");

		animationPlayer.Connect("AnimationFinished", this, nameof(AnimationFinished));
		foreach (CollisionShape2D box in hitBoxes.GetChildren()) 
		{
			box.Shape = new RectangleShape2D();
		}
		foreach (CollisionShape2D box in hurtBoxes.GetChildren())
		{
			box.Shape = new RectangleShape2D();
		}

		inputHandler = new InputHandler();
		Godot.Collections.Array allStates = GetNode<Node>("StateTree").GetChildren();
		foreach (Node state in allStates) 
		{
			state.Connect("StateFinished", this, nameof(ChangeState));
		}
		currentState = GetNode<State>("StateTree/Idle");
		ChangeState("Idle");

		if (debugPress)
		{
			foreach (char key in debugKeys)
			{
				inputHandler.heldKeys.Add(key);
			}
			
		}

		var shaderMaterial = sprite.Material as ShaderMaterial;
		//var resource = ResourceLoader.Load(shaderPaths[colorScheme]);
		shaderMaterial.SetShaderParam("palette", shaders[colorScheme]);
		//GD.Print(shaderMaterial.GetShaderParam("palette"));
		
	}

	public void Reset()
	{
		ResetComboAndProration();
		ChangeState("Idle");
	}

	public PlayerState GetState()
	{
		var pState = new PlayerState();
		pState.inBuf2 = new List<char[]>();
		foreach (char[] inp in inputHandler.inBuf2)
		{
			pState.inBuf2.Add((char[])inp.Clone());
		}

		pState.hitStopInputs = new List<char[]>();
		foreach (char[] inp in inputHandler.hitStopInputs)
		{
			pState.hitStopInputs.Add((char[])inp.Clone());
		}

		pState.inBuf2Timer = inputHandler.inBuf2Timer;


		pState.heldKeys = new List<char>();
		foreach (char c in inputHandler.heldKeys)
		{
			pState.heldKeys.Add(c);
		}

		pState.canDoubleJump = canDoubleJump;
		pState.currentState = currentState.Name;
		pState.stateData = currentState.Save();
		pState.frameCount = currentState.frameCount;
		pState.hitConnect = currentState.hitConnect;
		pState.stunRemaining = currentState.stunRemaining;
		pState.flipH = sprite.FlipH;
		pState.hitPushRemaining = hitPushRemaining;
		pState.health = health;
		
		pState.position = new int[] { (int)internalPos.x, (int)internalPos.y };
		pState.animationCursor = animationPlayer.cursor;
		
		pState.animationName = animationPlayer.AssignedAnimation;
		//GD.Print($"saving animation {pState.animationName}");
		pState.velocity = new int[] { (int)velocity.x, (int)velocity.y };
		pState.facingRight = facingRight;
		pState.grounded = grounded;
		pState.combo = combo;
		pState.proration = proration;
		pState.lastFrameInputs = inputHandler.lastFrameInputs;
		pState.invulnFrames = invulnFrames;
		pState.airDashFrames = airDashFrames;
		return pState;
	}

	public void SetState(PlayerState pState)
	{
		inputHandler.SetInBuf2(pState.inBuf2);
		inputHandler.inBuf2Timer = pState.inBuf2Timer;
		inputHandler.hitStopInputs = pState.hitStopInputs;
		inputHandler.heldKeys = pState.heldKeys;
		currentState = GetNode<State>("StateTree/" + pState.currentState);
		inputHandler.playerState = currentState;
		currentState.hitConnect = pState.hitConnect;
		currentState.frameCount = pState.frameCount;
		currentState.Load(pState.stateData);

		string animation = pState.animationName;
		//if (altState.Contains(animation.Substring(2)))
		//{
		//	animation = animation.Substring(2);
		//}
		animationPlayer.SetAnimationAndFrame(animation, pState.animationCursor);
		currentState.stunRemaining = pState.stunRemaining;
		sprite.FlipH = pState.flipH;
		hitPushRemaining = pState.hitPushRemaining;
		canDoubleJump = pState.canDoubleJump;

		health = pState.health;
		EmitSignal(nameof(HealthChanged), Name, health);
		internalPos = new Vector2(pState.position[0], pState.position[1]);
		velocity = new Vector2(pState.velocity[0], pState.velocity[1]);
		facingRight = pState.facingRight;
		grounded = pState.grounded;
		combo = pState.combo;
		proration = pState.proration;
		inputHandler.lastFrameInputs = pState.lastFrameInputs;
		invulnFrames = pState.invulnFrames;
		airDashFrames = pState.airDashFrames;
		EmitSignal(nameof(ComboSet), Name, combo);

	}

	/// <summary>
	/// Called to delete graphic effects if necessitated by a rollback
	/// </summary>
	/// <param name="frame"></param>
	public void Rollback(int frame)
	{
		gfxHand.Rollback(frame);
	}

	public void PrintBuffer()
	{
		GD.Print("Buffer ----");
		foreach (char[] inp in inputHandler.inBuf2)
		{
			GD.Print(string.Join(",", inp));
		}
		GD.Print("----");
		
	}

	/// <summary>
	/// Deals with unhandled inputs, the input buffer, and a hitstop buffer.  Subject to constant change
	/// </summary>
	private class InputHandler 
	{
		public List<char[]> inBuf2 = new List<char[]>();
		public List<char[]> hitStopInputs = new List<char[]>();

		//private List<char> order = new List<char>() { 's', 'k', 'p', '6', '4', ''}; consider input priority later

		public int inBuf2TimerMax = 5;
		public int inBuf2Timer = 5;
		public List<char> heldKeys = new List<char>();
		public State playerState;
		/// <summary>
		/// Used for checking if a key has been pressed or released
		/// </summary>
		public int lastFrameInputs;

		private void BufAddInput(char[] input)
		{
			inBuf2Timer = inBuf2TimerMax;
			inBuf2.Add(input);
			if (inBuf2.Count > 55)
				inBuf2 = new List<char[]>();
		}

		private void BufTimerDecrement()
		{
			if (inBuf2Timer > 0)
			{
				inBuf2Timer--;
			}
			else
			{
				inBuf2 = new List<char[]>();
			}
		}

		public void SetInBuf2(List<char[]> newBuf)
		{
			inBuf2 = new List<char[]> { };
			foreach (char[] inp in newBuf)
			{
				inBuf2.Add((char[])inp.Clone());
			}
		}

		public void EmptyHitStop()
		{
			hitStopInputs.Clear();
		}

		private void AddHitStopBuffer(List<char[]> unhandledInputs)
		{
			foreach (char[] inputArr in unhandledInputs)
			{

				hitStopInputs.Add(inputArr);
			}
		}

		private List<char[]> ConvertInputs(int inputs)
		{
			var unhandledInputs = new List<char[]>();
			if ((inputs & 1) != 0 && (lastFrameInputs & 1) == 0)
			{
				unhandledInputs.Add(new char[] { '8', 'p' });
			}
			else if ((inputs & 1) == 0 && (lastFrameInputs & 1) != 0)
			{
				unhandledInputs.Add(new char[] { '8', 'r' });
			}

			if ((inputs & 2) != 0 && (lastFrameInputs & 2) == 0)
			{
				unhandledInputs.Add(new char[] { '2', 'p' });
			}
			else if ((inputs & 2) == 0 && (lastFrameInputs & 2) != 0)
			{
				unhandledInputs.Add(new char[] { '2', 'r' });
			}

			if ((inputs & 4) != 0 && (lastFrameInputs & 4) == 0)
			{
				unhandledInputs.Add(new char[] { '6', 'p' });
			}
			else if ((inputs & 4) == 0 && (lastFrameInputs & 4) != 0)
			{
				unhandledInputs.Add(new char[] { '6', 'r' });
			}

			if ((inputs & 8) != 0 && (lastFrameInputs & 8) == 0)
			{
				unhandledInputs.Add(new char[] { '4', 'p' });
			}
			else if ((inputs & 8) == 0 && (lastFrameInputs & 8) != 0)
			{
				unhandledInputs.Add(new char[] { '4', 'r' });
			}

			if ((inputs & 16) != 0 && (lastFrameInputs & 16) == 0)
			{
				unhandledInputs.Add(new char[] { 'p', 'p' });
			}
			else if ((inputs & 16) == 0 && (lastFrameInputs & 16) != 0)
			{
				unhandledInputs.Add(new char[] { 'p', 'r' });
			}

			if ((inputs & 32) != 0 && (lastFrameInputs & 32) == 0)
			{
				unhandledInputs.Add(new char[] { 'k', 'p' });
			}
			else if ((inputs & 32) == 0 && (lastFrameInputs & 32) != 0)
			{
				unhandledInputs.Add(new char[] { 'k', 'r' });
			}

			if ((inputs & 64) != 0 && (lastFrameInputs & 64) == 0)
			{
				unhandledInputs.Add(new char[] { 's', 'p' });
			}
			else if ((inputs & 64) == 0 && (lastFrameInputs & 64) != 0)
			{
				unhandledInputs.Add(new char[] { 's', 'r' });
			}


			return unhandledInputs;
		}

		public void FrameAdvance(int hitStop, int inputs)
		{ 
			List<char[]> unhandledInputs = ConvertInputs(inputs);
			lastFrameInputs = inputs;
			foreach (char[] inputArr in unhandledInputs)
			{
				BufAddInput(inputArr);
			}
				

			if (hitStop > 0 || playerState.DelayInputs()) // delay the handling of inputs until after hitstop ends
			{
				AddHitStopBuffer(unhandledInputs);
				return;
			}

			if (unhandledInputs.Count == 0)
				BufTimerDecrement();

			unhandledInputs = hitStopInputs.Concat(unhandledInputs).ToList();
			unhandledInputs = SortInputs(unhandledInputs);
			hitStopInputs = new List<char[]>();
			foreach (char[] inputArr in unhandledInputs)
			{
				if (playerState.DelayInputs())
				{
					hitStopInputs.Add(inputArr);
					continue;
				}
				
				
				// Hold or release keys
				if (inputArr[1] == 'p')
				{
					heldKeys.Add(inputArr[0]);
					
				}
				else if (inputArr[1] == 'r')
				{
					bool removeResult = heldKeys.Remove(inputArr[0]);
				}
				
				

				playerState.HandleInput(inputArr);
			}
			
			
			unhandledInputs.Clear();
			
		}

		public List<char[]> SortInputs(List<char[]> inputs)
		{
			var jumpInputs = new List<char[]>();
			var otherInputs = new List<char[]>();
			foreach (char[] inputArr in inputs)
			{
				if (inputArr == new char[] {'6', 'p'} || inputArr == new char[] { '4', 'p' })
					jumpInputs.Add(inputArr);
				else
					otherInputs.Add(inputArr);
			}
			return jumpInputs.Concat(otherInputs).ToList();
		}

		public List<char[]> GetBuffer() 
		{
			return inBuf2;
		}
	}//input buffer needs to be tested with GGPO!!!

	/// <summary>
	/// Call the Enter() and Exit() methods of the current state and go to a new one
	/// </summary>
	/// <param name="nextStateName"></param>
	public void ChangeState(string nextStateName) 
	{
		currentState.Exit();
		
		if (altState.Contains(nextStateName))
			{ nextStateName = charName + nextStateName; }
		currentState = GetNode<State>("StateTree/" + nextStateName);
		if (currentState.animationName != "None")
			animationPlayer.NewAnimation(currentState.animationName);
		inputHandler.playerState = currentState;
		
		if (grounded && nextStateName != "Grab")
		{
			CheckTurnAround();
		}
		currentState.Enter();
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

	public void ClearUnhandled()
	{
		inputHandler.EmptyHitStop();
	}

	public bool CheckHeldKey(char key) 
	{
		return (inputHandler.heldKeys.Contains(key));
	}

	public bool CheckLastBufInput(char[] key)
	{
		var buf = inputHandler.GetBuffer();
		//GD.Print(buf[buf.Count - 1][0]);
		return (key[0] == buf[buf.Count - 2][0]);
	}

	/// <summary>
	/// Checks if the key is in the input buffer
	/// </summary>
	/// <param name="key"></param>
	/// <returns></returns>
	public bool CheckBuffer(char[] key)
	{
		
		return Globals.ArrayInList(inputHandler.GetBuffer(), key);
	}

	/// <summary>
	/// Checks if the sequence of inputs in elements can be found in order in the buffer
	/// </summary>
	/// <param name="elements"></param>
	/// <returns></returns>
	public bool CheckBufferComplex(List<char[]> elements)
	{
		return Globals.ArrOfArraysComplexInList(inputHandler.GetBuffer(), elements);
	}

	/// <summary>
	/// passes any new inputs since the past frame to the input handler for buffering, withholding and passing to the current state
	/// </summary>
	/// <param name="hitStop"></param>
	public void FrameAdvanceInputs(int hitStop,int unhandledInputs)
	{
		inputHandler.FrameAdvance(hitStop, unhandledInputs);
	}

	/// <summary>
	/// Called even during hitstop
	/// </summary>
	public void AlwaysFrameAdvance()
	{
		eventSched.FrameAdvance();
	}

	/// <summary>
	/// Called anytime outside of rollbacks
	/// </summary>
	public void TimeAdvance()
	{
		eventSched.TimeAdvance();
	}

	/// <summary>
	/// Only called outside of hitstop
	/// </summary>
	public void FrameAdvance() 
	{
		
		Update();
		
		animationPlayer.FrameAdvance();
		currentState.FrameAdvance();
		if (invulnFrames > 0)
		{
			invulnFrames--;
		}

		AdjustHitpush(); // make sure this is placed in the right spot...
		
		MoveSlideDeterministicOne();
		
	}

	/// <summary>
	/// First half of the integer based, deterministic collision detection system.
	/// </summary>
	private void MoveSlideDeterministicOne()
	{
		int xChange = (int)Math.Floor((velocity.x) / 2);
		int yChange = (int)Math.Floor(velocity.y / 2);
		internalPos += new Vector2(xChange, yChange);
		CorrectPositionBounds();
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
				//GD.Print($"Internal pos before hitPush applied = {internalPos}.  Speed = {hitPushSpeed}");
				if (hitPushRemaining < 0)
				{
					internalPos.x -= hitPushSpeed;
					hitPushRemaining += hitPushSpeed;
				}
				else
				{
					internalPos.x += hitPushSpeed;
					hitPushRemaining -= hitPushSpeed;
				}
				//GD.Print($"Internal pos after hitPush applied = {internalPos}");
			}
		}
	}

	/// <summary>
	/// Finishes the movement system
	/// </summary>
	public void MoveSlideDeterministicTwo()
	{
		int xChange = (int)Math.Ceiling((velocity.x) / 2);
		int yChange = (int)Math.Ceiling(velocity.y / 2);
		internalPos += new Vector2(xChange, yChange);
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
		debugPos.Text = $"{internalPos.x}, {internalPos.y}";
		Position = new Vector2((int)Math.Floor(internalPos.x / 100), (int)Math.Floor(internalPos.y / 100));
	}

	/// <summary>
	/// Stay inside the bounds of the stage
	/// </summary>
	private void CorrectPositionBounds()
	{
		if (internalPos.y > Globals.floor)
		{
			internalPos= new Vector2(internalPos.x, Globals.floor);
			grounded = true;
		}

		if (internalPos.x > Globals.rightWall)
		{
			internalPos = new Vector2(Globals.rightWall, internalPos.y);
			currentState.HitWall();
		}
		else if (internalPos.x < Globals.leftWall)
		{
			internalPos = new Vector2(Globals.leftWall, internalPos.y);
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

	public void SlideAway() //MAKE SURE THIS WORKS
	{
		var mod = 1;

		if (internalPos.x < otherPlayer.internalPos.x) 
		{
			mod = -1;
		}
		internalPos = new Vector2(internalPos.x + 4 * mod, internalPos.y); // FIX THIS
	}

	public void PushMovement(float xVel) 
	{
		currentState.PushMovement(xVel);
	}

	public bool OtherPlayerOnRight()
	{
		if (internalPos.x < otherPlayer.internalPos.x)
		{
			return true;
		}
		else
		{
			return false;
		}
	}

	public bool OtherPlayerOnLeft()
	{
		if (internalPos.x > otherPlayer.internalPos.x)
		{
			return true;
		}
		else
		{
			return false;
		}
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

	public void TurnRight()
	{
		facingRight = true;
		GetNode<Sprite>("Sprite").Scale = new Vector2(3, 3);
		hurtBoxes.Scale = new Vector2(1, 1);
		hitBoxes.Scale = new Vector2(1, 1);
	}

	public void TurnLeft()
	{
		facingRight = false;
		GetNode<Sprite>("Sprite").Scale = new Vector2(-3, 3);
		GD.Print($"Now turning the hitboxes for {Name}");
		hurtBoxes.Scale = new Vector2(-1, 1);
		hitBoxes.Scale = new Vector2(-1, 1);
		GD.Print("Hitboxes turnt");
	}

	/// <summary>
	/// Checks if this player is not in a hitstate so they can be grabbed.  Will eventually check for if they've only recently recovered.
	/// </summary>
	/// <returns></returns>
	public bool IsGrabbable()
	{
		if (currentState.GetType().IsSubclassOf(typeof(HitState)))
		{
			return false;
		}
		else if (!grounded)
		{
			return false;
		}
		else
		{
			return true;
		}
	}

	public bool IsAirGrabbable()
	{
		if (currentState.GetType().IsSubclassOf(typeof(HitState)))
		{
			return false;
		}
		else if (grounded)
		{
			return false;
		}
		else
		{
			return true;
		}
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
		//GD.Print("HIT DURING ", currentState.Name, currentState.isCounter);
		if (currentState.isCounter)
		{
			receivedHit = chDetails;
			//GD.Print("COUNTER");
		}
		velocity = new Vector2(0, 0);
		wasHit = true;
	}

	public void CalculateHit()
	{
		if (!wasHit)
		{
			return;
		}
		Globals.AttackDetails details = receivedHit;
		

		// I separate this into two pieces so that the next entered state can handle stun and damage
		currentState.ReceiveHit(details);
		GD.Print(currentState.Name);
		currentState.ReceiveStunDamage(details);
		EmitSignal(nameof(HitConfirm));
		
		wasHit = false;
	}

	public void OnHitConnected(int hitPush) 
	{
		if (otherPlayer.CheckTouchingWall())
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
		EmitSignal(nameof(HadoukenEmitted), h);
	}

	public void DeleteHadouken(HadoukenPart h)
	{
		EmitSignal(nameof(HadoukenRemoved), h);
	}

	public void ResetComboAndProration()
	{
		combo = 0;
		proration = 16;
		//GD.Print("Combo over");
		EmitSignal(nameof(ComboChanged), Name, combo);
	}

	public void ComboUp()
	{
		combo++;
		//GD.Print($"combo {combo}");
		EmitSignal(nameof(ComboChanged), Name, combo);
	}

	public void DeductHealth(int dmg)
	{
		GD.Print($"Receiving {dmg} damage");
		health -= dmg;
		EmitSignal(nameof(HealthChanged), Name, health);
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
		gfxHand.Effect(name, Position, facingRight);
	}

	public void GFXEvent(string name, Vector2 pos)
	{
		gfxHand.Effect(name, pos, facingRight);
	}

	public bool IsInvuln()
	{
		return invulnFrames > 0;
	}

	/// <summary>
	/// Checks if the opponent's collision box is in our hurtbox.  Used for airgrabs
	/// </summary>
	/// <returns></returns>
	public Vector2 CheckHurtRectGrab()
	{
		List<Rect2> myRects = GetRects(hurtBoxes, true);
		Rect2 otherRect = otherPlayer.GetCollisionRect();
		foreach (Rect2 hurtRect in myRects)
		{
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
		List<Rect2> myRects = GetRects(hurtBoxes, true);
		List<Rect2> otherRects = otherPlayer.GetRects(otherPlayer.hitBoxes, true);
		foreach (Rect2 hurtRect in myRects)
		{
			foreach (Rect2 hitRect in otherRects)
			{
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

	public List<Rect2> GetRects(Area2D area, bool globalPosition = false) 
	{
		List<Rect2> allRects = new List<Rect2>();
		foreach (CollisionShape2D colShape in area.GetChildren()) 
		{
			if (!colShape.Disabled)
			{
				allRects.Add(GetRect(colShape, globalPosition));
			}
			
		}
		return allRects;
	}

	public Rect2 GetRect(CollisionShape2D colShape, bool globalPosition = false) 
	{
		RectangleShape2D shape = (RectangleShape2D)colShape.Shape;
		Vector2 extents = shape.Extents * 2;
		Vector2 position;
		if (facingRight)
		{
			position = colShape.Position - extents / 2;
		}
		else
		{
			position = new Vector2(-colShape.Position.x - extents.x / 2, colShape.Position.y - extents.y / 2);
		}
		if (globalPosition)
		{
			position *= 100;
			position += new Vector2(internalPos.x, internalPos.y);
			extents *= 100;
		}
		return new Rect2(position, extents);
	}

	public Rect2 GetCollisionRect()
	{
		Vector2 start = new Vector2(internalPos.x - 700, internalPos.y - 900);
		Vector2 size = new Vector2(1400, 4800);
		return new Rect2(start, size);
	}

	/// <summary>
	/// Used for training mode
	/// </summary>
	public void ResetHealth()
	{
		health = 1600;
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
		//return;
		if (Globals.mode == Globals.Mode.TRAINING || Globals.mode == Globals.Mode.SYNCTEST)
		{
			List<Rect2> hitRects = GetRects(hitBoxes);
			List<Rect2> hurtRects = GetRects(hurtBoxes);
			Rect2 colRect = GetRect(colBox);


			foreach (Rect2 rect in hitRects)
			{
				DrawRect(rect, hitColor);
			}

			foreach (Rect2 rect in hurtRects)
			{
				DrawRect(rect, hurtColor);
			}
			if (IsInvuln())
				return;
			DrawRect(colRect, colColor);
		}

		
	}
}
