using Godot;
using System;
using System.Collections.Generic;

public class OL : Player
{
	public override void _EnterTree()
	{
		base._EnterTree();
		groundSpecials.Add(new Special(new List<char[]>() { new char[] { '6', 'p' }, new char[] { '2', 'p' }, new char[] { '6', 'p' }, new char[] { 'p', 'p' } }, "DP"));
		groundSpecials.Add(new Special(new List<char[]>() { new char[] { '2', 'p' }, new char[] { '6', 'p' }, new[] { 'p', 'p' } }, "Hadouken"));
		groundSpecials.Add(new Special(new List<char[]>() { new char[] { '6', 'p' }, new char[] { '2', 'p' }, new char[] { '6', 'r' }, new char[] { '4', 'p' }, new char[] { '2', 'r' }, new[] { 'k', 'p' } }, "CommandRun"));
		groundSpecials.Add(new Special(new List<char[]>() { new char[] { '2', 'p' }, new char[] { '2', 'p' }, new char[] { 's', 'p' } }, "AntiAir"));
	}
	public override void _Ready()
	{
		GD.Print("Calling OL Ready");
		base._Ready();
		charName = "OL";

		
		//AddAltState("CrouchB");
		//AddAltState("Walk");
		//AddAltState("Slash");
		//AddAltState("Crouch");
		//AddAltState("Idle");
	}
}
