using Godot;
using System;

public class Run : MoveState
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

		AddGatling(new[] { '6', 'r' }, "PostRun");
		AddGatling(new[] { '4', 'r' }, "PostRun");

		soundRate = 10;
	}

	public override void Enter()
	{
		base.Enter();
		if (owner.velocity.x < 0) { owner.velocity.x = -owner.dashSpeed;}
		else { owner.velocity.x = owner.dashSpeed;}

		if (owner.CheckHeldKey('8'))
		{
			EmitSignal(nameof(StateFinished), "MovingJump");
		}
		if (!owner.CheckHeldKey('6') && !owner.CheckHeldKey('4')) // this will need to be fixed
		{
			EmitSignal(nameof(StateFinished), "PostRun");
		}
	}

	public override void FrameAdvance()
	{
		frameCount++;

		if (frameCount % soundRate == 0)
		{
			owner.ScheduleEvent(EventScheduler.EventType.AUDIO, "Step", Name);
		}
	}

	public override void PushMovement(float _xVel)
	{
	}
}
