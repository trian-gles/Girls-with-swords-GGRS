using Godot;
using System;

public class GLCrouchC : BaseAttack
{
	public override void _Ready()
	{
		base._Ready();
		AddJumpCancel();
	}
}
