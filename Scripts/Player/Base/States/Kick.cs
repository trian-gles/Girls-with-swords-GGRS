using Godot;
using System;
using System.Collections.Generic;

public class Kick : Slash
{
	public override void _Ready()
	{
		base._Ready();
		AddGatling(new char[] { 's', 'p' }, "Slash");
		AddGatling(new char[] { 'k', 'p' }, () => owner.CheckHeldKey('2'), "CrouchB");
		AddGatling(new char[] { 'k', 'p' }, () => owner.CheckHeldKey('2'), "CrouchC");
	}
}

