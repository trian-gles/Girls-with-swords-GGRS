using Godot;
using System;

public class HitState : State
{
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
