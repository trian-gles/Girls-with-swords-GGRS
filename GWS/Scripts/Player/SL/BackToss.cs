using Godot;
using System;

public class BackToss : Hadouken
{
	[Export]
	public int earlyReleaseFrame = 14;
	public override void FrameAdvance()
	{
		base.FrameAdvance();
		var sl = (SL)owner;
		if (sl.leftCornerSnail && sl.rightCornerSnail && frameCount > earlyReleaseFrame)
			EmitSignal(nameof(StateFinished), "Idle");

	}
}
