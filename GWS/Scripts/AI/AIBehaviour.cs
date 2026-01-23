using Godot;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Linq;


public class AIBehaviour
{
	public int lastInp = 0;

	private int lastSlashFrame = 0;
	private int lastKickFrame = 0;

	private int frame = 0;

	public static unsafe bool CheckP1CurrentState(GameStateObjectRedesign.GameState state, string comparison)
	{
		return new string(state.P1State.currentState) == comparison;
	}

	public static unsafe bool CheckP2CurrentState(GameStateObjectRedesign.GameState state, string comparison)
	{
		return new string(state.P2State.currentState) == comparison;
	}

	public static unsafe string GetP1CurrentState(GameStateObjectRedesign.GameState state)
	{
		return new string(state.P1State.currentState);
	}

	public static unsafe string GetP2CurrentState(GameStateObjectRedesign.GameState state)
	{
		return new string(state.P2State.currentState);
	}

	public Globals.CHARID controlledChar;
	private BehaviourState behaviour;
	public GameStateObjectRedesign.GameState savedState;
	private string behaviourName;
	private Random random = new Random();
	private Dictionary<string, BehaviourState> behaviourStates = new Dictionary<string, BehaviourState>
	{
		{"WakeupAbare", new WakeupAbare() },
		{"Combo", new Combo() },
		{"RandomMash", new RandomMash() },
		{"Mixup", new Mixup() },
		{ "Chase", new Chase() },
		{"FloatTech", new FloatTech() },
		{"Zone", new Zone() },
		{"Oki", new Oki() },
		{"WakeupBlock", new WakeupBlock() },
		{"WakeupBackdash", new WakeupBackdash() },
		{"WakeupReversal", new WakeupReversal()}
	};

	// Global behaviours that must be saved here
	public static HashSet<string> floatStates = new HashSet<string>() { "Float", "WallBounce", "GroundBounce" };

	public static HashSet<string> groundHitConfirmStates = new HashSet<string>
	{
		"HitStun",
		"Stagger"
	};

	public static HashSet<string> mixupConfirmStates = new HashSet<string>
	{
		"Block",
		"CrouchBlock"
	};

	public static HashSet<string> airConfirmStates = new HashSet<string>
	{
		"Float",
		"GroundBounce",
		"WallBounce"
	};

	public static HashSet<string> kickStates = new HashSet<string>
	{
		"Kick",
		"6K"
	};

	public static HashSet<string> slashStates = new HashSet<string>
	{
		"Slash",
		"6C"
	};

	public AIBehaviour(int charId)
	{
		foreach (var b in behaviourStates.Values)
		{
			b.Init(this);
		}

		controlledChar = (Globals.CHARID) charId;

		behaviour = behaviourStates["Chase"];
		behaviourName = "Chase";
		behaviour.Init(this);
	}
	public int Poll(GameStateObjectRedesign.GameState state)
	{
		savedState = state;
		frame = state.frame;
		string nextState = behaviour.GetNextState(state);
		// Global handling which must be done here
		if (floatStates.Contains(GetP2CurrentState(state)))
			nextState = "FloatTech";

		if (CheckP2CurrentState(state, "Knockdown") && behaviourName.Substr(0, 6) != "Wakeup")
		{
			switch (random.Next(4))
			{
				case 0:
					nextState = "WakeupAbare";
					break;
				case 1:
					nextState = "WakeupBackdash";
					break;
				case 2:
					if (controlledChar == Globals.CHARID.OLID || controlledChar == Globals.CHARID.HLID)
						nextState = "WakeupReversal";
					else
						nextState = "WakeupBlock";
					break;
				default:
					nextState = "WakeupBlock";
					break;
			}

		}

		// 

		if (nextState != "")
		{
			EnterState(nextState);

		}

		int input = behaviour.Poll(state);
		if ((input & Globals.SLASH) != 0)
			lastSlashFrame = state.frame;
		if ((input & Globals.KICK) != 0)
			lastKickFrame = state.frame;
		lastInp = input;
		return input;
	}

	private void EnterState(string nextState)
	{
		behaviour.Exit();
		behaviour = behaviourStates[nextState];
		behaviourName = nextState;
		behaviour.Enter();
	}

	public bool CanKickWithoutGrabbing()
	{
		return (frame - lastSlashFrame > 7);
	}

	public bool CanSlashWithoutGrabbing()
	{
		return (frame - lastKickFrame > 7);
	}
}
