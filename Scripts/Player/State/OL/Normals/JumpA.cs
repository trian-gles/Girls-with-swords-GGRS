using Godot;
using System;

public class JumpA : JumpB
{
	public override void _Ready()
	{
		base._Ready();
		AddGatling(new char[] { 'k', 'p' }, "JumpB");
		AddGatling(new char[] { 's', 'p' }, "JumpC");
		
	}
}
