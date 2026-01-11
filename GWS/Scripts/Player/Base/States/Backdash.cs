using Godot;
using System;

public class Backdash: Walk
{
	[Export]
	public int len = 20;

	[Export]
	public int hopForce = 100;

	private Vector2 dustEmissionVector = new Vector2();

	public override void Enter()
	{
		frameCount = 0;
		if (owner.CheckHeldKey('8'))
		{
			EmitSignal(nameof(StateFinished), "PreJump");
		}

		owner.velocity.y = -1 * hopForce;
		owner.ScheduleEvent(EventScheduler.EventType.AUDIO);
		owner.grounded = false;
		dustEmissionVector.x = owner.internalPos.x;
		dustEmissionVector.y = owner.GetCollisionRect().End.y;

		GetNode<Node>("/root/Globals").EmitSignal(nameof(PlayerFXEmitted), 
			dustEmissionVector, 
			"dust", !owner.facingRight);
	}

	public override void HandleInput(char[] inputArr)
	{

	}

	public override void FrameAdvance()
	{
		frameCount++;
		if (frameCount == len)
		{
			EmitSignal(nameof(StateFinished), "Idle");
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
