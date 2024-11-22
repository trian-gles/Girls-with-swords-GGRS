using Godot;
using System;
using System.Collections.Generic;

public class Fall : AirState
{
	public override void _Ready()
	{
		base._Ready();
		loop = true;

		AddSpecials(owner.airSpecials);
		AddExSpecials(owner.airExSpecials);
		AddAirCommandNormals(owner.airCommandNormals);
		AddEasyAirSpecials();
		// AIRGRAB
		AddGatling(new[] { 's', 'p' },
			() =>
			{
				return (Mathf.Abs(owner.internalPos.x - owner.otherPlayer.internalPos.x) < 4000
				&& owner.internalPos.y - owner.otherPlayer.internalPos.y < 2000
				&& owner.internalPos.y - owner.otherPlayer.internalPos.y > -500
				&& owner.otherPlayer.IsAirGrabbable());
			}, "AirGrab");

		AddGatling(new[] { 'p', 'p' }, "JumpA");
		AddGatling(new[] { 'k', 'p' }, "JumpB");
		AddGatling(new[] { 's', 'p' }, "JumpC");

		// AIRDASH
		AddGatling(new List<char[]>() { new char[] { '6', 'p' }, new char[] { '6', 'p' } }, () => owner.canAirDash && owner.facingRight, "AirDash", () =>
		{
			owner.velocity.x = owner.airDashSpeed;
			owner.canDoubleJump = false;
			owner.canAirDash = false;
		}, false, false);


		AddGatling(new List<char[]>() { new char[] { '4', 'p' }, new char[] { '4', 'p' } }, () => owner.canAirDash && !owner.facingRight, "AirDash", () =>
		{
			owner.velocity.x = owner.airDashSpeed * -1;
			owner.canDoubleJump = false;
			owner.canAirDash = false;
		}, false, false);

		AddGatling(new List<char[]>() { new char[] { '6', 'p' }, new char[] { '6', 'p' } }, () => owner.canAirDash && !owner.facingRight, "AirBackdash", () =>
		{
			owner.velocity.x = owner.airBackdashSpeed;
			owner.canDoubleJump = false;
			owner.canAirDash = false;
		}, false, false);

		AddGatling(new List<char[]>() { new char[] { '4', 'p' }, new char[] { '4', 'p' } }, () => owner.canAirDash && owner.facingRight, "AirBackdash", () =>
		{
			owner.velocity.x = owner.airBackdashSpeed * -1;
			owner.canDoubleJump = false;
			owner.canAirDash = false;
		}, false, false);

		// EASY AIRDASH
		AddGatling(new char[] { 'c', 'p' }, () => owner.CheckFlippableHeldKey('6') && owner.canAirDash, "AirDash", () => {
			owner.velocity.x = owner.airDashSpeed;
			if (!owner.facingRight)
				owner.velocity.x *= -1;
			owner.canDoubleJump = false;
			owner.canAirDash = false;
		});

		AddGatling(new char[] { '6', 'p' }, () => owner.CheckBuffer(new char[] { 'c', 'p' }) && owner.canAirDash, "AirDash", () => {
			owner.velocity.x = owner.airDashSpeed;
			if (!owner.facingRight)
				owner.velocity.x *= -1;
			owner.canDoubleJump = false;
			owner.canAirDash = false;
		});

		AddGatling(new char[] { 'c', 'p' }, () => owner.CheckFlippableHeldKey('4') && owner.canAirDash, "AirBackdash", () => {
			owner.velocity.x = owner.airBackdashSpeed;
			if (owner.facingRight)
				owner.velocity.x *= -1;
			owner.canDoubleJump = false;
			owner.canAirDash = false;
		});

		AddGatling(new char[] { '4', 'p' }, () => owner.CheckBuffer(new char[] { 'c', 'p' }) && owner.canAirDash, "AirBackdash", () => {
			owner.velocity.x = owner.airBackdashSpeed;
			if (owner.facingRight)
				owner.velocity.x *= -1;
			owner.canDoubleJump = false;
			owner.canAirDash = false;
		});

		// DOUBLEJUMP
		AddGatling(new char[] { '8', 'p' }, () => owner.CheckHeldKey('6') && owner.canDoubleJump, "DoubleJump", () =>
		{
			owner.CheckTurnAround();
			owner.velocity.x = Math.Max(owner.speed, owner.velocity.x);
			owner.canDoubleJump = false;
            owner.canAirDash = false;
        });
		AddGatling(new char[] { '8', 'p' }, () => owner.CheckHeldKey('4') && owner.canDoubleJump, "DoubleJump", () =>
		{
			owner.CheckTurnAround();
			owner.velocity.x = Mathf.Min(-owner.speed, owner.velocity.x);
			owner.canDoubleJump = false;
            owner.canAirDash = false;
        });
		AddGatling(new char[] { '8', 'p' }, () => owner.canDoubleJump, "DoubleJump", () =>
		{
			owner.velocity.x = 0;
			owner.canDoubleJump = false;
            owner.canAirDash = false;
        });
	}
	public override void FrameAdvance()
	{
		base.FrameAdvance();
		if (owner.grounded && frameCount > 0)
		{
			owner.ForceEvent(EventScheduler.EventType.AUDIO, "Landing");
			EmitSignal(nameof(StateFinished), "Landing");
		}
		if (!owner.canDoubleJump)
		{
			owner.CheckTurnAround();
		}
		ApplyGravity();
	}

	public override void PushMovement(float _xVel)
	{
	}

}
