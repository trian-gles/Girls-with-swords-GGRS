using Godot;
using System;
using System.Collections.Generic;

public class HL : Player
{
	private const string HatString = "Hat";

	public bool hatted = true;
	public Vector2 hatCoors = new Vector2(0, 0);
	public override void _EnterTree()
	{
		base._EnterTree();
		// Super
		

		//6p
		commandNormals.Add(new CommandNormal(new List<char>() { '6', '4' }, 'p', "6P"));
		commandNormals.Add(new CommandNormal(new List<char>() { '6', '4' }, 'k', "6K"));
		commandNormals.Add(new CommandNormal(new List<char>() { '6', '4' }, 's', "6S"));

		//j2C
		airCommandNormals.Add(new CommandNormal(new List<char>() { '2', '2' }, 's', "J2C"));

		easyCommandSpecials.Add(new CommandNormal(new List<char>() { '6', '4' }, 'a', "DP"));
		easyCommandSpecials.Add(new CommandNormal(new List<char>() { '4', '6' }, 'a', "UpHat", false, true));
		easyCommandSpecials.Add(new CommandNormal(new List<char>() { '8', '8' }, 'a', "JoeRogan"));
		easyCommandSpecials.Add(new CommandNormal(new List<char>() { '2', '2' }, 'a', "UpUpHat", true));
		easySpecial = "Hadouken";

		easyAirSpecial = "JoeRogan";

		easySuper = "Super";

		dashSpecials.Add(new Special(new InputContainer(new[]{ new char[] { 's', 'p' } }), "DashAttack"));

	}
	public override void _Ready()
	{
		base._Ready();
		charName = "HL";
	}


	private const int HATTEDINDEX = 0;
	private const int HATXINDEX = 1;
	private const int HATYINDEX = 2;
	protected override int[] GetStateCharSpecific()
	{
		charSpecificData[HATTEDINDEX] = hatted ? 1: 0;
		charSpecificData[HATXINDEX] = (int)hatCoors.x;
		charSpecificData[HATYINDEX] = (int)hatCoors.y;
		return charSpecificData;
	}

	protected override void SetStateCharSpecific(int[] newCharSpecificData)
	{
		if (newCharSpecificData != null)
		{
			hatted = Convert.ToBoolean(newCharSpecificData[HATTEDINDEX]);
			hatCoors.x = newCharSpecificData[HATXINDEX];
			hatCoors.y = newCharSpecificData[HATYINDEX];
			
		}	
	}

	public override void Reset()
	{
		base.Reset();
		hatted = true;
	}

	public void WarpToHat()
	{
		internalPos = hatCoors * 100;
		Position = hatCoors;
		hatted = true;
	}

	public override void FrameAdvance()
	{
		if (!hatted)
			frontSprite.Visible = false;
		else
			frontSprite.Visible = true;

		if (!hatted && grounded && !CheckHeldKey('2'))
		{
			if (CheckHeldKey('s'))
			{
					CommandHadouken(HatString, HadoukenPart.ProjectileCommand.MoveHatRight);
			}

			if (CheckHeldKey('k'))
			{
					CommandHadouken(HatString, HadoukenPart.ProjectileCommand.MoveHatLeft);
			}
		}


		base.FrameAdvance();
	}

		public override bool GetRects(Godot.Collections.Array<CollisionShape2D> colShapes, Rect2[] array, bool globalPosition = false) 
	{
		bool active = false;
		for (int i = 0; i < colShapes.Count; i++) 
		{
			var colShape = colShapes[i];
			if (!colShape.Disabled && (hatted || i == 0)){
				array[i] = GetRect(colShape, globalPosition);
				active = true;
			}
			else
			{
				array[i] = new Rect2(); // TODO : make sure this isn't breaking anything
			}
		}

		return active;
	}

	//  // Called every frame. 'delta' is the elapsed time since the previous frame.
	//  public override void _Process(float delta)
	//  {
	//      
	//  }
}
