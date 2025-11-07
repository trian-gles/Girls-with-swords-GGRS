using Godot;
using System;
using System.Collections.Generic;

public class CrouchBlock : Block
{
    public override HashSet<string> tags { get; set; } = new HashSet<string>() { "block", "crouching" };
    public override void FrameAdvance()
	{
		frameCount++;
		if (frameCount == 1)
			owner.EmitSignal("Recovery", owner.Name);
		if (slowdownSpeed != 0) SlowDown();
		stunRemaining--;
		if (stunRemaining == 0)
		{
			if (owner.CheckHeldKey('2'))
				EmitSignal(nameof(StateFinished), "Crouch");
			else
				EmitSignal(nameof(StateFinished), "Idle");

		}

        if (owner.CheckHeldKeys(new[] { 'p', 'k' }) && owner.CheckFlippableHeldKey('6') && owner.TrySpendMeter())
        {
            EmitSignal(nameof(StateFinished), "GuardCancel");
        }
    }

    public override void EnterShieldState()
    {
        EmitSignal(nameof(StateFinished), "CrouchShield");
    }
}
