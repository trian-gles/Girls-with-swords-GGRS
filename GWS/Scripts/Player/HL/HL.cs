using Godot;
using System;
using System.Collections.Generic;

public class HL : Player
{

	public bool hatted = true;
	public char hatKey = ' ';
	public Vector2 hatCoors = new Vector2(0, 0);
	public override void _EnterTree()
	{
		base._EnterTree();
		// Super
		

		//6p
		commandNormals.Add(new CommandNormal(new List<char>() { '6', '4' }, 'p', "6P"));
		//commandNormals.Add(new CommandNormal(new List<char>() { '6', '4' }, 'k', "6K"));
		//commandNormals.Add(new CommandNormal(new List<char>() { '6', '4' }, 's', "6S"));

		easyCommandSpecials.Add(new CommandNormal(new List<char>() { '6', '4' }, 'a', "UpHat"));
		easyCommandSpecials.Add(new CommandNormal(new List<char>() { '2', '2' }, 'a', "UpUpHat", true));
		//easyCommandSpecials.Add(new CommandNormal(new List<char>() { '4', '6' }, 'a', "Feint"));
		easySpecial = "Hadouken";

		easyAirSpecial = "JoeRogan";


		//DP
		groundSpecials.Add(new Special(new List<char[]>() { new char[] { '6', 'r' }, new char[] { '2', 'p' }, new char[] { '6', 'p' }, new char[] { 's', 'p' } }, "DP"));
		groundSpecials.Add(new Special(new List<char[]>() { new char[] { '2', 'p' }, new char[] { '6', 'r' }, new char[] { '6', 'p' }, new char[] { 's', 'p' } }, "DP"));

		//Hadouken
		groundSpecials.Add(new Special(new List<char[]>() { new char[] { '6', 'p' }, new char[] { '2', 'r' }, new[] { 'p', 'p' } }, "Hadouken"));
		groundSpecials.Add(new Special(new List<char[]>() { new char[] { '6', 'p' }, new char[] { '2', 'r' }, new[] { 'k', 'p' } }, "UpHat"));
		groundSpecials.Add(new Special(new List<char[]>() { new char[] { '6', 'p' }, new char[] { '2', 'r' }, new[] { 's', 'p' } }, "UpUpHat"));

		groundSpecials.Add(new Special(new List<char[]>() { new char[] { '4', 'p' }, new char[] { '2', 'r' }, new[] { 'p', 'p' } }, "JoeRogan"));

	}
	public override void _Ready()
	{
		negEdgeCallback = (char releasedInp) =>
		{
			if (releasedInp == hatKey)
			{
				CommandHadouken("Hat", HadoukenPart.ProjectileCommand.StopHat);
			}
		};
		GD.Print("Calling HL Ready");
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
		var dict = new Dictionary<string, int>();
		dict["hatted"] = hatted ? 1: 0;
		dict["hatKey"] = hatKey;
		dict["hattx"] = (int)hatCoors.x;
		dict["hatty"] = (int)hatCoors.y;
		return dict;
	}

	protected override void SetStateCharSpecific(Dictionary<string, int> dict)
	{
		if (dict != null)
		{
			hatted = (dict["hatted"] == 1);
			hatCoors.x = dict["hattx"];
			hatCoors.y = dict["hatty"];
			hatKey = Convert.ToChar(dict["hatKey"]);

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

		base.FrameAdvance();
	}

	public override List<Rect2> GetRects(Area2D area, bool globalPosition = false)
	{
		List<Rect2> allRects = new List<Rect2>();
		int i = 0;
		foreach (CollisionShape2D colShape in area.GetChildren())
		{
			i++;
			if (!hatted && i == 2) continue; // the second box is for the hat
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
