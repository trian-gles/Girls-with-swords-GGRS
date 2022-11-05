using Godot;
using System;
using System.Collections.Generic;

public class Kick : Slash
{
	public override void _Ready()
	{
		base._Ready();
		AddJumpCancel();
		AddGatling(new char[] { 's', 'p' }, "Slash");
		AddGatling(new char[] { 'k', 'p' }, () => owner.CheckHeldKey('2'), "CrouchB");
		AddGatling(new char[] { 's', 'p' }, () => owner.CheckHeldKey('2'), "CrouchC");
		AddKara(new char[] { 's', 'p' }, () => owner.CanGrab(), "GrabStart");
	}
}

