using Godot;
using System;

public class Fall : State
{
	public override void _Ready()
	{
		base._Ready();
		loop = true;
		AddGatling(new[] { 'p', 'p' }, "JumpA");
		AddGatling(new[] { 'k', 'p' }, "JumpB");
		AddGatling(new[] { 's', 'p' }, "JumpC");
		AddGatling(new[] { '8', 'p' }, () => owner.canDoubleJump, "DoubleJump", () => { owner.canDoubleJump = false; });
	}
	public override void FrameAdvance()
	{
		base.FrameAdvance();
		if (owner.grounded && frameCount > 0)
		{
			owner.ForceEvent(EventScheduler.EventType.AUDIO, "Landing");
			EmitSignal(nameof(StateFinished), "Idle");
		}
		//owner.CheckTurnAround();
		ApplyGravity();
	}

	public override void PushMovement(float _xVel)
	{
	}
}
