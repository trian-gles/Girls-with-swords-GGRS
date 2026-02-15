using Godot;
using System;
using System.Collections.Generic;
using System.Linq;


/// <summary>
/// Collection of constants and static functions
/// </summary>
/// 
public class Globals : Node
{
	[Signal]
	public delegate void GhostEmitted(Player p);

	[Signal]
	public delegate void PlayerFXEmitted(Vector2 location, string particleName, bool flipH);

	[Signal]
	public delegate void LocalLobbyReturn();

	[Signal]
	public delegate void NetPlayLobbyReturn();

	public static string VERSION = "1.0.0";
	public static string netplaySessionName = "";
	public static int ROLLBACKDEPTH = 8;

	public static int frame = 0;
	public static int lastConfirmedFrame = 0;
	public static int rollbackFrame = 0;
	public static bool hosting;

	private static string loggingName;
	public static bool logOn = false;

	public static bool alwaysBlock = false;
	public static bool autoTech = false;

	public static bool DISABLESFX = false;
	public static bool DISABLEGFX = false;
	public static bool DISABLEPARTICLES = false;
	public static bool DISABLESHAKE = false;

	public static AIDIFFICULTY aiDifficulty = AIDIFFICULTY.HI;

	public static Player[] P1Characters = new Player[4];
	public static Player[] P2Characters = new Player[4];

	public static void GenerateCharacters(PackedScene[] playerScenes)
	{
		for (int i = 0; i < playerScenes.Length; i++)
		{
			P1Characters[i] = playerScenes[i].Instance() as Player;
			P2Characters[i] = playerScenes[i].Instance() as Player;
		}
	}

	public enum AIDIFFICULTY
	{
		LO, HI
	}


	public enum CHARID
	{
		OLID, GLID, SLID, HLID
	}

	public const string P1UPACTION = "8";
	public const string P1DOWNACTION = "2";
	public const string P1RIGHTACTION = "6";
	public const string P1LEFTACTION = "4";
	public const string P1PUNCHACTION = "p";
	public const string P1KICKACTION = "k";
	public const string P1SLASHACTION = "s";
	public const string P1SPECIALACTION = "a";
	public const string P1STRINGACTION = "b";
	public const string P1DASHACTION = "c";
	public const string P2UPACTION = "8b";
	public const string P2DOWNACTION = "2b";
	public const string P2RIGHTACTION = "6b";
	public const string P2LEFTACTION = "4b";
	public const string P2PUNCHACTION = "pb";
	public const string P2KICKACTION = "kb";
	public const string P2SLASHACTION = "sb";
	public const string P2SPECIALACTION = "ab";
	public const string P2STRINGACTION = "bb";
	public const string P2DASHACTION = "cb";
	
	public const int UP = 1;
	public const int DOWN = 2;
	public const int RIGHT = 4;
	public const int LEFT = 8;
	public const int PUNCH = 16;
	public const int KICK = 32;
	public const int SLASH = 64;
	public const int SPECIAL = 128;
	public const int STRING = 256;
	public const int DASH = 512;


	public static char[] RIGHTPRESS = new[] {'6', 'p'};
	public static char[] LEFTPRESS = new[] {'4', 'p'};
	public static char[] UPPRESS = new[] {'8', 'p'};
	public static char[] DOWNPRESS = new[] {'2', 'p'};
	public static char[] JABPRESS = new[] {'p', 'p'};
	public static char[] KICKPRESS = new[] {'k', 'p'};
	public static char[] SLASHPRESS = new[] {'s', 'p'};
	public static char[] SPECIALPRESS = new[] {'a', 'p'};
	public static char[] STRINGPRESS = new[] {'b', 'p'};
	public static char[] DASHPRESS = new[] {'c', 'p'};

