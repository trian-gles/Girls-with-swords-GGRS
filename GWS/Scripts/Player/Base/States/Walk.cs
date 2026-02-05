using Godot;
using System;
using System.Collections.Generic;

public class Walk : MoveState
{
	protected int soundRate = 15;
	private const string PreJumpString = "PreJump";
	private const string CrouchString = "Crouch";
	private const string IdleString = "Idle";
	private const string PreRunString = "PreRun";
	private const string BackdashString = "Backdash";
	private const string StepString = "Step";
	public override void _Ready()
	{
		base._Ready();
		loop = true;
		AddExSpecials(owner.groundExSpecials);
		// AddGatling(new[] { 's', 'p' }, () => (Mathf.Abs(owner.internalPos.x - owner.otherPlayer.internalPos.x) < 2000) && owner.otherPlayer.IsGrabbable(), "Grab");
		AddGatling(new[] { '8', 'p' }, PreJumpString);
		AddGatling(new[] { '2', 'p' }, CrouchString);
		AddGatling(new[] { '6', 'r' }, IdleString);
		AddGatling(new[] { '4', 'r' }, IdleString);

		AddSpecials(owner.groundSpecials);
		AddCommandNormals(owner.commandNormals);
		AddEasyGroundSpecials();
		AddNormals();
		AddGatling(new InputContainer( new[]{ new char[] { '6', 'p' }, new char[] { '6', 'r' }, new char[] { '6', 'p' } }), PreRunString, () => { owner.velocity.x = owner.speed; if (!owner.facingRight) { owner.velocity.x *= -1; } }, false);
		AddGatling(new InputContainer( new[] { new char[] { '4', 'p' }, new char[] { '4', 'r' }, new char[] { '4', 'p' } }), () => owner.backdashCooldownRemaining == 0, BackdashString, 
			() => 
			{ 
				owner.velocity.x = owner.speed * -2; 
				if (!owner.facingRight) 
				{ 
					owner.velocity.x *= -1; 
				} 
				owner.backdashCooldownRemaining = 30;
			}, 
			false);

		AddGatling(new[] { 'c', 'p' }, () => { return ((owner.velocity.x > 0) == owner.facingRight); },
			PreRunString, () => { owner.velocity.x = owner.speed; if (!owner.facingRight) { owner.velocity.x *= -1; } });

		AddGatling(new[] { 'c', 'p' }, () => { return ((owner.velocity.x > 0) != owner.facingRight) && owner.backdashCooldownRemaining == 0; },
			BackdashString, () => { 
				owner.velocity.x = 
				owner.speed * -2; 
				if (!owner.facingRight) { owner.velocity.x *= -1; }
				owner.backdashCooldownRemaining = 30;
			});
	}

	public override void Enter()
	{
		base.Enter();
		if (owner.CheckHeldKey('8'))
		{
			owner.ChangeState(PreJumpString);
		}

		if (owner.CheckHeldKey('c') && owner.CheckBuffer(Globals.SLASHPRESS))
        {
			if ((owner.velocity.x > 0) == owner.facingRight) {
				owner.velocity.x = owner.speed; 
				if (!owner.facingRight) 
					owner.velocity.x *= -1;
				owner.ChangeState(PreRunString);

			}

			
		}
	}

	public override void FrameAdvance()
	{
		base.FrameAdvance();
		owner.CheckTurnAround();
		if ((frameCount - 3) % soundRate == 0)
		{
			owner.ScheduleEvent(EventScheduler.EventType.AUDIO, StepString, Name);
		}
	}

	public override void PushMovement(float _xVel)
	{
	}
}

