using Godot;
using System;
using System.Collections.Generic;

public class HitState : State
{
	public override HashSet<string> tags { get; set; } = new HashSet<string>() { "hitstate" };

	public override bool wasHit
	{ get { return true; } }

	public override bool DelayInputs()
	{
		return frameCount > 0;
	}

    public override void FrameAdvance()
    {
        base.FrameAdvance();
		if (frameCount == 1)
			owner.EmitSignal("Recovery", owner.Name);
	}

    public override void Exit()
    {
        base.Exit();
		owner.grabInvulnFrames = 5;
    }
}
