using Godot;
using System;

public class SixK : MovingAttack
{
	[Export]
	public string stringCancel = "";
	public override void _Ready()
	{
		base._Ready();
		AddSpecials(owner.groundSpecials);
		AddExSpecials(owner.groundExSpecials);
		AddKara(new char[] { 's', 'p' }, () => owner.CanGrab(), "GrabStart");
		if (stringCancel != "")
			AddGatling(new char[] { 'b', 'p' }, stringCancel);
	}
}
