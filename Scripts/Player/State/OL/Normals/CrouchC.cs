using Godot;
using System;

public class OLCrouchC : BaseAttack
{
	public override void _Ready()
	{
		base._Ready();
		AddJumpCancel();
	}
}
