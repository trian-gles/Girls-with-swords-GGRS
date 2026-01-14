using Godot;
using System;
using System.Collections.Generic;

public class SL : Player
{
	public bool leftCornerSnail = false;
	public bool rightCornerSnail = false;
	public bool leftCornerSnailArrived = false;
	public bool rightCornerSnailArrived = false;
	public override void _EnterTree()
	{
		base._EnterTree();
		//groundExSpecials.Add(new Special(new List<char[]>() { new char[] { '6', 'p' }, new char[] { '2', 'r' }, new char[] { '2', 'p' }, new char[] { '6', 'p' }, new char[] { 's', 'p' } }, "SnailStrike"));
		//groundExSpecials.Add(new Special(new List<char[]>() { new char[] { '2', 'p' }, new char[] { '6', 'p' }, new char[] { '2', 'p' }, new char[] { '2', 'r' }, new char[] { 's', 'p' } }, "SnailStrike"));
		//groundExSpecials.Add(new Special(new List<char[]>() { new char[] { '6', 'p' }, new char[] { '2', 'r' }, new char[] { '2', 'p' }, new char[] { '2', 'r' }, new char[] { 's', 'p' } }, "SnailStrike"));
		//groundExSpecials.Add(new Special(new List<char[]>() { new char[] { '2', 'p' }, new char[] { '6', 'p' }, new char[] { '2', 'p' }, new char[] { '6', 'p' }, new char[] { 's', 'p' } }, "SnailStrike"));

		commandNormals.Add(new CommandNormal(new List<char>() { '6', '4' }, 's', "6C"));
		commandNormals.Add(new CommandNormal(new List<char>() { '6', '4' }, 'k', "6K"));
		commandNormals.Add(new CommandNormal(new List<char>() { '6', '4' }, 'p', "6P"));

		airCommandNormals.Add(new CommandNormal(new List<char>() { '2', '2' }, 's', "J2C"));

		//groundSpecials.Add(new Special(new List<char[]>() { new char[] { '4', 'p' }, new char[] { '2', 'r' }, new[] { 's', 'p' } }, "SnailCall"));
		//groundSpecials.Add(new Special(new List<char[]>() { new char[] { '4', 'p' }, new char[] { '2', 'r' }, new[] { 'k', 'p' } }, "BackToss"));

		//airSpecials.Add(new Special(new List<char[]>() { new char[] { '4', 'p' }, new char[] { '2', 'r' }, new char[] { 'k', 'p' } }, "AirSnail"));
		//airSpecials.Add(new Special(new List<char[]>() { new char[] { '4', 'p' }, new char[] { '2', 'r' }, new char[] { '8', 'p' }, new char[] { 'k', 'p' } }, "AirSnail"));

		easyCommandSpecials.Add(new CommandNormal(new List<char>() { '6', '4' }, 'a', "SnailCallJump"));
		easyCommandSpecials.Add(new CommandNormal(new List<char>() { '2', '2' }, 'a', "SnailCallFake", true, true));
		easyCommandSpecials.Add(new CommandNormal(new List<char>() { '4', '6' }, 'a', "BackToss"));

		dashSpecials.Add(new Special(new List<char[]>() { new char[] { 's', 'p' } }, "DashAttack"));

		//dashSpecials.Add(new Special(new List<char[]>() { new char[] { 's', 'p' } }, "SnailRideAttempt"));
		easySpecial = "SnailCall";

		easyAirSpecial = "AirSnail";

		easySuper = "SnailStrike";
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
			ChangeState("SnailAirSnipe");
		else
			ChangeState("SnailRide");
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
		if (currentState.tags.Contains("hurtstate"))
		{
			CommandHadouken("Snail", HadoukenPart.ProjectileCommand.Kill);
			CommandHadouken("Snail", HadoukenPart.ProjectileCommand.Kill);
			CommandHadouken("Snail", HadoukenPart.ProjectileCommand.Kill);
		}
	}
}
