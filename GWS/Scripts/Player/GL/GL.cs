using Godot;
using System;
using System.Collections.Generic;

public class GL : Player
{
	private const string BlackHoleString = "BlackHole";
	private const string HadoukenString = "Hadouken";
	public override void _EnterTree()
	{
		//j2C
		airCommandNormals.Add(new CommandNormal(new List<char>() { '2', '2' }, 's', "J2C"));

		//6k
		commandNormals.Add(new CommandNormal(new List<char>() { '6', '4' }, 'k', "6K"));

		//3k which is actually a 6p hahaha
		commandNormals.Add(new CommandNormal(new List<char>() { '6', '4' }, 'p', "3K"));

		//6c
		commandNormals.Add(new CommandNormal(new List<char>() { '6', '4' }, 's', "6C"));
		
		airSpecials.Add(new Special(Globals.GetQCF('k'), "HadoukenAirDown", true));
		AddSpecials("GunBlazed", "Hadouken", "Feint", "HadoukenAir", "BlackHolePlace", "GLDP");
		AddEasySpecials("GunBlazed", "Hadouken", "Feint", "HadoukenAir", "BlackHolePlace", "GLDP");
		groundSpecials.Add(new Special(Globals.GetQCB('p'), "MegaFist", true));

		//DP
		//groundExSpecials.Add(new Special(new InputContainer() { new char[] { '6', 'r' }, new char[] { '2', 'p' }, new char[] { '6', 'p' }, new char[] { 'p', 'p' } }, "GLDP"));
		//groundExSpecials.Add(new Special(new InputContainer() { new char[] { '2', 'p' }, new char[] { '6', 'r' }, new char[] { '6', 'p' }, new char[] { 'p', 'p' } }, "GLDP"));
		//Air DP
		//Black hole
		//airSpecials.Add(new Special(new InputContainer() { new char[] { '4', 'p' }, new char[] { '2', 'r' }, new char[] { 's', 'p' }}, "BlackHolePlace"));
		//airSpecials.Add(new Special(new InputContainer() { new char[] { '4', 'p' }, new char[] { '2', 'r' }, new char[] { '8', 'p' }, new char[] { 's', 'p' } }, "BlackHolePlace")); // allow TK

		//Black Hole Powerup
		//groundExSpecials.Add(new Special(new InputContainer() { new char[] { '6', 'p' }, new char[] { '2', 'r' }, new char[] { '2', 'p' }, new char[] { '6', 'p' }, new char[] { 's', 'p' } }, "PowerBlackholes"));
		//groundExSpecials.Add(new Special(new InputContainer() { new char[] { '2', 'p' }, new char[] { '6', 'p' }, new char[] { '2', 'p' }, new char[] { '2', 'r' }, new char[] { 's', 'p' } }, "PowerBlackholes"));
		//groundExSpecials.Add(new Special(new InputContainer() { new char[] { '6', 'p' }, new char[] { '2', 'r' }, new char[] { '2', 'p' }, new char[] { '2', 'r' }, new char[] { 's', 'p' } }, "PowerBlackholes"));
		//groundExSpecials.Add(new Special(new InputContainer() { new char[] { '2', 'p' }, new char[] { '6', 'p' }, new char[] { '2', 'p' }, new char[] { '6', 'p' }, new char[] { 's', 'p' } }, "PowerBlackholes"));


		//allow forward as last input for air DP
		//airExSpecials.Add(new Special(new InputContainer() { new char[] { '6', 'r' }, new char[] { '2', 'p' }, new char[] { '6', 'p' }, new char[] { 'p', 'p' } }, "GLDP"));
		//airExSpecials.Add(new Special(new InputContainer() { new char[] { '2', 'p' }, new char[] { '6', 'r' }, new char[] { '6', 'p' }, new char[] { 'p', 'p' } }, "GLDP"));

		//groundSpecials.Add(new Special(new InputContainer() { new char[] { '2', 'p' }, new char[] { '6', 'r' }, new char[] { '4', 'p' }, new char[] { '2', 'r' }, new[] { 's', 'p' } }, "GunBlazed"));
		//groundSpecials.Add(new Special(new InputContainer() { new char[] { '6', 'p' }, new char[] { '2', 'r' }, new[] { 'p', 'p' } }, "Hadouken"));
		//groundSpecials.Add(new Special(new InputContainer() { new char[] { '6', 'p' }, new char[] { '2', 'r' }, new[] { 's', 'p' } }, "Feint"));

		//groundSpecials.Add(new Special(new InputContainer() { new char[] { '6', 'p' }, new char[] { '2', 'r' }, new[] { 'k', 'p' } }, "HadoukenAir"));

		dashSpecials.Add(new Special(new InputContainer(new[]{ new char[] { 's', 'p' } }),  "DashAttack"));
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
		CommandHadouken(BlackHoleString, HadoukenPart.ProjectileCommand.BlackHoleDeactivate);
		return true;
	}

	protected override void PostHitCall()
	{
		base.PostHitCall();
		if (currentState.tags.Contains(Globals.Tags.hitstate))
		{
			CommandHadouken(HadoukenString, HadoukenPart.ProjectileCommand.Kill);
			CommandHadouken(HadoukenString, HadoukenPart.ProjectileCommand.Kill);
		}
	}

}
