using Godot;
using System;

public class OLJumpB : JumpC
{
	public override void _Ready()
	{
		base._Ready();
		AddGatling(new char[] { 's', 'p' }, "JumpC");
	}
}

