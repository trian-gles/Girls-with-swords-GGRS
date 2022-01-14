using Godot;
using System;
using System.Collections.Generic;

public class Slash : GroundAttack
{
	public override void _Ready()
	{
		base._Ready();
		AddJumpCancel();
		AddGatling(new List<char[]>() { new char[] { '6', 'p' }, new char[] { '2', 'p' }, new char[] { '6', 'p' }, new char[] { 'p', 'p' } }, "DP");
		AddGatling(new List<char[]>() { new char[] { '6', 'p' }, new char[] { '2', 'p' }, new char[] { '6', 'r' }, new char[] { '4', 'p' }, new char[] { '2', 'r' }, new[] {'k', 'p' } }, "CommandRun");
	}

}
