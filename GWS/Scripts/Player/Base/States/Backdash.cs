using Godot;
using System;

public class Backdash: Walk
{
	[Export]
	public int len = 20;

	[Export]
	public int hopForce = 100;
	private const string PreJumpString = "PreJump";
	private const string DustString = "dust";
	private const string IdleString = "Idle";
	private Vector2 dustEmissionVector = new Vector2();

	public override void Enter()
	{
		frameCount = 0;
		if (owner.CheckHeldKey('8'))
		{
			owner.ChangeState(PreJumpString);
		}

		owner.velocity.y = -1 * hopForce;
		owner.ScheduleEvent(EventScheduler.EventType.AUDIO);
		owner.grounded = false;
		dustEmissionVector.x = owner.internalPos.x;
		dustEmissionVector.y = owner.GetCollisionRect().End.y;

		Globals.EmitPlayerFXEmitted(dustEmissionVector, DustString, !owner.facingRight);
	}

	public override void HandleInput(char[] inputArr)
	{

	}

	public override void FrameAdvance()
	{
		frameCount++;
		if (frameCount == len)
		{
			owner.ChangeState(IdleString);
		}
		ApplyGravity();
	}

	public override void ReceiveHit(Globals.AttackDetails details)
	{
		details.hitStun += 5;
		if (owner.terminalVelocity == owner.standardTerminalVelocity)
        {
            owner.terminalVelocity = 100;
        }
		ReceiveHitNoBlock(details);
	}
}
