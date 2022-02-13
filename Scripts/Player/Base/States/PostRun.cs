using Godot;
using System;
using System.Collections.Generic;

public class PostRun : MoveState
{
	public override void _Ready()
	{
		base._Ready();
		loop = true;
		slowdownSpeed = 30;
		AddGatling(new[] { 'p', 'p' }, "Jab");
		AddGatling(new[] { 'k', 'p' }, "Kick");
		AddGatling(new[] { 's', 'p' }, "Slash");
		AddGatling(new List<char[]>() { new char[] { '2', 'p' }, new char[] { '6', 'p' }, new[] { 'p', 'p' } }, "Hadouken");
		
		AddGatling(new List<char[]>() { new char[] { '6', 'p' }, new char[] { '2', 'p' }, new char[] { '6', 'r' }, new char[] { '4', 'p' }, new char[] { '2', 'r' }, new[] { 'k', 'p' } }, "CommandRun");
	}
	public override void FrameAdvance()
	{
		base.FrameAdvance();
		if (frameCount  == 12)
		{
			EmitSignal(nameof(StateFinished), "Idle");
		}
	}

}

