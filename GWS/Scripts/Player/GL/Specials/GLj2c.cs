using Godot;
using System;

public class GLj2c : BaseAttack
{
	public override void _Ready()
	{
		base._Ready();
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

	//protected override void EnterHitState(bool knockdown, Vector2 launch, Vector2 collisionPnt, BaseAttack.EXTRAEFFECT effect)
	//{
	//	GetNode<Node>("/root/Globals").EmitSignal(nameof(PlayerFXEmitted), collisionPnt, "hit", false);
	//	bool launchBool = false;
	//	owner.ComboUp();
	//	if (!(launch == Vector2.Zero))
	//	{
	//		owner.velocity = launch;
	//		launchBool = true;
	//	}

	//	if (launch.y == 0)
	//	{
	//		owner.velocity.y = -400;
	//	}

	//	bool airState = (launchBool || !owner.grounded);

	//	if (!knockdown)
	//	{
	//		EmitSignal(nameof(StateFinished), "CounterFloat");
	//	}
	//	else
	//	{
	//		EmitSignal(nameof(StateFinished), "AirKnockdown");
	//	}
	//}

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
		EmitSignal(nameof(StateFinished), "Fall");
	}
}
