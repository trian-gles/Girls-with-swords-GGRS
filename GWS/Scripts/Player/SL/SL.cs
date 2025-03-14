using Godot;
using System;
using System.Collections.Generic;

public class SL : Player
{
	public bool leftCornerSnail = false;
	public bool rightCornerSnail = false;
	public override void _EnterTree()
	{
		base._EnterTree();
		groundExSpecials.Add(new Special(new List<char[]>() { new char[] { '6', 'p' }, new char[] { '2', 'r' }, new char[] { '2', 'p' }, new char[] { '6', 'p' }, new char[] { 's', 'p' } }, "SnailStrike"));
		groundExSpecials.Add(new Special(new List<char[]>() { new char[] { '2', 'p' }, new char[] { '6', 'p' }, new char[] { '2', 'p' }, new char[] { '2', 'r' }, new char[] { 's', 'p' } }, "SnailStrike"));
		groundExSpecials.Add(new Special(new List<char[]>() { new char[] { '6', 'p' }, new char[] { '2', 'r' }, new char[] { '2', 'p' }, new char[] { '2', 'r' }, new char[] { 's', 'p' } }, "SnailStrike"));
		groundExSpecials.Add(new Special(new List<char[]>() { new char[] { '2', 'p' }, new char[] { '6', 'p' }, new char[] { '2', 'p' }, new char[] { '6', 'p' }, new char[] { 's', 'p' } }, "SnailStrike"));

		commandNormals.Add(new CommandNormal(new List<char>() { '6', '4' }, 's', "6C"));
		commandNormals.Add(new CommandNormal(new List<char>() { '6', '4' }, 'k', "6K"));
		commandNormals.Add(new CommandNormal(new List<char>() { '6', '4' }, 'p', "6P"));

		groundSpecials.Add(new Special(new List<char[]>() { new char[] { '4', 'p' }, new char[] { '2', 'r' }, new[] { 's', 'p' } }, "SnailCall"));
		groundSpecials.Add(new Special(new List<char[]>() { new char[] { '4', 'p' }, new char[] { '2', 'r' }, new[] { 'k', 'p' } }, "BackToss"));

		airSpecials.Add(new Special(new List<char[]>() { new char[] { '4', 'p' }, new char[] { '2', 'r' }, new char[] { 'k', 'p' } }, "AirSnail"));
		airSpecials.Add(new Special(new List<char[]>() { new char[] { '4', 'p' }, new char[] { '2', 'r' }, new char[] { '8', 'p' }, new char[] { 'k', 'p' } }, "AirSnail"));

		easyCommandSpecials.Add(new CommandNormal(new List<char>() { '6', '4' }, 'a', "SnailCallJump"));
		easyCommandSpecials.Add(new CommandNormal(new List<char>() { '2', '2' }, 'a', "SnailCallFake", true));
		easyCommandSpecials.Add(new CommandNormal(new List<char>() { '4', '6' }, 'a', "BackToss"));
		easySpecial = "SnailCall";

		easyAirSpecial = "AirSnail";

		easySuper = "SnailStrike";
	}
	public override void _Ready()
	{
		GD.Print("Calling SL Ready");
		base._Ready();
		charName = "SL";

		

	}

	public override void Reset()
	{
		base.Reset();
		leftCornerSnail = false;
		rightCornerSnail = false;
	}

	public void SnailRide()
	{
		if (CheckHeldKey('8'))
			ChangeState("SnailAirSnipe");
		else
			ChangeState("SnailRide");
	}

	protected override Dictionary<string, int> GetStateCharSpecific()
	{
		var dict = new Dictionary<string, int>();
		dict["leftCornerSnail"] = Convert.ToInt32(leftCornerSnail);
		dict["rightCornerSnail"] = Convert.ToInt32(rightCornerSnail);
		return dict;
	}

	protected override void SetStateCharSpecific(Dictionary<string, int> dict)
	{
		if (dict != null)
		{
			leftCornerSnail = dict["leftCornerSnail"] == 1;
			rightCornerSnail = dict["rightCornerSnail"] == 1;
		}

	}
}
