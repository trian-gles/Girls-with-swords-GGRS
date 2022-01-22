using Godot;
using System;
using System.Collections.Generic;

public class OLKick : OLSlash
{
	public override void _Ready()
	{
		base._Ready();
		AddGatling(new char[] { 's', 'p' }, "Slash");
		AddGatling(new List<char[]> { new char[] { '2', 'p' }, new char[] { 'k', 'p' } }, "CrouchB");
		AddGatling(new List<char[]> { new char[] { '2', 'p' }, new char[] { 's', 'p' } }, "CrouchC");
	}
}

