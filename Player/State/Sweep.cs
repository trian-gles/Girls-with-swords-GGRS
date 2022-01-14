using Godot;
using System;

public class Sweep : BaseAttack
{
	public override void _Ready()
	{
		base._Ready();
		AddGatling(new char[] { '8', 'p' }, "Jump");
		AddGatling(new char[] { 's', 'p' }, () => owner.CheckHeldKey('2'), "CrouchSlash");
		AddGatling(new char[] { 's', 'p' }, "Slash");
		
		
	}
}
