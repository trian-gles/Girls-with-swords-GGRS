using Godot;
using System;
using System.Collections.Generic;

public class PreRun : MoveState
{
	public override void _Ready()
	{
		base._Ready();
		loop = true;
		AddExSpecials(owner.groundExSpecials);
		AddSpecials(owner.groundSpecials);
		AddEasyGroundSpecials();
		AddGatling(new[] { '6', 'r' }, () => frameCount > 1, "PostRun");
		AddGatling(new[] { '4', 'r' }, () => frameCount > 1, "PostRun");
		AddGatling(new[] { '8', 'p' }, "MovingJump");
		AddCommandNormals(owner.commandNormals);
		AddNormals();
	}


	public override void FrameAdvance()
	{
		//GD.Print("PreRun frame advance");
		frameCount++;
		int mod = (owner.velocity.x > 0) ? 1 : -1;
		owner.velocity = new Vector2(owner.velocity.x + owner.accel * mod, 0);
		if (Math.Abs(owner.velocity.x) >= owner.dashSpeed)
		{
			EmitSignal(nameof(StateFinished), "Run");
		}
	}

	public override void ReceiveHit(Globals.AttackDetails details)
	{
		ReceiveHitNoBlock(details);
	}

}

