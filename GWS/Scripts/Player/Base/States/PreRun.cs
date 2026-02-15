using Godot;
using System;
using System.Collections.Generic;

public class PreRun : MoveState
{
	private const string PostRunString = "PostRun";
	private const string PreJumpString = "PreJump";
	private const string RunString = "Run";
	public override void _Ready()
	{
		base._Ready();
		loop = true;
		tags.Add(Globals.Tags.run);
		AddExSpecials(owner.groundExSpecials);
		AddSpecials(owner.groundSpecials);
		AddEasyGroundSpecials();
		AddGatling(new[] { '6', 'r' }, () => frameCount > 1, PostRunString);
		AddGatling(new[] { '4', 'r' }, () => frameCount > 1, PostRunString);
		AddGatling(new[] { '8', 'p' }, PreJumpString);
		AddCommandNormals(owner.commandNormals);
		AddNormals();
	}


	public override void FrameAdvance()
	{
		frameCount++;
		int mod = (owner.velocity.x > 0) ? 1 : -1;
		owner.velocity.x += owner.accel * mod;
		owner.velocity.y = 0;
		if (Math.Abs(owner.velocity.x) >= owner.dashSpeed)
		{
			owner.ChangeState(RunString);
		}
	}

	public override void ReceiveHit(Globals.AttackDetails details)
	{
		ReceiveHitNoBlock(details);
	}

}

