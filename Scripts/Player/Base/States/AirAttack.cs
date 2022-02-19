using Godot;
using System;
using System.Collections.Generic;


public abstract class AirAttack : BaseAttack
{
    public override void _Ready()
    {
        base._Ready();
		slowdownSpeed = 0;
		AddCancel("Fall");
	}
    protected override void AddJumpCancel()
	{
		AddGatling(new char[] { '8', 'p' }, () => owner.CheckHeldKey('6') && owner.canDoubleJump, "DoubleJump", () =>
		{
			owner.velocity.x = owner.speed;
			owner.canDoubleJump = false;
		});
		AddGatling(new char[] { '8', 'p' }, () => owner.CheckHeldKey('4') && owner.canDoubleJump, "DoubleJump", () =>
		{
			owner.velocity.x = -owner.speed;
			owner.canDoubleJump = false;
		});
		AddGatling(new char[] { '8', 'p' }, () => owner.canDoubleJump, "DoubleJump", () => owner.canDoubleJump = false);
	}

	protected override void EnterHitState(bool knockdown, Vector2 launch, Vector2 collisionPnt)
	{
		GetNode<Node>("/root/Globals").EmitSignal(nameof(PlayerFXEmitted), collisionPnt, "hit", false);
		bool launchBool = false;
		owner.ComboUp();
		if (!(launch == Vector2.Zero))
		{
			owner.velocity = launch;
			launchBool = true;
		}

		bool airState = (launchBool || !owner.grounded);

		if (!knockdown)
		{
			EmitSignal(nameof(StateFinished), "CounterFloat");
		}
		else
		{
			EmitSignal(nameof(StateFinished), "AirKnockdown");
		}
	}
}
