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

	public static int frame = 0;

	private static string loggingName;
	private static bool logOn = false;

	public static void SetLogging(string loggingName)
	{
		Globals.loggingName = loggingName;
		logOn = true;
	}

	public static void Log(string msg)
	{
		if (logOn)
			GD.Print(loggingName + " : " + msg);
	}
	
	public const bool rhythmGame = true;

	public const int rightWall = 46500;
	public const int leftWall = 1500;
	public const int floor = 22000;
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
		SYNCTEST = 3
	}

	public struct AttackDetails
	{
		public int hitStun;
		public int blockStun;
		public int dmg;
		public int hitPush;
		public int prorationLevel;
		public bool knockdown;
		public Vector2 collisionPnt;
		public Vector2 opponentLaunch;
		public BaseAttack.EXTRAEFFECT effect;
		public BaseAttack.HEIGHT height;
		public BaseAttack.ATTACKDIR dir;
	}

	public struct AttackLevel
	{
		public AttackDetails hit;
		public AttackDetails counterHit;
	}

	public static AttackLevel[] attackLevels = new AttackLevel[]
	{
		// LVL 1
		new AttackLevel {
			hit = new AttackDetails{
				hitStun = 12,
				blockStun = 11,
				dmg = 4,
				hitPush = 2000,
				prorationLevel = 2,
				knockdown = false,
				opponentLaunch = Vector2.Zero,
				effect = BaseAttack.EXTRAEFFECT.NONE,
				height = State.HEIGHT.MID,
				dir = BaseAttack.ATTACKDIR.EQUAL

			}, 
			counterHit = new AttackDetails{
				hitStun = 16,
				blockStun = 11,
				dmg = 4,
				hitPush = 2000,
				prorationLevel = 2,
				knockdown = false,
				opponentLaunch = Vector2.Zero,
				effect = BaseAttack.EXTRAEFFECT.NONE,
				height = State.HEIGHT.MID,
				dir = BaseAttack.ATTACKDIR.EQUAL
			}
		},
		
		// LVL 2
		new AttackLevel {
			hit = new AttackDetails{
				hitStun = 14,
				blockStun = 13,
				dmg = 5,
				hitPush = 2500,
				prorationLevel = 1,
				knockdown = false,
				opponentLaunch = Vector2.Zero,
				effect = BaseAttack.EXTRAEFFECT.NONE,
				height = State.HEIGHT.MID,
				dir = BaseAttack.ATTACKDIR.EQUAL

			},
			counterHit = new AttackDetails{
				hitStun = 18,
				blockStun = 13,
				dmg = 5,
				hitPush = 2500,
				prorationLevel = 1,
				knockdown = false,
				opponentLaunch = Vector2.Zero,
				effect = BaseAttack.EXTRAEFFECT.NONE,
				height = State.HEIGHT.MID,
				dir = BaseAttack.ATTACKDIR.EQUAL
			}
		},
		
		// LVL 3
		new AttackLevel {
			hit = new AttackDetails{
				hitStun = 17,
				blockStun = 16,
				dmg = 5,
				hitPush = 3200,
				prorationLevel = 1,
				knockdown = false,
				opponentLaunch = Vector2.Zero,
				effect = BaseAttack.EXTRAEFFECT.NONE,
				height = State.HEIGHT.MID,
				dir = BaseAttack.ATTACKDIR.EQUAL

			},
			counterHit = new AttackDetails{
				hitStun = 34,
				blockStun = 16,
				dmg = 7,
				hitPush = 3200,
				prorationLevel = 1,
				knockdown = false,
				opponentLaunch = Vector2.Zero,
				effect = BaseAttack.EXTRAEFFECT.STAGGER,
				height = State.HEIGHT.MID,
				dir = BaseAttack.ATTACKDIR.EQUAL
			}
		},
		
		// LVL 4
		new AttackLevel {
			hit = new AttackDetails{
				hitStun = 19,
				blockStun = 18,
				dmg = 5,
				hitPush = 4000,
				prorationLevel = 0,
				knockdown = false,
				opponentLaunch = Vector2.Zero,
				effect = BaseAttack.EXTRAEFFECT.NONE,
				height = State.HEIGHT.MID,
				dir = BaseAttack.ATTACKDIR.EQUAL

			},
			counterHit = new AttackDetails{
				hitStun = 38,
				blockStun = 18,
				dmg = 7,
				hitPush = 4000,
				prorationLevel = 0,
				knockdown = false,
				opponentLaunch = Vector2.Zero,
				effect = BaseAttack.EXTRAEFFECT.NONE,
				height = State.HEIGHT.MID,
				dir = BaseAttack.ATTACKDIR.EQUAL
			}
		}
	};

	public override void _Ready()
	{
		//Tests();
	}

	static public Mode mode;
	public static bool ArrayInList(List<char[]> arr, char[] element)
	{
		var elementTest = (from e in arr select Enumerable.SequenceEqual(e, element));
		return elementTest.Contains(true);
	}

	public static List<int> PositionsOfArrayInList(List<char[]> arr, char[] element) //this is really dumb I'm sure there's a better way
	{
		var indexes = new List<int>();
		var trueFalse = (from e in arr select Enumerable.SequenceEqual(e, element));
		int i = 0;
		foreach (bool b in trueFalse)
		{
			if (b)
			{
				indexes.Add(i);
			}
			i++;
		}
		return indexes;
	}

	/// <summary>
	/// Tests if the elements are found in that order but possibly separated by other elements within the array.  
	/// </summary>
	/// <param name="arr"> The array to search in </param>
	/// <param name="elements"> The elements to search for in order </param>
	/// <returns></returns>
	public static bool ArrOfArraysComplexInList(List<char[]> arr, List<char[]> elements)
	{
		if (arr.Count > 9) // prevents huge buffers
		{
			arr = arr.GetRange(arr.Count - 9, 9);
		}
		int cursor = -1; // used to make sure moves are in the correct order
		foreach (char[] element in elements)
		{
			List<int> indexes = PositionsOfArrayInList(arr, element);
			if (indexes == null) // The element does not exist in the list
			{
				return false; 
			}
			bool indexFound = false;
			foreach (int index in indexes)
			{
				if (index > cursor)
				{
					
					if (cursor >= 0 && index - cursor > 9) // the buffer is too long.  Prevents unwanted specials
						return false;
					cursor = index;
					indexFound = true;
					break;
				}
			}

			if (!indexFound) // If we can't find the element at a greater index than that of the last
			{
				return false;
			}
		}
		return true;
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
		
		var arr = new List<char[]>();
		arr.Add(new char[] { 'p', 'p' });
		arr.Add(new char[] { 'k', 'p' });
		arr.Add(new char[] { 'p', 'r' });
		arr.Add(new char[2]);
		arr.Add(new char[] { 'p', 'p' });
		arr.Add(new char[] { 'k', 'r' });
		arr.Add(new char[] { 'p', 'r' });
		arr.Add(new char[] { '2', 'p' });

		GD.Print($"Testing {nameof(PositionsOfArrayInList)}");
		var element = new char[] { 'p', 'p' };
		List<int> positions = PositionsOfArrayInList(arr, element);
		GD.Print($"Result of finding indeces of punch-punch in array = {positions.SequenceEqual(new List<int> { 0, 4 })}");


		GD.Print($"Testing {nameof(ArrOfArraysComplexInList)}");
		var elements = new List<char[]>();
		elements.Add(new char[] { 'p', 'p' });
		elements.Add(new char[] { 'p', 'p' });
		GD.Print($"Result of testing punch-punch in array = {ArrOfArraysComplexInList(arr, elements)}");

		elements = new List<char[]>();
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
