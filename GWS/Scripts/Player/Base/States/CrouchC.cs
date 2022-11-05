using Godot;
using System;

public class CrouchC : GroundAttack
{
	public override void _Ready()
	{
		base._Ready();
		AddSpecials(owner.groundSpecials);
		AddKara(new char[] { 'k', 'p' }, () => owner.CanGrab(), "GrabStart");
	}
}
