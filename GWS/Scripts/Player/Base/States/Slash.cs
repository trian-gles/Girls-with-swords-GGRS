using Godot;
using System;
using System.Collections.Generic;

public class Slash : GroundAttack
{
	public override void _Ready()
	{
		base._Ready();
		AddGatling(new char[] { 's', 'p' }, () => owner.CheckHeldKey('2'), "CrouchC");
		AddGatling(new char[] { 'b', 'p' }, "CrouchC");
		AddExSpecials(owner.groundExSpecials);
		AddSpecials(owner.groundSpecials);
		AddEasyGroundSpecials();
		AddCommandNormals(owner.commandNormals);
		AddKara(new char[] { 'k', 'p' }, () => owner.CanGrab(), "GrabStart");
	}
}
