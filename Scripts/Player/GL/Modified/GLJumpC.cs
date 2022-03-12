using Godot;
using System;

public class GLJumpC : BaseAttack
{
	public override void _Ready()
	{
		base._Ready();
		AddSpecials(owner.airSpecials);
		slowdownSpeed = 0;
		AddCancel("Fall");
	}
	public override void FrameAdvance()
	{
		base.FrameAdvance();

		ApplyGravity();
	}

	public override void InHurtbox(Vector2 collisionPnt)
	{
		if (owner.grounded)
        {
			height = HEIGHT.MID;
        }
        else
        {
			height = HEIGHT.HIGH;
        }
		base.InHurtbox(collisionPnt);
	}

	public override void AnimationFinished()
	{
		EmitSignal(nameof(StateFinished), "Fall");
	}
}
