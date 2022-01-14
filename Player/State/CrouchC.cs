using Godot;
using System;

public class CrouchC : BaseAttack
{
	public override void _Ready()
	{
		base._Ready();
		AddJumpCancel();
	}
}
