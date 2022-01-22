using Godot;
using System;
using System.Collections.Generic;


public abstract class AirAttack : BaseAttack
{
	protected override void AddJumpCancel()
	{
		AddGatling(new char[] { '8', 'p' }, () => owner.CheckHeldKey('6') && owner.canDoubleJump, "Jump", () => owner.velocity.x = owner.speed);
		AddGatling(new char[] { '8', 'p' }, () => owner.CheckHeldKey('4') && owner.canDoubleJump, "Jump", () => owner.velocity.x = -owner.speed);
		AddGatling(new char[] { '8', 'p' }, () => owner.canDoubleJump, "Jump");
	}
}
