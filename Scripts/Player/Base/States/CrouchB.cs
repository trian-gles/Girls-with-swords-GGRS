using Godot;
using System;
using System.Collections.Generic;

public class CrouchB : GroundAttack
{
	public override void _Ready()
	{
		base._Ready();
		AddJumpCancel();
		AddGatling(new char[] { '8', 'p' }, "Jump");
		AddGatling(new char[] { 's', 'p' }, () => owner.CheckHeldKey('2'), "CrouchC");
		AddGatling(new char[] { 's', 'p' }, "Slash");
	}
}
