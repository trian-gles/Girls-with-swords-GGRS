using Godot;
using System;
using System.Collections.Generic;

public class Walk : MoveState
{
	protected int soundRate = 15;
	public override void _Ready()
	{
		base._Ready();
		loop = true;
		AddExSpecials(owner.groundExSpecials);
		// AddGatling(new[] { 's', 'p' }, () => (Mathf.Abs(owner.internalPos.x - owner.otherPlayer.internalPos.x) < 2000) && owner.otherPlayer.IsGrabbable(), "Grab");
		AddGatling(new[] { '8', 'p' }, "PreJump");
		AddGatling(new[] { '2', 'p' }, "Crouch");
		AddGatling(new[] { '6', 'r' }, "Idle");
		AddGatling(new[] { '4', 'r' }, "Idle");

		AddSpecials(owner.groundSpecials);
		AddCommandNormals(owner.commandNormals);
		AddEasyGroundSpecials();
		AddNormals();
		AddGatling(new List<char[]>() { new char[] { '6', 'p' }, new char[] { '6', 'p' } }, "PreRun", () => { owner.velocity.x = owner.speed; if (!owner.facingRight) { owner.velocity.x *= -1; } }, false);

		AddGatling(new List<char[]>() { new char[] { '4', 'p' }, new char[] { '4', 'p' } }, "Backdash", () => { owner.velocity.x = owner.speed * -2; if (!owner.facingRight) { owner.velocity.x *= -1; } }, false);

		AddGatling(new[] { 'c', 'p' }, () => { return ((owner.velocity.x > 0) == owner.facingRight); },
			"PreRun", () => { owner.velocity.x = owner.speed; if (!owner.facingRight) { owner.velocity.x *= -1; } });

		AddGatling(new[] { 'c', 'p' }, () => { return ((owner.velocity.x > 0) != owner.facingRight); },
			"Backdash", () => { owner.velocity.x = owner.speed * -2; if (!owner.facingRight) { owner.velocity.x *= -1; } });
	}

	public override void Enter()
	{
		base.Enter();
		if (owner.CheckHeldKey('8'))
		{
			EmitSignal(nameof(StateFinished), "MovingJump");
		}

		if (owner.CheckHeldKey('c') && owner.CheckBuffer(new[] {'c', 'p'}))
        {
			if ((owner.velocity.x > 0) == owner.facingRight) {
				owner.velocity.x = owner.speed; 
				if (!owner.facingRight) 
					owner.velocity.x *= -1;
				EmitSignal(nameof(StateFinished), "PreRun");

			}
			else
            {
				owner.velocity.x = owner.speed * -2; 
				if (!owner.facingRight) 
					owner.velocity.x *= -1;
				EmitSignal(nameof(StateFinished), "Backdash");
			}

			
		}
	}

	public override void FrameAdvance()
	{
		base.FrameAdvance();
		owner.CheckTurnAround();
		if ((frameCount - 3) % soundRate == 0)
		{
			owner.ScheduleEvent(EventScheduler.EventType.AUDIO, "Step", Name);
		}
	}

	public override void PushMovement(float _xVel)
	{
	}
}

