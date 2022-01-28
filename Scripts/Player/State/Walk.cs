using Godot;
using System;
using System.Collections.Generic;

public class Walk : State
{
	protected int soundRate = 15;
	public override void _Ready()
	{
		base._Ready();
		loop = true;
		AddGatling(new[] { 's', 'p' }, () => (Mathf.Abs(owner.internalPos.x - owner.otherPlayer.internalPos.x) < 3500) && owner.otherPlayer.IsGrabbable(), "Grab");
		AddGatling(new[] { 'p', 'p' }, "Jab");
		AddGatling(new[] { 'k', 'p' }, "Kick");
		AddGatling(new[] { 's', 'p' }, "Slash");
		AddGatling(new[] { '8', 'p' }, "MovingJump");
		AddGatling(new[] { '2', 'p' }, "Crouch");
		AddGatling(new[] { '6', 'r' }, "Idle");
		AddGatling(new[] { '4', 'r' }, "Idle");
		AddGatling(new List<char[]>() { new char[] { '2', 'p' }, new char[] { '6', 'p' }, new[] { 'p', 'p' } }, "Hadouken");
		AddGatling(new List<char[]>() { new char[] { '6', 'p' }, new char[] { '6', 'p' } }, "PreRun", () => { owner.velocity.x = owner.speed; if (!owner.facingRight) { owner.velocity.x *= -1; } }, false);
		AddGatling(new List<char[]>() { new char[] { '4', 'p' }, new char[] { '4', 'p' } }, "Backdash", () => { owner.velocity.x = owner.speed * -2; if (!owner.facingRight) { owner.velocity.x *= -1; } }, false);
		AddGatling(new List<char[]>() { new char[] { '6', 'p' }, new char[] { '2', 'p' }, new char[] { '6', 'r' }, new char[] { '4', 'p' }, new char[] { '2', 'r' }, new[] {'k', 'p' } }, "CommandRun");
	}

	public override void Enter()
	{
		base.Enter();
		if (owner.CheckHeldKey('8'))
		{
			EmitSignal(nameof(StateFinished), "MovingJump");
		}
	}

	public override void FrameAdvance()
	{
		base.FrameAdvance();
		owner.CheckTurnAround();
		if (frameCount % soundRate == 0)
		{
			owner.ScheduleEvent(EventScheduler.EventType.AUDIO, "Step", Name);
		}
	}

	public override void PushMovement(float _xVel)
	{
	}
}

