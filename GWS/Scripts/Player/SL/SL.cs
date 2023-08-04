using Godot;
using System;
using System.Collections.Generic;

public class SL : Player
{
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

	public void SnailRide()
	{
		ChangeState("SnailRide");
	}
}
