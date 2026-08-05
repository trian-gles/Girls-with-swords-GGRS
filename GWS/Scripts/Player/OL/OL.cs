using Godot;
using System;
using System.Collections.Generic;

public class OL : Player
{
	private const string HadoukenString = "Hadouken";
	public override void _EnterTree()
	{
		base._EnterTree();

		//j2C
		airCommandNormals.Add(new CommandNormal(new List<char>() { '2', '2' }, 's', "InstantOverhead"));

		//easyCommandSpecials.Add(new CommandNormal(new List<char>() { '4', '6' }, 'a', "Hojogiri"));

		//6p
		commandNormals.Add(new CommandNormal(new List<char>() { '6', '4' }, 'p', "6P"));
		commandNormals.Add(new CommandNormal(new List<char>() { '6', '4' }, 'k', "6K"));
		commandNormals.Add(new CommandNormal(new List<char>() { '6', '4' }, 's', "6S"));
		groundSpecials.Add(new Special(Globals.GetQCB('p'), "FastHadouken", true));
		airSpecials.Add(new Special(Globals.GetQCF('p'), "AirHadouken", true));
		cooldownSpecials.Add("Hadouken");
		cooldownSpecials.Add("AirHadouken");
		AddSpecials("Hadouken", "HojogiriCharge", "CommandRunWillTurn", "AntiAir", "AntiAir", "Super");
		AddEasySpecials("Hadouken", "HojogiriCharge", "CommandRunWillTurn", "AntiAir", "AntiAir", "Super");
		dashSpecials.Add(new Special(new InputContainer(new[]{ new char[] { 's', 'p' } }), "InstantOverhead"));
	}
	public override void _Ready()
	{
		base._Ready();
		charName = "OL";
	}

	protected override void PostHitCall()
	{
		base.PostHitCall();
		if (currentState.tags.Contains(Globals.Tags.hitstate))
		{
			CommandHadouken(HadoukenString, HadoukenPart.ProjectileCommand.Kill);
		}
	}
}
