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
		groundSpecials.Add(new Special(new List<char[]>() { new char[] { '4', 'p' }, new char[] { '2', 'r' }, new[] { 's', 'p' } }, "SnailCall"));
		groundSpecials.Add(new Special(new List<char[]>() { new char[] { '4', 'p' }, new char[] { '2', 'r' }, new[] { 'k', 'p' } }, "BackToss"));

		airSpecials.Add(new Special(new List<char[]>() { new char[] { '4', 'p' }, new char[] { '2', 'r' }, new char[] { 'k', 'p' } }, "AirSnail"));
		airSpecials.Add(new Special(new List<char[]>() { new char[] { '4', 'p' }, new char[] { '2', 'r' }, new char[] { '8', 'p' }, new char[] { 'k', 'p' } }, "AirSnail"));
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
