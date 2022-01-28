using Godot;
using System;
using System.Collections.Generic;

public class PreRun : State
{
	public override void _Ready()
	{
		base._Ready();
		loop = true;
		AddGatling(new[] { '6', 'r' }, "Idle");
		AddGatling(new[] { '4', 'r' }, "Idle");
		AddGatling(new[] { '8', 'p' }, "MovingJump");
	}


	public override void FrameAdvance()
	{
		frameCount++;
		if (frameCount  == 5)
		{
			EmitSignal(nameof(StateFinished), "Run");
		}
	}

}

