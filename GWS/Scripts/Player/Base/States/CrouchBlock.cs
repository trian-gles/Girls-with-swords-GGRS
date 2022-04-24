using Godot;
using System;

public class CrouchBlock : Block
{
	public override void FrameAdvance()
	{
		frameCount++;
		if (slowdownSpeed != 0) SlowDown();
		stunRemaining--;
		if (stunRemaining == 0)
		{
			if (owner.CheckHeldKey('2'))
				EmitSignal(nameof(StateFinished), "Crouch");
			else
				EmitSignal(nameof(StateFinished), "Idle");

		}
	}
}
