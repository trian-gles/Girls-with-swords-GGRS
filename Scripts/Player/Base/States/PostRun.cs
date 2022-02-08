using Godot;
using System;
using System.Collections.Generic;

public class PostRun : State
{
	public override void _Ready()
	{
		base._Ready();
		loop = true;
		AddGatling(new[] { 'p', 'p' }, "Jab");
		AddGatling(new[] { 'k', 'p' }, "Kick");
		AddGatling(new[] { 's', 'p' }, "Slash");
		AddGatling(new List<char[]>() { new char[] { '2', 'p' }, new char[] { '6', 'p' }, new[] { 'p', 'p' } }, "Hadouken");
		
		AddGatling(new List<char[]>() { new char[] { '6', 'p' }, new char[] { '2', 'p' }, new char[] { '6', 'r' }, new char[] { '4', 'p' }, new char[] { '2', 'r' }, new[] { 'k', 'p' } }, "CommandRun");
	}

	public override void Enter()
	{
		base.Enter();
		if (owner.velocity.x < 0) { owner.velocity.x = -owner.speed; }
		else { owner.velocity.x = owner.speed; }
	}
	public override void FrameAdvance()
	{
		frameCount++;
		if (frameCount  == 12)
		{
			EmitSignal(nameof(StateFinished), "Idle");
		}
	}

}

