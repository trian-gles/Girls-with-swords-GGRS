using Godot;
using System;
using System.Collections.Generic;

public class PreJump : State
{
	[Export]
	public int len = 2;
	public override void _Ready()
	{
		base._Ready();
		animationName = "None";
		stop = false;
	}
	public override bool DelayInputs()
	{
		return true;
	}

	public override void FrameAdvance()
	{
		base.FrameAdvance();
		if (frameCount == len)
			EmitSignal(nameof(StateFinished), "Jump");
	}
}


