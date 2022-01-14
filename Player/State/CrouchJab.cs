using Godot;
using System;
using System.Collections.Generic;

public class CrouchJab : BaseAttack
{
	public override void _Ready()
	{
		base._Ready();
		AddGatling(new char[] { '8', 'p' }, "Jump");
		AddGatling(new char[] { 'p', 'p' }, () => owner.CheckHeldKey('2'), "CrouchJab");
		AddGatling(new char[] { 'k', 'p' }, () => owner.CheckHeldKey('2'), "Sweep");
		AddGatling(new char[] { 's', 'p' }, () => owner.CheckHeldKey('2'), "CrouchSlash");
		AddGatling(new char[] { 'p', 'p' }, "Jab");
		AddGatling(new char[] { 'k', 'p' }, "Kick");
		AddGatling(new char[] { 's', 'p' }, "Slash");
		
		
	}
}

