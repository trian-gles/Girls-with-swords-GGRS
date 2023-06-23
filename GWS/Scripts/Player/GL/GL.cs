using Godot;
using System;
using System.Collections.Generic;

public class GL : Player
{
	public override void _EnterTree()
	{
		//j2C
		airCommandNormals.Add(new CommandNormal(new List<char>() { '2', '2' }, 's', "J2C"));

		//6k
		commandNormals.Add(new CommandNormal(new List<char>() { '6', '4' }, 'k', "6K"));

		//6c
		commandNormals.Add(new CommandNormal(new List<char>() { '6', '4' }, 's', "6C"));

		//DP
		groundExSpecials.Add(new Special(new List<char[]>() { new char[] { '6', 'r' }, new char[] { '2', 'p' }, new char[] { '6', 'p' }, new char[] { 'p', 'p' } }, "GLDP"));
		groundExSpecials.Add(new Special(new List<char[]>() { new char[] { '2', 'p' }, new char[] { '6', 'r' }, new char[] { '6', 'p' }, new char[] { 'p', 'p' } }, "GLDP"));
		//allow forward as last input
		groundSpecials.Add(new Special(new List<char[]>() { new char[] { '6', 'r' }, new char[] { '2', 'p' }, new char[] { '6', 'p' }, new char[] { '2', 'r' }, new char[] { 'p', 'p' } }, "GLDP"));
		groundSpecials.Add(new Special(new List<char[]>() { new char[] { '2', 'p' }, new char[] { '6', 'r' }, new char[] { '6', 'p' }, new char[] { '2', 'r' }, new char[] { 'p', 'p' } }, "GLDP"));
		//Air DP
		airSpecials.Add(new Special(new List<char[]>() { new char[] { '6', 'r' }, new char[] { '2', 'p' }, new char[] { '6', 'p' }, new char[] { 'p', 'p' } }, "GLDP"));
		airSpecials.Add(new Special(new List<char[]>() { new char[] { '2', 'p' }, new char[] { '6', 'r' }, new char[] { '6', 'p' }, new char[] { 'p', 'p' } }, "GLDP"));
		//Black hole
		airSpecials.Add(new Special(new List<char[]>() { new char[] { '4', 'p' }, new char[] { '2', 'r' }, new char[] { 's', 'p' }}, "BlackHolePlace"));
		airSpecials.Add(new Special(new List<char[]>() { new char[] { '4', 'p' }, new char[] { '2', 'r' }, new char[] { '8', 'p' }, new char[] { 's', 'p' } }, "BlackHolePlace")); // allow TK


		//allow forward as last input for air DP
		airExSpecials.Add(new Special(new List<char[]>() { new char[] { '6', 'r' }, new char[] { '2', 'p' }, new char[] { '6', 'p' }, new char[] { '2', 'r' }, new char[] { 'p', 'p' } }, "GLDP"));
		airExSpecials.Add(new Special(new List<char[]>() { new char[] { '2', 'p' }, new char[] { '6', 'r' }, new char[] { '6', 'p' }, new char[] { '2', 'r' }, new char[] { 'p', 'p' } }, "GLDP"));
		
		
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
}
