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

		groundSpecials.Add(new Special(new List<char[]>() { new char[] { '4', 'p' }, new char[] { '2', 'r' }, new[] { 'p', 'p' } }, "Teleport"));


		//DP
		groundSpecials.Add(new Special(new List<char[]>() { new char[] { '6', 'r' }, new char[] { '2', 'p' }, new char[] { '6', 'p' }, new char[] { 's', 'p' } }, "DP"));
		groundSpecials.Add(new Special(new List<char[]>() { new char[] { '2', 'p' }, new char[] { '6', 'r' }, new char[] { '6', 'p' }, new char[] { 's', 'p' } }, "DP"));

		//Hadouken
		groundSpecials.Add(new Special(new List<char[]>() { new char[] { '6', 'p' }, new char[] { '2', 'r' }, new[] { 'p', 'p' } }, "Hadouken"));
		groundSpecials.Add(new Special(new List<char[]>() { new char[] { '6', 'p' }, new char[] { '2', 'r' }, new[] { 'k', 'p' } }, "UpHat"));

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

	//  // Called every frame. 'delta' is the elapsed time since the previous frame.
	//  public override void _Process(float delta)
	//  {
	//      
	//  }
}
