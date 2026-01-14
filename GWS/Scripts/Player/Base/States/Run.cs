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
			AddGatling(dashSpecial.inputs[0], () => frameCount > 5, dashSpecial.state);
		AddGatling(new[] { '8', 'p' }, "PreJump");
		AddExSpecials(owner.groundExSpecials);
		AddSpecials(owner.groundSpecials);
		AddEasyGroundSpecials();
		AddCommandNormals(owner.commandNormals);
		
		
		AddNormals();
		AddGatling(new[] { '6', 'r' }, "PostRun");
		AddGatling(new[] { '4', 'r' }, "PostRun");
		foreach (Player.Special dashSpecial in owner.dashSpecials)
			AddGatling(dashSpecial.inputs[0], () => frameCount > 5, dashSpecial.state);

		soundRate = 10;
	}

	private Vector2 dustEmissionVector = new Vector2();
	public override void Enter()
	{
		base.Enter();
		if (owner.velocity.x < 0) { owner.velocity.x = -owner.dashSpeed;}
		else { owner.velocity.x = owner.dashSpeed;}

		owner.GainMeter(500);
		dustEmissionVector.x = owner.internalPos.x;
		dustEmissionVector.y = owner.GetCollisionRect().End.y;
		GetNode<Node>("/root/Globals").EmitSignal(nameof(PlayerFXEmitted),
			dustEmissionVector,
			"dust", owner.facingRight);

		if (owner.CheckHeldKey('8'))
		{
			owner.ChangeState("PreJump");
		}
		if (!owner.CheckHeldKey('6') && !owner.CheckHeldKey('4')) // this will need to be fixed
		{
			owner.ChangeState("PostRun");
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
