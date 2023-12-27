using Godot;
using System;
using System.Collections.Generic;
using System.Linq;


public abstract class AirAttack : BaseAttack
{
	public override void _Ready()
    {
        base._Ready();
		tags.Add("aerial");
		slowdownSpeed = 0;
		AddCancel("Fall");
		hitDetails.airBlockable = true;
	}
	protected override void AddJumpCancel()
	{
		AddGatling(new char[] { '8', 'p' }, () => owner.CheckHeldKey('6') && owner.canDoubleJump, "DoubleJump", () =>
		{
			owner.velocity.x = owner.speed;
			owner.canDoubleJump = false;
			owner.canAirDash = false;
		});
		AddGatling(new char[] { '8', 'p' }, () => owner.CheckHeldKey('4') && owner.canDoubleJump, "DoubleJump", () =>
		{
			owner.velocity.x = -owner.speed;
			owner.canDoubleJump = false;
			owner.canAirDash = false;
		});
		AddGatling(new char[] { '8', 'p' }, () => owner.canDoubleJump, "DoubleJump", () =>
		{
			owner.canDoubleJump = false;
			owner.canAirDash = false;

		});
	}

	//public override bool DelayInputs()
	//{
	//	return owner.internalPos.y > 19000;
//
	//}

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

	public override void FrameAdvance()
	{
		base.FrameAdvance();
		if (owner.grounded && frameCount > 1)
		{
			EmitSignal(nameof(StateFinished), "Landing");
		}

		if (restoreHitFrames != null && restoreHitFrames.Contains(frameCount))
		{
			hitConnect = false;
		}


		Globals.Log($"{Name} at position {owner.Position}");

		if (owner.airDashFrames > 0)
			owner.airDashFrames--;
		else
			ApplyGravity();
	}

}
