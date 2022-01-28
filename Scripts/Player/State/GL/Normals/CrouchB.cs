using Godot;
using System;
using System.Collections.Generic;

public class GLCrouchB : BaseAttack
{
	public override void _Ready()
	{
		base._Ready();
		AddGatling(new char[] { '8', 'p' }, "Jump");
		AddGatling(new char[] { 's', 'p' }, () => owner.CheckHeldKey('2'), "CrouchC");
		AddGatling(new char[] { 's', 'p' }, "Slash");
		AddGatling(new List<char[]>() { new char[] { '6', 'p' }, new char[] { '2', 'p' }, new char[] { '6', 'p' }, new char[] { 'p', 'p' } }, "DP");		
		
		
		
		
	}
}