	public static char[] RIGHTREL = new[] {'6', 'r'};
	public static char[] LEFTREL = new[] {'4', 'r'};
	public static char[] UPREL = new[] {'8', 'r'};
	public static char[] DOWNREL = new[] {'2', 'r'};
	public static char[] JABREL = new[] {'p', 'r'};
	public static char[] KICKREL = new[] {'k', 'r'};
	public static char[] SLASHREL = new[] {'s', 'r'};
	public static char[] SPECIALREL = new[] {'a', 'r'};
	public static char[] STRINGREL = new[] {'b', 'r'};
	public static char[] DASHREL = new[] {'c', 'r'};
	static public Mode mode;

	public static List<string> logBuffer = new List<string>();
	public static void SetLogging(string loggingName)
	{
		Globals.loggingName = loggingName;
		logOn = true;
	}

	public static string lastLogMessage = "";

	public static long TestGC1()
	{
		return GC.GetTotalMemory(false); // allocated managed memory
	}

	public static bool TESTGC = false;

	public static void TestGC2(long mem)
	{
		mem = GC.GetTotalMemory(false) - mem;
		if (mem > 0)
			GD.Print($"DELTA MEM: {mem / 1024} KB, Gen0: {GC.CollectionCount(0)}, Gen2: {GC.CollectionCount(2)}");
	}

	public static void TestGC2(long mem, string why)
	{
		mem = GC.GetTotalMemory(false) - mem;
		if (mem > 0)
		{
			GD.Print($"DELTA MEM {why}: {mem / 1024} KB, Gen0: {GC.CollectionCount(0)}, Gen2: {GC.CollectionCount(2)}");
			Log($"DELTA MEM {why}: {mem / 1024} KB, Gen0: {GC.CollectionCount(0)}, Gen2: {GC.CollectionCount(2)}");
		}
			
	}
	public static void Log(string msg)
	{
		lastLogMessage = msg;
		if (rollbackFrame != 0)
		{
			msg = rollbackFrame + " : " + msg;
		}

		string logMsg = frame + " : " + loggingName + " : " + msg;

		if (logOn)
			logBuffer.Add(logMsg);

		
	}


	public enum PlayerSignal
	{
		HealthSet,
		MeterChanged,
		BurstSet,
		ComboSet,
		ComboChanged,
		HealthChanged,
		Counter,
		Mixup,
		CanTech,
		MissedTech,
		SuperFlash

	}

	public delegate void PlayerSingleArgSignalListener(string name, int arg);
	public delegate void PlayerNoArgSignalListener(string name);
	public delegate void PlayerSignalListener(Player p);
	public delegate void GFXParticleSignalListener(Vector2 location, string particleName, bool flipH);
	private static Dictionary<PlayerSignal, PlayerSingleArgSignalListener> singleArgSignalListeners = new Dictionary<PlayerSignal, PlayerSingleArgSignalListener>();
	private static Dictionary<PlayerSignal, PlayerNoArgSignalListener> noArgSignalListeners = new Dictionary<PlayerSignal, PlayerNoArgSignalListener>();
	private static List<PlayerSignalListener> ghostListeners = new List<PlayerSignalListener>();
	private static List<GFXParticleSignalListener> gfxParticleListeners = new List<GFXParticleSignalListener>();

	public static void ConnectPlayerSingleArgSignalListener(
		PlayerSignal signal,
		PlayerSingleArgSignalListener listener
	)
	{
		if (!singleArgSignalListeners.ContainsKey(signal))
		{
			singleArgSignalListeners[signal] = null;
		}
		else
		{
			GD.PrintErr("UHOH, DOUBLE CONNECTING SIGNAL " + signal.ToString());
		}
		singleArgSignalListeners[signal] += listener;
	}

	public static void ConnectPlayerNoArgSignalListener(
		PlayerSignal signal,
		PlayerNoArgSignalListener listener
	)
	{
		if (!noArgSignalListeners.ContainsKey(signal))
		{
			noArgSignalListeners[signal] = null;
		}
		else
		{
			GD.PrintErr("UHOH, DOUBLE CONNECTING SIGNAL " + signal.ToString());
		}
		noArgSignalListeners[signal] += listener;
	}

