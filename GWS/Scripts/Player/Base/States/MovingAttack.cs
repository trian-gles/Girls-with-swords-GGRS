using Godot;
using Godot.Collections;
using System;

/// <summary>
/// Attacks that may be grounded or aerial
/// </summary>
public class MovingAttack : ComNorm
{
	[Export]
	protected int moveSpeed = 0;

	[Export]
	protected int startMovingFrame = 0;

	[Export]
	protected int stopFrame = 0;

	[Export]
	protected Array<int> dustFrames = new Array<int>();
	public override void _Ready()
	{
		base._Ready();
		slowdownSpeed = 0; // we are constantly moving forwards
		
	}

	public override void Enter()
	{
		base.Enter();
		if (startMovingFrame == 0)
			StartMoving();
	}

	private void StartMoving()
	{
		owner.velocity.x = moveSpeed;
		if (!owner.facingRight)
		{
			owner.velocity.x *= -1;
		}
	}

	private Vector2 dustEmissionVector = new Vector2();
	public override void FrameAdvance()
	{
		base.FrameAdvance();
		if (frameCount == startMovingFrame)
		{
			StartMoving();
		}
		if (frameCount > 0 && frameCount == stopFrame)
		{
			owner.velocity.x = 0;
		}

		if (dustFrames.Contains(frameCount))
		{
			dustEmissionVector.x = owner.internalPos.x;
			dustEmissionVector.y = owner.GetCollisionRect().End.y;
            GetNode<Node>("/root/Globals").EmitSignal(nameof(PlayerFXEmitted),
            dustEmissionVector,
            "dust", owner.facingRight);
        }
		
	}
}

