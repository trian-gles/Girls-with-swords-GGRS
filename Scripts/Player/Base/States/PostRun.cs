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

