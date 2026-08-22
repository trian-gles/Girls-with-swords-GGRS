using Godot;
using System;
using System.Collections.Generic;

public class SL : Player
{
	public bool leftCornerSnail = false;
	public bool rightCornerSnail = false;
	public bool leftCornerSnailArrived = false;
	public bool rightCornerSnailArrived = false;

	private const string SnailAirSnipeString = "SnailAirSnipe";
	private const string SnailRideString = "SnailRide";
		private const string SnailCommandString = "Snail";
	public override void _EnterTree()
	{
		base._EnterTree();

		commandNormals.Add(new CommandNormal(new List<char>() { '6', '4' }, 's', "6C"));
		commandNormals.Add(new CommandNormal(new List<char>() { '6', '4' }, 'k', "6K"));
		commandNormals.Add(new CommandNormal(new List<char>() { '6', '4' }, 'p', "6P"));

		airCommandNormals.Add(new CommandNormal(new List<char>() { '2', '2' }, 's', "J2C"));
		cooldownSpecials.Add("SnailCallFake");
		cooldownSpecials.Add("PhoneTossLow");

		dashSpecials.Add(new Special(new InputContainer( new[] { new char[] { 's', 'p' } }), "DashAttack"));
		groundSpecials.Add(new Special(Globals.GetQCB('p'), "BackToss", true));
		AddSpecials("SnailCallFake", "PhoneTossLow", "SnailCall", "SnailCallJump", "SnailCallJump", "SnailStrike");
		AddEasySpecials("SnailCallFake", "SnailCall", "BackToss", "SnailCallJump", "AirSnail", "SnailStrike");
		airSpecials.Add(new Special(Globals.GetQCF('s'), "SnailCall"));
		airSpecials.Add(new Special(Globals.GetQCB('p'), "AirSnail", true));

		characterMoves.Add(new Godot.Collections.Array<string>() { "qcf", "p", "", "", "It's\nFor You" });
		characterMoves.Add(new Godot.Collections.Array<string>() { "qcb", "p", "", "", "What's\nThis?" });
		characterMoves.Add(new Godot.Collections.Array<string>() { "qcf", "k", "", "", "It's\nFor You" });
		characterMoves.Add(new Godot.Collections.Array<string>() { "qcf", "s", "", "", "Snail\nCall" });
		characterMoves.Add(new Godot.Collections.Array<string>() { "dp", "s", "", "", "Snail\nCall" });
		characterMoves.Add(new Godot.Collections.Array<string>() { "air", "qcb", "p", "", "Air\nToss" });
		characterMoves.Add(new Godot.Collections.Array<string>() { "air", "qcf", "s", "",  "Air\nCall" });
		characterMoves.Add(new Godot.Collections.Array<string>() { "air", "dp", "s", "", "", "Air\nCall" });
		characterMoves.Add(new Godot.Collections.Array<string>() { "right", "s", "hold", "", "Come\nBack" });
		characterMoves.Add(new Godot.Collections.Array<string>() { "qcf", "qcf", "s", "", "Snail\nStrike" });
	}
	public override void _Ready()
	{
		base._Ready();
		charName = "SL";

		

	}

	public override void Reset()
	{
		base.Reset();
		leftCornerSnail = false;
		rightCornerSnail = false;
		leftCornerSnailArrived = false;
		rightCornerSnailArrived = false;
	}

	public void SnailRide()
	{
		if (!otherPlayer.grounded)
			ChangeState(SnailAirSnipeString);
		else
			ChangeState(SnailRideString);
	}
	
	private const int LEFTCORNERSNAILINDEX = 0;
	private const int RIGHTCORNERSNAILINDEX = 1;
	private const int LEFTCORNERSNAILARRIVEDINDEX = 2;
	private const int RIGHTCORNERSNAILARRIVEDINDEX = 3;
	protected override int[] GetStateCharSpecific()
	{
		charSpecificData[LEFTCORNERSNAILINDEX] = leftCornerSnail ? 1 : 0;
		charSpecificData[RIGHTCORNERSNAILINDEX] = rightCornerSnail ? 1 : 0;
		charSpecificData[LEFTCORNERSNAILARRIVEDINDEX] = leftCornerSnailArrived ? 1 : 0;
		charSpecificData[RIGHTCORNERSNAILARRIVEDINDEX] = rightCornerSnailArrived ? 1 : 0;
		return charSpecificData;
	}

	protected override void SetStateCharSpecific(int[] newCharSpecificData)
	{
		if (newCharSpecificData != null)
		{
			leftCornerSnail = Convert.ToBoolean(newCharSpecificData[LEFTCORNERSNAILINDEX]);
			rightCornerSnail = Convert.ToBoolean(newCharSpecificData[RIGHTCORNERSNAILINDEX]);
			leftCornerSnailArrived = Convert.ToBoolean(newCharSpecificData[LEFTCORNERSNAILARRIVEDINDEX]);
			rightCornerSnailArrived = Convert.ToBoolean(newCharSpecificData[RIGHTCORNERSNAILARRIVEDINDEX]);
			
		}	
	}

	protected override void PostHitCall()
	{
		base.PostHitCall();
		if (currentState.tags.Contains(Globals.Tags.hitstate))
		{
			CommandHadouken(SnailCommandString, HadoukenPart.ProjectileCommand.Kill);
			CommandHadouken(SnailCommandString, HadoukenPart.ProjectileCommand.Kill);
			CommandHadouken(SnailCommandString, HadoukenPart.ProjectileCommand.Kill);
		}
	}
}
