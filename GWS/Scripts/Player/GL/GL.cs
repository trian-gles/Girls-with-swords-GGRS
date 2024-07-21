using Godot;
using System;
using System.Collections.Generic;

public class GL : Player
{

	public int PoweredBlackHoleFramesRemaining = 0;
	public int BlackHolesTotal = 0;
	public override void _EnterTree()
	{
		//j2C
		airCommandNormals.Add(new CommandNormal(new List<char>() { '2', '2' }, 's', "J2C"));

		//6k
		commandNormals.Add(new CommandNormal(new List<char>() { '6', '4' }, 'k', "6K"));

		//3k which is actually a 2p hahaha
		commandNormals.Add(new CommandNormal(new List<char>() { '6', '4' }, 'p', "3K"));

		//6c
		commandNormals.Add(new CommandNormal(new List<char>() { '6', '4' }, 's', "6C"));

		
		airCommandNormals.Add(new CommandNormal(new List<char>() { '2', '2' }, 'a', "BlackHolePlace"));

		commandNormals.Add(new CommandNormal(new List<char>() { '6', '4' }, 'a', "Hadouken"));
		commandNormals.Add(new CommandNormal(new List<char>() { '2', '2' }, 'a', "GunBlazed", true));
		commandNormals.Add(new CommandNormal(new List<char>() { '4', '6' }, 'a', "HadoukenAir"));

		//DP
		groundExSpecials.Add(new Special(new List<char[]>() { new char[] { '6', 'r' }, new char[] { '2', 'p' }, new char[] { '6', 'p' }, new char[] { 'p', 'p' } }, "GLDP"));
		groundExSpecials.Add(new Special(new List<char[]>() { new char[] { '2', 'p' }, new char[] { '6', 'r' }, new char[] { '6', 'p' }, new char[] { 'p', 'p' } }, "GLDP"));
		//Air DP
		//Black hole
		airSpecials.Add(new Special(new List<char[]>() { new char[] { '4', 'p' }, new char[] { '2', 'r' }, new char[] { 's', 'p' }}, "BlackHolePlace"));
		airSpecials.Add(new Special(new List<char[]>() { new char[] { '4', 'p' }, new char[] { '2', 'r' }, new char[] { '8', 'p' }, new char[] { 's', 'p' } }, "BlackHolePlace")); // allow TK

		//Black Hole Powerup
		groundExSpecials.Add(new Special(new List<char[]>() { new char[] { '6', 'p' }, new char[] { '2', 'r' }, new char[] { '2', 'p' }, new char[] { '6', 'p' }, new char[] { 's', 'p' } }, "PowerBlackholes"));
		groundExSpecials.Add(new Special(new List<char[]>() { new char[] { '2', 'p' }, new char[] { '6', 'p' }, new char[] { '2', 'p' }, new char[] { '2', 'r' }, new char[] { 's', 'p' } }, "PowerBlackholes"));
		groundExSpecials.Add(new Special(new List<char[]>() { new char[] { '6', 'p' }, new char[] { '2', 'r' }, new char[] { '2', 'p' }, new char[] { '2', 'r' }, new char[] { 's', 'p' } }, "PowerBlackholes"));
		groundExSpecials.Add(new Special(new List<char[]>() { new char[] { '2', 'p' }, new char[] { '6', 'p' }, new char[] { '2', 'p' }, new char[] { '6', 'p' }, new char[] { 's', 'p' } }, "PowerBlackholes"));


		//allow forward as last input for air DP
		airExSpecials.Add(new Special(new List<char[]>() { new char[] { '6', 'r' }, new char[] { '2', 'p' }, new char[] { '6', 'p' }, new char[] { 'p', 'p' } }, "GLDP"));
		airExSpecials.Add(new Special(new List<char[]>() { new char[] { '2', 'p' }, new char[] { '6', 'r' }, new char[] { '6', 'p' }, new char[] { 'p', 'p' } }, "GLDP"));

		groundSpecials.Add(new Special(new List<char[]>() { new char[] { '2', 'p' }, new char[] { '6', 'r' }, new char[] { '4', 'p' }, new char[] { '2', 'r' }, new[] { 's', 'p' } }, "GunBlazed"));
		groundSpecials.Add(new Special(new List<char[]>() { new char[] { '6', 'p' }, new char[] { '2', 'r' }, new[] { 'p', 'p' } }, "Hadouken"));
		groundSpecials.Add(new Special(new List<char[]>() { new char[] { '6', 'p' }, new char[] { '2', 'r' }, new[] { 's', 'p' } }, "Feint"));

		groundSpecials.Add(new Special(new List<char[]>() { new char[] { '6', 'p' }, new char[] { '2', 'r' }, new[] { 'k', 'p' } }, "HadoukenAir"));
	}
	public override void _Ready()
	{
		base._Ready();
		charName = "GL";
	}

	public override bool CalculateHit()
	{
		if (!base.CalculateHit())
			return false;
		CommandHadouken("BlackHole", HadoukenPart.ProjectileCommand.BlackHoleDeactivate);
		return true;
	}

	protected override void CharSpecificFrameAdvance()
	{
		if (PoweredBlackHoleFramesRemaining > 0)
			PoweredBlackHoleFramesRemaining--;
	}

	protected override Dictionary<string, int> GetStateCharSpecific()
	{
		var dict = new Dictionary<string, int>();
		dict["BlackHoleFrames"] = PoweredBlackHoleFramesRemaining;
		dict["BlackHolesTotal"] = BlackHolesTotal;
		return dict;
	}

	protected override void SetStateCharSpecific(Dictionary<string, int> dict)
	{
		if (dict != null)
		{
			PoweredBlackHoleFramesRemaining = dict["BlackHoleFrames"];
			BlackHolesTotal = dict["BlackHolesTotal"];

		}
			
	}

}
