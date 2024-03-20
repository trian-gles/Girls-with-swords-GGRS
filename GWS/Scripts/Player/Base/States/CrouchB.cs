using Godot;
using System;
using System.Collections.Generic;

public class CrouchB : GroundAttack
{
	public override void _Ready()
	{
		base._Ready();
		AddSpecials(owner.groundSpecials);
		AddExSpecials(owner.groundExSpecials);
		AddGatling(new char[] { 's', 'p' }, () => owner.CheckHeldKey('2'), "CrouchC");
		AddGatling(new char[] { 's', 'p' }, "Slash");
		AddGatling(new char[] { 'b', 'p' }, "Slash");
		AddKara(new char[] { 's', 'p' }, () => owner.CanGrab(), "GrabStart");
	}
}