	public static void EmitSignal(PlayerSignal signal, string name, int arg)
	{
		if (singleArgSignalListeners.ContainsKey(signal))
		{
			singleArgSignalListeners[signal]?.Invoke(name, arg);
		}
		else
		{
			GD.Print("UHOH, EMITTING UNCONNECTED SIGNAL " + signal.ToString());
		}
	}

	public static void EmitSignal(PlayerSignal signal, string name)
	{
		if (noArgSignalListeners.ContainsKey(signal))
		{
			noArgSignalListeners[signal]?.Invoke(name);
		}
		else
		{
			GD.Print("UHOH, EMITTING UNCONNECTED NO-ARG SIGNAL " + signal.ToString());
		}
	}


	public static void ConnectGhostEmitted(PlayerSignalListener listener)
	{
		ghostListeners.Add(listener);
	}

	public static void ConnectGFXParticleEmitted(GFXParticleSignalListener listener)
	{
		gfxParticleListeners.Add(listener);
	}

	public static void EmitGhostEmitted(Player p)
	{
		foreach (var listener in ghostListeners)
		{
			listener(p);
		}
	}

	public static void EmitPlayerFXEmitted(Vector2 location, string particleName, bool flipH)
	{
		foreach (var listener in gfxParticleListeners)
		{
			listener(location, particleName, flipH);
		}
	}

	public static void ClearSignals()
	{
		noArgSignalListeners.Clear();
		singleArgSignalListeners.Clear();
		ghostListeners.Clear();
		gfxParticleListeners.Clear();
	}
	

	public const int rightWall = 46500;
	public const int leftWall = 1500;
	public const int floor = 22000;

	public const int MAXAIRDASHDEPTH = 21200;

	public const int MAXJPDEPTH = 18000;
	public enum Inputs
	{
		UP = 1,
		DOWN = 2,
		LEFT = 3,
		RIGHT = 4,
		PUNCH = 5,
		KICK = 6,
		SLASH = 7
	}

	public enum Press
	{
		PRESS = 0,
		RELEASE = 1
	}

	public enum Mode
	{
		LOCAL = 0,
		TRAINING = 1,
		GGPO = 2,
		SYNCTEST = 3,
		CPU = 4,
		TUTORIAL = 5
	}

	public enum Tags
	{
		attack,	aerial, hitstate, tech, block, crouching, recovery, grab, idle, knockdown, @float, run, 
		jab, kick, slash, special, movestate
	}

	public struct AttackDetails
	{
		public bool projectile;
		public int hitStun;
		public int blockStun;
		public int dmg;
		public int hitPush;
		public int prorationLevel;
		public bool ignoreProration;
		public bool knockdown;
		public bool electrocute;
		public bool removeOTG;
		public int hitStop;
		public Vector2 collisionPnt;
		public Vector2 opponentLaunch;
		public BaseAttack.EXTRAEFFECT effect;
		public BaseAttack.GRAPHICEFFECT graphicFX;
		public BaseAttack.HEIGHT height;
		public BaseAttack.ATTACKDIR dir;
		public bool airBlockable;
		public bool spike;
		public bool chipDmg;
	}

	public struct AttackLevel
	{
		public AttackDetails hit;
		public AttackDetails counterHit;
		public int counterStopFrames;
	}

	public static AttackDetails otgHit = new AttackDetails
	{
		hitStun = 9,
		blockStun = 12,
		hitStop = 12,
		dmg = 4,
		hitPush = 6000,
		prorationLevel = 4,
		knockdown = false,
		opponentLaunch = new Vector2(400, -300),
		effect = BaseAttack.EXTRAEFFECT.NONE,
		graphicFX = BaseAttack.GRAPHICEFFECT.NONE,
		height = State.HEIGHT.MID,
		dir = BaseAttack.ATTACKDIR.EQUAL
	};

