using Godot;
using System;

public class Gunblazed : GroundAttack
{
	public override void _Ready()
	{
		base._Ready();
		AddJumpCancel();
	}
}
