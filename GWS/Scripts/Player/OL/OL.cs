using Godot;
using System;
using System.Collections.Generic;

public class OL : Player
{
	public override void _EnterTree()
	{
		base._EnterTree();
		// Super
		groundExSpecials.Add(new Special(new List<char[]>() { new char[] { '6', 'p' }, new char[] { '2', 'r' }, new char[] { '2', 'p' }, new char[] { '6', 'p' }, new char[] { 's', 'p' } }, "Super"));
		groundExSpecials.Add(new Special(new List<char[]>() { new char[] { '2', 'p' }, new char[] { '6', 'p' }, new char[] { '2', 'p' },  new char[] { '2', 'r' }, new char[] { 's', 'p' } }, "Super"));
		groundExSpecials.Add(new Special(new List<char[]>() { new char[] { '6', 'p' }, new char[] { '2', 'r' }, new char[] { '2', 'p' }, new char[] { '2', 'r' }, new char[] { 's', 'p' } }, "Super"));
		groundExSpecials.Add(new Special(new List<char[]>() { new char[] { '2', 'p' }, new char[] { '6', 'p' }, new char[] { '2', 'p' }, new char[] { '6', 'p' }, new char[] { 's', 'p' } }, "Super"));


		//j2C
		airCommandNormals.Add(new CommandNormal(new List<char>() { '6', '4' }, 's', "InstantOverhead"));

		//6k
		commandNormals.Add(new CommandNormal(new List<char>() { '6', '4' }, 'k', "6K"));


		//DP
		groundSpecials.Add(new Special(new List<char[]>() { new char[] { '6', 'r' }, new char[] { '2', 'p' }, new char[] { '6', 'p' }, new char[] { 's', 'p' } }, "AntiAir"));
		groundSpecials.Add(new Special(new List<char[]>() { new char[] { '2', 'p' }, new char[] { '6', 'r' }, new char[] { '6', 'p' }, new char[] { 's', 'p' } }, "AntiAir"));
		//air DP
		airSpecials.Add(new Special(new List<char[]>() { new char[] { '6', 'r' }, new char[] { '2', 'p' }, new char[] { '6', 'p' }, new char[] { 's', 'p' } }, "AntiAir"));
		airSpecials.Add(new Special(new List<char[]>() { new char[] { '2', 'p' }, new char[] { '6', 'r' }, new char[] { '6', 'p' }, new char[] { 's', 'p' } }, "AntiAir"));
		
		groundSpecials.Add(new Special(new List<char[]>() { new char[] { '6', 'p' }, new char[] { '2', 'r' }, new[] { 'p', 'p' } }, "Hadouken"));
		groundSpecials.Add(new Special(new List<char[]>() { new char[] { '2', 'p' }, new char[] { '6', 'r' }, new char[] { '4', 'p' }, new char[] { '2', 'r' }, new[] { 'k', 'p' } }, "CommandRun"));
		groundSpecials.Add(new Special(new List<char[]>() { new char[] { '2', 'p' }, new char[] { '6', 'r' }, new char[] { '4', 'p' }, new char[] { '2', 'r' }, new[] { 's', 'p' } }, "HojogiriCharge"));
		//groundSpecials.Add(new Special(new List<char[]>() { new char[] { '2', 'p' }, new char[] { '2', 'p' }, new char[] { 's', 'p' } }, "AntiAir"));

		//airSpecials.Add(new Special(new List<char[]>() { new char[] { '2', 'p' }, new char[] { '2', 'p' }, new char[] { 's', 'p' } }, "AntiAir"));


		rhythmSpecials.Add(new Special(new List<char[]>() { new char[] { '6', 'p' }, new[] { 'r', 'p' } }, "Hadouken"));
		rhythmSpecials.Add(new Special(new List<char[]>() { new char[] { '8', 'p' }, new char[] { 'r', 'p' } }, "DP"));
		rhythmSpecials.Add(new Special(new List<char[]>() { new char[] { '4', 'p' }, new char[] { 'r', 'p' } }, "CommandRun"));
		rhythmSpecials.Add(new Special(new List<char[]>() { new char[] { '2', 'p' }, new char[] { 'r', 'p' } }, "AntiAir"));

		rhythmSpecials.Add(new Special(new List<char[]>() { new char[] { 'r', 'p' }, new[] { '6', 'p' } }, "Hadouken"));
		rhythmSpecials.Add(new Special(new List<char[]>() { new char[] { 'r', 'p' }, new char[] { '8', 'p' } }, "DP"));
		rhythmSpecials.Add(new Special(new List<char[]>() { new char[] { 'r', 'p' }, new char[] { '4', 'p' } }, "CommandRun"));
		rhythmSpecials.Add(new Special(new List<char[]>() { new char[] { 'r', 'p' }, new char[] { '2', 'p' } }, "AntiAir"));

		dashSpecials.Add(new Special(new List<char[]>() { new char[] { 's', 'p' } }, "InstantOverhead"));
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
