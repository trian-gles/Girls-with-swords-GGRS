using Godot;
using System;

public class GLj2c : BaseAttack
{
	private const string FallString = "Fall";
	public override void _Ready()
	{
		base._Ready();
		tags.Add(Globals.Tags.aerial);
		AddSpecials(owner.airSpecials);
		AddExSpecials(owner.airExSpecials);
		slowdownSpeed = 0;
		AddCancel("Fall");
		AddKara(new char[] { 'k', 'p' }, "AirGrabStart");
		hitDetails.airBlockable = true;
	}
	public override void FrameAdvance()
	{
		base.FrameAdvance();
		if (owner.airDashFrames > 0)
			owner.airDashFrames--;
		else
			ApplyGravity();
	}

	public override void InHurtbox(Vector2 collisionPnt)
	{
		if (owner.grounded)
		{
			hitDetails.height = HEIGHT.MID;
		}
		else
		{
			hitDetails.height = HEIGHT.HIGH;
		}
		base.InHurtbox(collisionPnt);
	}

	public override void AnimationFinished()
	{
		owner.ChangeState(FallString);
	}
}
