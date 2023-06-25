using Godot;
using System;
using System.Collections.Generic;

public class Slash : GroundAttack
{
	public override void _Ready()
	{
		base._Ready();
		AddGatling(new char[] { 's', 'p' }, () => owner.CheckHeldKey('2'), "CrouchC");
		AddExSpecials(owner.groundExSpecials);
		AddSpecials(owner.groundSpecials);
		AddCommandNormals(owner.commandNormals);
		AddKara(new char[] { 'k', 'p' }, () => owner.CanGrab(), "GrabStart");
	}
}