	public static AttackDetails electrocuteDetails = new AttackDetails
	{
		hitStun = 20,
		blockStun = 18,
		hitStop = 0,
		dmg = 7,
		hitPush = 4000,
		prorationLevel = 1,
		knockdown = false,
		effect = BaseAttack.EXTRAEFFECT.NONE,
		graphicFX = BaseAttack.GRAPHICEFFECT.SPARKS,
		height = State.HEIGHT.MID,
		dir = BaseAttack.ATTACKDIR.EQUAL,
		opponentLaunch = new Vector2(0, -700),
	};
	public static AttackLevel[] attackLevels = new AttackLevel[]
	{
		// LVL 1
		new AttackLevel {
			hit = new AttackDetails{
				hitStun = 12,
				blockStun = 11,
				hitStop = 12,
				dmg = 4,
				hitPush = 2000,
				prorationLevel = 4,
				knockdown = false,
				opponentLaunch = Vector2.Zero,
				effect = BaseAttack.EXTRAEFFECT.NONE,
				graphicFX = BaseAttack.GRAPHICEFFECT.NONE,
				height = State.HEIGHT.MID,
				dir = BaseAttack.ATTACKDIR.EQUAL

			},
			counterHit = new AttackDetails{
				hitStun = 16,
				blockStun = 11,
				hitStop = 16,
				dmg = 4,
				hitPush = 2000,
				prorationLevel = 1,
				knockdown = false,
				opponentLaunch = Vector2.Zero,
				effect = BaseAttack.EXTRAEFFECT.NONE,
				graphicFX = BaseAttack.GRAPHICEFFECT.NONE,
				height = State.HEIGHT.MID,
				dir = BaseAttack.ATTACKDIR.EQUAL
			},
			counterStopFrames = 2
		},
		
		// LVL 2
		new AttackLevel {
			hit = new AttackDetails{
				hitStun = 14,
				blockStun = 13,
				hitStop = 14,
				dmg = 5,
				hitPush = 2500,
				prorationLevel = 3,
				knockdown = false,
				opponentLaunch = Vector2.Zero,
				effect = BaseAttack.EXTRAEFFECT.NONE,
				graphicFX = BaseAttack.GRAPHICEFFECT.NONE,
				height = State.HEIGHT.MID,
				dir = BaseAttack.ATTACKDIR.EQUAL

			},
			counterHit = new AttackDetails{
				hitStun = 18,
				blockStun = 13,
				hitStop = 17,
				dmg = 5,
				hitPush = 2500,
				prorationLevel = 1,
				knockdown = false,
				opponentLaunch = Vector2.Zero,
				effect = BaseAttack.EXTRAEFFECT.NONE,
				graphicFX = BaseAttack.GRAPHICEFFECT.NONE,
				height = State.HEIGHT.MID,
				dir = BaseAttack.ATTACKDIR.EQUAL
			},
			counterStopFrames = 4
		},
		
		// LVL 3
		new AttackLevel {
			hit = new AttackDetails{
				hitStun = 17,
				blockStun = 16,
				hitStop = 15,
				dmg = 6,
				hitPush = 3200,
				prorationLevel = 2,
				knockdown = false,
				opponentLaunch = Vector2.Zero,
				effect = BaseAttack.EXTRAEFFECT.NONE,
				graphicFX = BaseAttack.GRAPHICEFFECT.NONE,
				height = State.HEIGHT.MID,
				dir = BaseAttack.ATTACKDIR.EQUAL

			},
			counterHit = new AttackDetails{
				hitStun = 34,
				blockStun = 16,
				hitStop = 18,
				dmg = 6,
				hitPush = 3200,
				prorationLevel = 0,
				knockdown = false,
				opponentLaunch = Vector2.Zero,
				effect = BaseAttack.EXTRAEFFECT.STAGGER,
				graphicFX = BaseAttack.GRAPHICEFFECT.NONE,
				height = State.HEIGHT.MID,
				dir = BaseAttack.ATTACKDIR.EQUAL
			},
			counterStopFrames = 6
		},
		
		// LVL 4
		new AttackLevel {
			hit = new AttackDetails{
				hitStun = 19,
				blockStun = 18,
				hitStop = 16,
				dmg = 7,
				hitPush = 4000,
				prorationLevel = 1,
				knockdown = false,
				opponentLaunch = Vector2.Zero,
				effect = BaseAttack.EXTRAEFFECT.NONE,
				graphicFX = BaseAttack.GRAPHICEFFECT.NONE,
				height = State.HEIGHT.MID,
				dir = BaseAttack.ATTACKDIR.EQUAL

			},
			counterHit = new AttackDetails{
				hitStun = 38,
				blockStun = 18,
				hitStop = 19,
				dmg = 7,
				hitPush = 4000,
				prorationLevel = 0,
				knockdown = false,
				opponentLaunch = Vector2.Zero,
				effect = BaseAttack.EXTRAEFFECT.NONE,
				graphicFX = BaseAttack.GRAPHICEFFECT.NONE,
				height = State.HEIGHT.MID,
				dir = BaseAttack.ATTACKDIR.EQUAL
			},
			counterStopFrames = 8
		},
		// LVL 5
		new AttackLevel {
			hit = new AttackDetails{
				hitStun = 20,
				blockStun = 19,
				hitStop = 18,
				dmg = 9,
				hitPush = 5000,
				prorationLevel = 0,
				knockdown = false,
				opponentLaunch = Vector2.Zero,
				effect = BaseAttack.EXTRAEFFECT.NONE,
				graphicFX = BaseAttack.GRAPHICEFFECT.NONE,
				height = State.HEIGHT.MID,
				dir = BaseAttack.ATTACKDIR.EQUAL

			},
			counterHit = new AttackDetails{
				hitStun = 38,
				blockStun = 19,
				hitStop = 22,
				dmg = 9,
				hitPush = 5000,
				prorationLevel = 0,
				knockdown = false,
				opponentLaunch = Vector2.Zero,
				effect = BaseAttack.EXTRAEFFECT.NONE,
				graphicFX = BaseAttack.GRAPHICEFFECT.NONE,
				height = State.HEIGHT.MID,
				dir = BaseAttack.ATTACKDIR.EQUAL
			},
			counterStopFrames = 10
		}
	};

