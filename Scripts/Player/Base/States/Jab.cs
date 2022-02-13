using Godot;
using System;

public class Jab : BaseAttack
{
	
	public override void _Ready()
	{
		base._Ready();
		AddGatling(new char[] { 'p', 'p' }, "Jab");
		AddGatling(new char[] { 'k', 'p' }, "Kick");
		AddGatling(new char[] { 's', 'p' }, "Slash");
		AddGatling(new char[] { 'k', 'p' }, () => owner.CheckHeldKey('2'), "CrouchB");
		AddGatling(new char[] { 'k', 'p' }, () => owner.CheckHeldKey('2'), "CrouchC");
	}

	
}
