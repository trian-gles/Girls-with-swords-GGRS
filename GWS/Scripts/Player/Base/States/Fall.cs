using Godot;
using System;
using System.Collections.Generic;

public class Fall : AirState
{
	public override void _Ready()
	{
		base._Ready();
		loop = true;
		AddGatling(new[] { 'p', 'p' }, "JumpA");
		AddGatling(new[] { 'k', 'p' }, "JumpB");
		AddGatling(new[] { 's', 'p' }, "JumpC");

		// AIRDASH
		AddGatling(new List<char[]>() { new char[] { '6', 'p' }, new char[] { '6', 'p' } }, () => owner.canDoubleJump && owner.facingRight, "AirDash", () =>
		{
			owner.velocity.x = owner.speed * 3;
			owner.canDoubleJump = false;
		}, false, false);


		AddGatling(new List<char[]>() { new char[] { '4', 'p' }, new char[] { '4', 'p' } }, () => owner.canDoubleJump && !owner.facingRight, "AirDash", () =>
		{
			owner.velocity.x = owner.speed * -3;
			owner.canDoubleJump = false;
		}, false, false);

		AddGatling(new List<char[]>() { new char[] { '6', 'p' }, new char[] { '6', 'p' } }, () => owner.canDoubleJump && !owner.facingRight, "AirBackdash", () =>
		{
			owner.velocity.x = owner.speed * 3;
			owner.canDoubleJump = false;
		}, false, false);

		AddGatling(new List<char[]>() { new char[] { '4', 'p' }, new char[] { '4', 'p' } }, () => owner.canDoubleJump && owner.facingRight, "AirBackdash", () =>
		{
			owner.velocity.x = owner.speed * -3;
			owner.canDoubleJump = false;
		}, false, false);

		// DOUBLEJUMP
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
	public override void FrameAdvance()
	{
		base.FrameAdvance();
		if (owner.grounded && frameCount > 0)
		{
			owner.ForceEvent(EventScheduler.EventType.AUDIO, "Landing");
			EmitSignal(nameof(StateFinished), "Landing");
		}
		ApplyGravity();
	}

	public override void PushMovement(float _xVel)
	{
	}

}