	public override void _Ready()
	{
		//Tests();
	}

	public static string GetVersion()
	{
		return VERSION;
	}

	public static void ChangeDifficulty(int dif)
	{
		aiDifficulty = (AIDIFFICULTY)dif;
		GD.Print(aiDifficulty);
	}

	public static bool CheckTrainingMode()
	{
		return mode == Mode.TRAINING; 
	}
	public static void SetAlwaysBlock(bool state)
	{
		alwaysBlock = state;
	}

	public static void SetAutoTech(bool state)
	{
		autoTech = state;
	}
	public static bool IsFrameConfirmed()
	{
		return frame == lastConfirmedFrame;
	}

	public static bool ArrayInList(InputContainer arr, char[] element)
	{
		foreach (var listItem in arr)
		{
			if (CompareInput(listItem, element))
				return true;
		}
		return false;
	}

	private static int[] tempResultBuffer = new int[64];


	private static bool ArraysEqual(char[] a, char[] b)
	{
		if (ReferenceEquals(a, b))
			return true;

		if (a == null || b == null || a.Length != b.Length)
			return false;

		for (int i = 0; i < a.Length; i++)
		{
			if (a[i] != b[i])
				return false;
		}

		return true;
	}


	public static bool CompareInput(char[] i1, char[] i2)
	{
		return i1[0] == i2[0] && i1[1] == i2[1];
	}

	public static bool IsSameOrSubclass(Type potentialBase, Type potentialDescendant)
	{
		return potentialDescendant.IsSubclassOf(potentialBase)
			   || potentialDescendant == potentialBase;
	}

