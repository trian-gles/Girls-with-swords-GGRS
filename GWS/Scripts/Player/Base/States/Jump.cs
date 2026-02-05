using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class Jump : AirState
{
	private const string IdleString = "Idle";
	private const string AirGrabStartString = "AirGrabStart";
	private const string FallString = "Fall";
	private const string JumpString = "Jump";
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
			() => owner.CheckHeldKey('k'), "AirGrabStart");
		AddGatling(new[] { 'k', 'p' },
			() => owner.CheckHeldKey('s'), "AirGrabStart");

		AddSpecials(owner.airSpecials);
		AddExSpecials(owner.airExSpecials);
		AddAirCommandNormals(owner.airCommandNormals);
		AddEasyAirSpecials();
		// ATTACKS
		AddGatling(new[] { 'p', 'p' }, "JumpA");
		AddGatling(new[] { 'k', 'p' }, "JumpB");
		AddGatling(new[] { 's', 'p' }, "JumpC");

		AddAirdash();

		// DOUBLE JUMP
		AddGatling(new char[] { '8', 'p' }, () => owner.CheckHeldKey('6') && owner.canDoubleJump && LateEnoughDoubleJump(), "DoubleJump", () =>
		{
			owner.CheckTurnAround();
			owner.velocity.x = Math.Max(owner.speed, owner.velocity.x);
			owner.canDoubleJump = false;
			owner.canAirDash = false;
			owner.hasDoubleOrSuperJumped = true;
		});
		AddGatling(new char[] { '8', 'p' }, () => owner.CheckHeldKey('4') && owner.canDoubleJump && LateEnoughDoubleJump(), "DoubleJump", () =>
		{
			owner.CheckTurnAround();
			owner.velocity.x = Mathf.Min(-owner.speed, owner.velocity.x);
			owner.canDoubleJump = false;
			owner.canAirDash = false;
			owner.hasDoubleOrSuperJumped = true;
		});
		AddGatling(new char[] { '8', 'p' }, () => owner.canDoubleJump && LateEnoughDoubleJump(), "DoubleJump", () =>
		{
			owner.velocity.x = 0;
			owner.canDoubleJump = false;
			owner.canAirDash = false;
			owner.hasDoubleOrSuperJumped = true;
		});


	}
	
	private bool LateEnoughDoubleJump()
    {
		return frameCount > 13;
    }

	public override void Enter()
	{
		base.Enter();
		owner.velocity.y = -1 * owner.jumpForce;
		owner.grounded = false;
		owner.ScheduleEvent(EventScheduler.EventType.AUDIO, JumpString, Name);

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
	}

	public override bool DelayInputs()
	{
		return frameCount < startupFrames && owner.canDoubleJump;
	}

    public override void HandleInput(char[] inputArr)
    {
        base.HandleInput(inputArr);

		// Allows the user to choose direction slightly into the jump
		if (frameCount < 3)
		{
			if (Enumerable.SequenceEqual(inputArr, Globals.RIGHTPRESS))
            {
				owner.velocity.x = Math.Max(owner.speed, owner.velocity.x);
			}
				
			else if (Enumerable.SequenceEqual(inputArr, Globals.LEFTPRESS))
            {
				owner.velocity.x = Mathf.Min(-owner.speed, owner.velocity.x);
			}
				
		}
    }


    public override void FrameAdvance()
	{
		base.FrameAdvance();
		if (owner.grounded && frameCount > 0) 
		{
			owner.ChangeState(IdleString);
		}
		ApplyGravity();
		if (!owner.canDoubleJump)
        {
			owner.CheckTurnAround();
        }

		if (DelayInputs() && owner.CheckHitStopBuffer(Globals.KICKPRESS) && owner.CheckHitStopBuffer(Globals.SLASHPRESS))
		{
			owner.ChangeState(AirGrabStartString);
		}
		
	}

	public override void PushMovement(float _xVel)
	{
	}

	public override void AnimationFinished()
	{
		owner.ChangeState(FallString);
	}

    public override void ReceiveHit(Globals.AttackDetails details)
    {
		if (frameCount < 3)
			ReceiveHitNoBlock(details);
		else
        	base.ReceiveHit(details);
    }
}


