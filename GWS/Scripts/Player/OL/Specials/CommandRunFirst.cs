using Godot;
using System;
using System.Collections.Generic;

public class CommandRunFirst : CommandRunBase
{
	[Export]
	public int checkTurnFrame = 9;


	public override void FrameAdvance()
	{
		base.FrameAdvance();
		if (frameCount == checkTurnFrame && CheckNotFacingDirectionHeld()) // needs to check where opponent is
		{
			EmitSignal(nameof(StateFinished), "CommandRunTurn");
		}
	}

	private bool CheckNotFacingDirectionHeld()
    {
		return ((owner.facingRight && owner.CheckHeldKey('4')) || !owner.facingRight && owner.CheckHeldKey('6'));
    }
}
