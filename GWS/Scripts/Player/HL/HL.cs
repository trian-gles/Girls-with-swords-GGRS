using Godot;
using System;
using System.Collections.Generic;

public class HL : Player
{

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
		//easyCommandSpecials.Add(new CommandNormal(new List<char>() { '4', '6' }, 'a', "Feint"));
		easySpecial = "Hadouken";

		easyAirSpecial = "JoeRogan";

		easySuper = "Super";

		dashSpecials.Add(new Special(new List<char[]>() { new char[] { 's', 'p' } }, "DashAttack"));


		//DP
		//groundSpecials.Add(new Special(new List<char[]>() { new char[] { '6', 'r' }, new char[] { '2', 'p' }, new char[] { '6', 'p' }, new char[] { 's', 'p' } }, "DP"));
		//groundSpecials.Add(new Special(new List<char[]>() { new char[] { '2', 'p' }, new char[] { '6', 'r' }, new char[] { '6', 'p' }, new char[] { 's', 'p' } }, "DP"));

		//Hadouken
		//groundSpecials.Add(new Special(new List<char[]>() { new char[] { '6', 'p' }, new char[] { '2', 'r' }, new[] { 'p', 'p' } }, "Hadouken"));
		//groundSpecials.Add(new Special(new List<char[]>() { new char[] { '6', 'p' }, new char[] { '2', 'r' }, new[] { 'k', 'p' } }, "UpHat"));
		//groundSpecials.Add(new Special(new List<char[]>() { new char[] { '6', 'p' }, new char[] { '2', 'r' }, new[] { 's', 'p' } }, "UpUpHat"));

		//groundSpecials.Add(new Special(new List<char[]>() { new char[] { '4', 'p' }, new char[] { '2', 'r' }, new[] { 'p', 'p' } }, "JoeRogan"));

	}
	public override void _Ready()
	{
		//GD.Print("Calling HL Ready");
		base._Ready();
		charName = "HL";


		//AddAltState("CrouchB");
		//AddAltState("Walk");
		//AddAltState("Slash");
		//AddAltState("Crouch");
		//AddAltState("Idle");
	}

	protected override Dictionary<string, int> GetStateCharSpecific()
	{
		charSpecificData["hatted"] = hatted ? 1: 0;
		charSpecificData["hattx"] = (int)hatCoors.x;
		charSpecificData["hatty"] = (int)hatCoors.y;
		return charSpecificData;
	}

	protected override void SetStateCharSpecific(Dictionary<string, int> dict)
	{
		if (dict != null)
		{
			hatted = (dict["hatted"] == 1);
			hatCoors.x = dict["hattx"];
			hatCoors.y = dict["hatty"];

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
					CommandHadouken("Hat", HadoukenPart.ProjectileCommand.MoveHatRight);
			}

			if (CheckHeldKey('k'))
			{
					CommandHadouken("Hat", HadoukenPart.ProjectileCommand.MoveHatLeft);
			}
		}


		base.FrameAdvance();
	}

	public override List<Rect2> GetRects(Area2D area, bool globalPosition = false)
	{
		List<Rect2> allRects = new List<Rect2>();
		int i = 0;
		foreach (CollisionShape2D colShape in area.GetChildren())
		{
			i++;
			if (!hatted && i > 1) continue; // the second and third boxes are for the hat
			if (!colShape.Disabled)
			{
				allRects.Add(GetRect(colShape, globalPosition));
			}
			
		}
		return allRects;
	}

	//  // Called every frame. 'delta' is the elapsed time since the previous frame.
	//  public override void _Process(float delta)
	//  {
	//      
	//  }
}
