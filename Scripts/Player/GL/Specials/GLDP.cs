using Godot;
using System;

public class GLDP : LaunchAttack
{
	public override void _Ready()
	{
		base._Ready();
		AddJumpCancel();
	}
}