	private static InputContainer tempContainer = new InputContainer(9);
	/// <summary>
	/// Tests if the elements are found in that order but possibly separated by other elements within the array.  
	/// </summary>
	/// <param name="arr"> The array to search in </param>
	/// <param name="elements"> The elements to search for in order </param>
	/// <returns></returns>
	public static bool ArrOfArraysComplexInList(
		InputContainer arr,
		InputContainer elements
	)
	{
		int arrCount = arr.Count;
		int windowSize = Math.Min(arrCount, 9);
		int windowStart = arrCount - windowSize;

		int cursor = -1; // relative to windowStart

		foreach (char[] element in elements)
		{
			bool found = false;

			// search forward from cursor + 1
			for (int i = cursor + 1; i < windowSize; i++)
			{
				int arrIndex = windowStart + i;

				if (ArraysEqual(arr[arrIndex], element))
				{
					if (cursor >= 0 && i - cursor > 9)
						return false;

					cursor = i;
					found = true;
					break;
				}
			}

			if (!found)
				return false;
		}

		return true;
	}


	public static int IntSqrt(int num)
	{
		int min = 0;
		int max = num + 1;
		
		while (true)
		{
			int mid = min + (int)Math.Floor((float)(max - min) / 2);

			int square = mid * mid;

			if (min + 1 == max)
			{
				int mins = Math.Abs(min * min - num);
				int maxs = Math.Abs(max * max - num);

				if (mins > maxs)
					return max;
				else
					return min;

			}
			if (square > num)
			{
				max = mid;
			}
			else if (square < num)
			{
				min = mid;
			}
			else if (square == num)
			{
				return mid;
			}

		}
	}

	public static int BoolToInt(bool a)
	{
		return a ? 1 : 0;
	}

	public static bool IntToBool(int i)
	{
		return i == 1;
	}

	public static int[] IntArcTan(int o, int a)
	{

		int denom = 15 * (int)Math.Pow(a, 5);

		int numer = 15 * o * (int)Math.Pow(a, 4) - 5 * (int)Math.Pow(o, 3) * (int)Math.Pow(a, 2) + 3 * (int)Math.Pow(o, 5);
		return new[] { numer, denom };

	}

	public static bool CheckKeyPress(char[] input, char desiredPress)
	{
		return (input[1] == 'p' && input[0] == desiredPress);
	}

	public static bool CheckKeyRelease(char[] input, char desiredRelease)
	{
		return (input[1] == 'r' && input[0] == desiredRelease);
	}

	public static void Tests()
	{
		
		var arr = new InputContainer(8);
		arr.Add(new char[] { 'p', 'p' });
		arr.Add(new char[] { 'k', 'p' });
		arr.Add(new char[] { 'p', 'r' });
		arr.Add(new char[2]);
		arr.Add(new char[] { 'p', 'p' });
		arr.Add(new char[] { 'k', 'r' });
		arr.Add(new char[] { 'p', 'r' });
		arr.Add(new char[] { '2', 'p' });

		

		GD.Print($"Testing {nameof(ArrOfArraysComplexInList)}");
		var elements = new InputContainer(5);
		elements.Add(new char[] { 'p', 'p' });
		elements.Add(new char[] { 'p', 'p' });
		GD.Print($"Result of testing punch-punch in array = {ArrOfArraysComplexInList(arr, elements)}");

		elements = new InputContainer(5);
		elements.Add(new char[] { 'p', 'p' });
		elements.Add(new char[] { 'k', 'p' });
		GD.Print($"Result of testing punch-kick in array = {ArrOfArraysComplexInList(arr, elements)}");

		elements.Add(new char[] { 's', 'p' });
		bool result = (ArrOfArraysComplexInList(arr, elements) == false);
		GD.Print($"Result of testing nonexistant elements in array = {result}");

		var perms = State.Permutations(new List<char> {'a', 'b', 'c'});
		GD.Print($"Permutations of abc = ");
		foreach (List<char> perm in perms)
		{
			var thing = string.Join(",", perm);
			GD.Print(thing);
		}
	}
}
