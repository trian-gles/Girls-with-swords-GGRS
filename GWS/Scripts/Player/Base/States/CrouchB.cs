using Godot;
using System;
using System.Collections.Generic;

public class CrouchB : GroundAttack
{
	public override void _Ready()
	{
		base._Ready();
		AddSpecials(owner.groundSpecials);
		AddGatling(new char[] { 's', 'p' }, () => owner.CheckHeldKey('2'), "CrouchC");
		AddGatling(new char[] { 's', 'p' }, "Slash");
		AddKara(new char[] { 's', 'p' }, () => owner.CanGrab(), "GrabStart");
	}
}
