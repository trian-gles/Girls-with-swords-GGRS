using Godot;
using System;
using System.Collections.Generic;

public class CommandRunWillTurn : CommandRunBase
{
	[Export]
	public int checkTurnFrame = 9;


	public override void FrameAdvance()
	{
		base.FrameAdvance();
		if (frameCount == checkTurnFrame)
		{
			EmitSignal(nameof(StateFinished), "CommandRunTurn");
		}
	}
}
