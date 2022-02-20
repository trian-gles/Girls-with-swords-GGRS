using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Base class for all states
/// </summary>
public abstract class State : Node
{
	public Player owner;
	public int frameCount
	{ get; set; }

	/// <summary>
	/// if this is true, the character immediately stops on entering the state
	/// </summary>
	protected bool stop = true;

	protected int slowdownSpeed = 0;


	[Signal]
	public delegate void StateFinished(string nextStateName);

	[Signal]
	public delegate void PlayerFXEmitted(Vector2 pos, ParticleSprite particle);

	public int stunRemaining 
	{ get; set; }
	public bool loop = false;

	public bool hitConnect = false;

	public enum HEIGHT
	{
		LOW,
		MID,
		HIGH
	}

	protected List<NormalGatling> normalGatlings = new List<NormalGatling>();
	protected List<CommandGatling> commandGatlings = new List<CommandGatling>();
	protected delegate bool RequiredConditionCallback();
	protected delegate void PostInputCallback();
	public override void _Ready()
	{
		owner = GetOwner<Player>();
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

	protected void ApplyGravity()
	{
		owner.velocity.y += owner.gravity;
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

	protected struct CommandGatling
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

	private List<List<char>> Permutations(List<char> chars)
	{
		var res = new List<List<char>>();
		var queue = new Queue<List<char>>();
		queue.Enqueue(new List<char>() { });

		foreach (char c in chars)
		{
			for (int i = 0; i < queue.Count(); i++)
			{
				var perm = queue.Dequeue();
				var newPerm = new List<char>(perm);
				newPerm.Insert(i, c);

				if (newPerm.Count < chars.Count)
					queue.Enqueue(newPerm);
				else
					res.Add(newPerm);
			}
		}
		return res;
	}

	protected void AddCancel(string cancelState)
	{
		foreach (var perm in Permutations(new List<char>() { 'p', 'k', 's' }))
		{
			//GD.Print(perm);
			AddGatling(new char[] {perm[0] ,'p' },  () => owner.CheckHeldKey(perm[1]) && owner.CheckHeldKey(perm[2]), 
				cancelState, () => owner.GFXEvent("Cancel"));
		}
		
	}

	public virtual void HandleInput(char[] inputArr)
	{
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

					EmitSignal(nameof(StateFinished), comGat.state);
					return;
				}
			}
		}
		foreach (NormalGatling normGat in normalGatlings)
		{
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

				if (normGat.postCall != null)
				{
					normGat.postCall();
				}

				EmitSignal(nameof(StateFinished), normGat.state);
				return;
			}
		}
	}

	/// <summary>
	/// Just advances the frameCount, please make a base. call anyways though!
	/// </summary>
	public virtual void FrameAdvance()
	{
		frameCount++;
		if (slowdownSpeed != 0) SlowDown();
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
			owner.velocity = new Vector2(owner.velocity.x - slowdownSpeed * mod, 0);

		}
	}

	/// <summary>
	/// Called if the other player is found in this hurtbox
	/// </summary>
	public virtual void InHurtbox(Vector2 collisionPnt)
	{


	}

	/// <summary>
	/// Determines which hitconfirm state to enter
	/// </summary>
	/// <param name="knockdown"></param>
	/// <param name="launch"></param>
	protected virtual void EnterHitState(bool knockdown, Vector2 launch, Vector2 collisionPnt, BaseAttack.EXTRAEFFECT effect)
	{
		GetNode<Node>("/root/Globals").EmitSignal(nameof(PlayerFXEmitted), collisionPnt, "hit", false);
		bool launchBool = false;
		
		owner.ComboUp();
		if (!(launch == Vector2.Zero)) // LAUNCH NEEDS MORE WORK
		{
			//GD.Print("Launch is not zero!");
			owner.velocity = launch;
			launchBool = true;
		}

		bool airState = (launchBool || !owner.grounded);

		if (effect == BaseAttack.EXTRAEFFECT.GROUNDBOUNCE)
		{
			EmitSignal(nameof(StateFinished), "GroundBounce");
		}
		else if (airState && !knockdown)
		{
			if (launch.y == 0)
			{
				launch.y = -400;
			}
			EmitSignal(nameof(StateFinished), "Float");
		}
		else if (airState && knockdown)
		{
			EmitSignal(nameof(StateFinished), "AirKnockdown");
		}
		else if (!airState && knockdown)
		{
			EmitSignal(nameof(StateFinished), "Knockdown");
			
		}
		else
		{
			EmitSignal(nameof(StateFinished), "HitStun");
		}
	}

	protected virtual void EnterBlockState(string stateName, Vector2 collisionPnt)
	{
		GetNode<Node>("/root/Globals").EmitSignal(nameof(PlayerFXEmitted), collisionPnt, "block", false);
		EmitSignal(nameof(StateFinished), stateName);
	}

	public virtual void ReceiveHit(BaseAttack.ATTACKDIR attackDir, HEIGHT height, int hitPush, Vector2 launch, bool knockdown, Vector2 collisionPnt, BaseAttack.EXTRAEFFECT effect)
	{
		owner.velocity = new Vector2(0, 0);
		switch (attackDir)
		{
			case BaseAttack.ATTACKDIR.RIGHT:
				break;
			case BaseAttack.ATTACKDIR.LEFT:
				launch.x *= -1;
				hitPush *= -1;
				break;
			case BaseAttack.ATTACKDIR.EQUAL:
				launch.x = 0;
				hitPush = 0;
				break;
		}
		

		owner.hitPushRemaining = hitPush;
		//GD.Print($"Setting hitPush in {Name} to {owner.hitPushRemaining}");

		if (owner.velocity.y < 0)
		{
			owner.grounded = false;
		}

		bool rightBlock = attackDir == BaseAttack.ATTACKDIR.RIGHT && owner.CheckHeldKey('6');
		bool leftBlock = attackDir == BaseAttack.ATTACKDIR.LEFT && owner.CheckHeldKey('4');
		bool anyBlock = attackDir == BaseAttack.ATTACKDIR.EQUAL && (owner.CheckHeldKey('4') || owner.CheckHeldKey('6'));

		if (height == HEIGHT.HIGH) 
		{
			if (!owner.CheckHeldKey('2'))
			{
				if (rightBlock || leftBlock || anyBlock)
				{
					EnterBlockState("Block", collisionPnt);
				}
				else
				{
					EnterHitState(knockdown, launch, collisionPnt, effect);
				}
			}
			else
			{
				EnterHitState(knockdown, launch, collisionPnt, effect);
			}
			
		}
		else if (height == HEIGHT.LOW) 
		{
			if (owner.CheckHeldKey('2') && owner.grounded)
			{
				if (rightBlock || leftBlock || anyBlock)
				{
					EnterBlockState("CrouchBlock", collisionPnt);
				}
				else
				{
					EnterHitState(knockdown, launch, collisionPnt, effect);
				}
			}
			else
			{
				EnterHitState(knockdown, launch, collisionPnt, effect);
			}
		}
		else
		{
			if (rightBlock || leftBlock || anyBlock)
			{
				EnterBlockState("Block", collisionPnt);
			}
			else 
			{
				EnterHitState(knockdown, launch, collisionPnt, effect);
			}
		}
	}

	public virtual void receiveStun(int hitStun, int blockStun)
	{
		stunRemaining = hitStun;
	}

	/// <summary>
	/// This is multiplied by the proration value that is currently set, THEN the new proration is calculated for future moves
	/// </summary>
	/// <param name="dmg"></param>
	public virtual void receiveDamage(int dmg, int prorationLevel)
	{
		owner.DeductHealth(dmg * owner.proration);
		owner.Prorate(prorationLevel);
	}

	public virtual bool LevelUp()
	{
		return false;
	}
}
