using Godot;
using System;
using System.Collections.Generic;

public class PostRun : State
{
	public override void _Ready()
	{
		base._Ready();
		loop = true;
	}


	public override void FrameAdvance()
	{
		frameCount++;
		if (frameCount  == 5)
		{
			EmitSignal(nameof(StateFinished), "Idle");
		}
	}

}

