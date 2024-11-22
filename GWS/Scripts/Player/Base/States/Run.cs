using Godot;
using System;
using System.Collections.Generic;

public class Run : MoveState
{
	protected int soundRate = 15;
	public override void _Ready()
	{
		base._Ready();
		loop = true;
		foreach (Player.Special dashSpecial in owner.dashSpecials)
			AddGatling(dashSpecial.inputs[0], dashSpecial.state);
		AddGatling(new[] { '8', 'p' }, "PreJump");
		AddExSpecials(owner.groundExSpecials);
		AddSpecials(owner.groundSpecials);
		AddEasyGroundSpecials();
		AddCommandNormals(owner.commandNormals);
		
		
		AddNormals();
		AddGatling(new[] { '6', 'r' }, "PostRun");
		AddGatling(new[] { '4', 'r' }, "PostRun");
		foreach (Player.Special dashSpecial in owner.dashSpecials)
			AddGatling(dashSpecial.inputs[0], dashSpecial.state);

		soundRate = 10;
	}

	public override void Enter()
	{
		base.Enter();
		if (owner.velocity.x < 0) { owner.velocity.x = -owner.dashSpeed;}
		else { owner.velocity.x = owner.dashSpeed;}

		owner.GainMeter(500);
		GetNode<Node>("/root/Globals").EmitSignal(nameof(PlayerFXEmitted),
			new Vector2(owner.internalPos.x, owner.GetCollisionRect().End.y),
			"dust", owner.facingRight);

		if (owner.CheckHeldKey('8'))
		{
			EmitSignal(nameof(StateFinished), "PreJump");
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
