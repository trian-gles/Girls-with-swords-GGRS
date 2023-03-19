using Godot;
using System;

public class Jab : GroundAttack
{
	
	public override void _Ready()
	{
		base._Ready();
		AddSpecials(owner.groundSpecials);
		AddExSpecials(owner.groundExSpecials);
		AddGatling(new char[] { 'k', 'p' }, () => owner.CheckHeldKey('2'), "CrouchB");
		AddGatling(new char[] { 's', 'p' }, () => owner.CheckHeldKey('2'), "CrouchC");
		AddGatling(new char[] { 'p', 'p' }, "Jab");
		AddGatling(new char[] { 'k', 'p' }, "Kick");
		AddGatling(new char[] { 's', 'p' }, "Slash");
	}
}
