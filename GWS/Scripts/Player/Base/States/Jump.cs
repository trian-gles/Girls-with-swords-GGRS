using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Jump : AirState
{
	[Export]
	public int startupFrames = 8;
	public override void _Ready()
	{
		base._Ready();

		// AIRGRAB
		//AddGatling(new[] { 's', 'p' },
		//	() =>
		//	{
		//		return (Mathf.Abs(owner.internalPos.x - owner.otherPlayer.internalPos.x) < 4000
		//		&& owner.internalPos.y - owner.otherPlayer.internalPos.y < 2000
		//		&& owner.internalPos.y - owner.otherPlayer.internalPos.y > -500
		//		&& owner.otherPlayer.IsAirGrabbable());
		//	}, "AirGrab");


		// NEW AIRGRAB
		AddGatling(new[] { 's', 'p' },
			() => owner.CheckHeldKey('4') || owner.CheckHeldKey('6'), "AirGrabStart");

		// ATTACKS
		AddGatling(new[] { 'p', 'p' }, "JumpA");
		AddGatling(new[] { 'k', 'p' }, "JumpB");
		AddGatling(new[] { 's', 'p' }, "JumpC");

		// AIRDASH
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

		// DOUBLE JUMP
		AddGatling(new char[] { '8', 'p' }, () => owner.CheckHeldKey('6') && owner.canDoubleJump, "DoubleJump", () =>
		{
			owner.CheckTurnAround();
			owner.velocity.x = Math.Max(owner.speed, owner.velocity.x);
			owner.canDoubleJump = false;
		});
		AddGatling(new char[] { '8', 'p' }, () => owner.CheckHeldKey('4') && owner.canDoubleJump, "DoubleJump", () =>
		{
			owner.CheckTurnAround();
			owner.velocity.x = Mathf.Min(-owner.speed, owner.velocity.x);
			owner.canDoubleJump = false;
		});
		AddGatling(new char[] { '8', 'p' }, () => owner.canDoubleJump, "DoubleJump", () =>
		{
			owner.velocity.x = 0;
			owner.canDoubleJump = false;
		});

		
	}
	public override void Enter()
	{
		base.Enter();
		owner.velocity.y = -1 * owner.jumpForce;
		owner.grounded = false;
		owner.ScheduleEvent(EventScheduler.EventType.AUDIO, "Jump", Name);
		//GD.Print("Jump");

		if (owner.CheckHeldKey('6'))
		{
			owner.velocity.x = Mathf.Max(owner.speed, owner.velocity.x);
		}

		else if (owner.CheckHeldKey('4'))
		{
			owner.velocity.x = Mathf.Min(-owner.speed, owner.velocity.x);
		}


	}

	public override void Exit()
	{
		base.Exit();
		//GD.Print(frameCount);
	}

	public override bool DelayInputs()
	{
		return frameCount < startupFrames && owner.canDoubleJump;
	}

    public override void HandleInput(char[] inputArr)
    {
        base.HandleInput(inputArr);
		if (frameCount < 3)
		{
			if (Enumerable.SequenceEqual(inputArr, new char[] { '6', 'p' }))
            {
				owner.velocity.x = Math.Max(owner.speed, owner.velocity.x);
				GD.Print($"Using delayed input from 6 press, vel is now {owner.velocity.x}");
			}
				
			else if (Enumerable.SequenceEqual(inputArr, new char[] { '4', 'p' }))
            {
				owner.velocity.x = Mathf.Min(-owner.speed, owner.velocity.x);
				GD.Print("Using delayed input");
			}
				
		}
    }


    public override void FrameAdvance()
	{
		base.FrameAdvance();
		if (owner.grounded && frameCount > 0) 
		{
			EmitSignal(nameof(StateFinished), "Idle");
		}
		ApplyGravity();
		if (!owner.canDoubleJump)
        {
			owner.CheckTurnAround();
        }
		
	}

	public override void PushMovement(float _xVel)
	{
	}

	public override void AnimationFinished()
	{
		EmitSignal(nameof(StateFinished), "Fall");
	}
}


