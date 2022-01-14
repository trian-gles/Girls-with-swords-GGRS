using Godot;
using System;
using System.Collections.Generic;

public class Kick : Slash
{
	public override void _Ready()
	{
		base._Ready();
		AddGatling(new char[] { 's', 'p' }, "Slash");
		AddGatling(new List<char[]> { new char[] { '2', 'p' }, new char[] { 'k', 'p' } }, "Sweep");
		AddGatling(new List<char[]> { new char[] { '2', 'p' }, new char[] { 's', 'p' } }, "CrouchSlash");
	}
}

