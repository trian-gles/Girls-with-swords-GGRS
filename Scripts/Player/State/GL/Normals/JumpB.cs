using Godot;
using System;

public class GLJumpB : GLJumpC
{
	public override void _Ready()
	{
		base._Ready();
		AddGatling(new char[] { 's', 'p' }, "JumpC");
	}
}

