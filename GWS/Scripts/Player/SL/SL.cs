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
	}
	public override void _Ready()
	{
		GD.Print("Calling SL Ready");
		base._Ready();
		charName = "SL";

		

	}
}
