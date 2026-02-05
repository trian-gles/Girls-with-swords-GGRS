using Godot;
using System;
using System.Collections.Generic;

public class CommandRunWillTurn : CommandRunBase
{
	[Export]
	public int checkTurnFrame = 9;

	private const string CommandRunTurnString = "CommandRunTurn";


	public override void FrameAdvance()
	{
		base.FrameAdvance();
		if (frameCount == checkTurnFrame)
		{
			owner.ChangeState(CommandRunTurnString);
		}
	}
}
