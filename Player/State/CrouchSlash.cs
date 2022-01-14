using Godot;
using System;

public class CrouchSlash : BaseAttack
{
	public override void _Ready()
	{
		base._Ready();
		AddJumpCancel();
	}
}
