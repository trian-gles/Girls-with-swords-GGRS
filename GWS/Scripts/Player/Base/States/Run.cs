using Godot;
using System;
using System.Collections.Generic;

public class Run : MoveState
{
	private const string PreJumpString = "PreJump";
	private const string PostRunString = "PostRun";
	private const string DustString = "dust";
	private const string StepString = "Step";
	protected int soundRate = 15;
	public override void _Ready()
	{
		base._Ready();
		loop = true;
		tags.Add(Globals.Tags.run);
		foreach (Player.Special dashSpecial in owner.dashSpecials)
			AddGatling(new[] { dashSpecial.inputs.Get(0).A, dashSpecial.inputs.Get(0).B}, () => frameCount > 5, dashSpecial.state );
		AddGatling(new[] { '8', 'p' }, PreJumpString);
		AddExSpecials(owner.groundExSpecials);
		AddSpecials(owner.groundSpecials);
		AddEasyGroundSpecials();
		AddCommandNormals(owner.commandNormals);
		
		
		AddNormals();
		AddGatling(new[] { '6', 'r' }, PostRunString);
		AddGatling(new[] { '4', 'r' }, PostRunString);
		foreach (Player.Special dashSpecial in owner.dashSpecials)
			AddGatling(new[] { dashSpecial.inputs.Get(0).A, dashSpecial.inputs.Get(0).B}, () => frameCount > 5, dashSpecial.state );
		soundRate = 10;
	}
	public override void Enter()
	{
		base.Enter();
		if (owner.velocity.x < 0) { owner.velocity.x = -owner.dashSpeed;}
		else { owner.velocity.x = owner.dashSpeed;}

		owner.GainMeter(500);

		if (owner.CheckHeldKey('8'))
		{
			owner.ChangeState(PreJumpString);
		}
		if (!owner.CheckHeldKey('6') && !owner.CheckHeldKey('4')) // this will need to be fixed
		{
			owner.ChangeState(PostRunString);
		}
	}

	public override void FrameAdvance()
	{
		frameCount++;

		if (frameCount % soundRate == 0)
		{
			owner.ScheduleEvent(EventScheduler.EventType.AUDIO, StepString, Name);
		}
	}

	public override void PushMovement(float _xVel)
	{
	}
}
