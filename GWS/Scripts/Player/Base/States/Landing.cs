using Godot;
using System;
using System.Collections.Generic;

public class Landing : State
{
	[Export]
	public int len = 3;
	public override void _Ready()
	{
		base._Ready();
		animationName = "None";
		stop = false;
		AddNormals();
		AddSpecials(owner.groundSpecials);
	}
	//public override bool DelayInputs()
	//{
	//	return true;
	//}
	public override void Enter()
    {
		base.Enter();
		owner.canDoubleJump = true;
        owner.canAirDash = true;
    }
	public override void FrameAdvance()
	{
		base.FrameAdvance();
		if (frameCount == len)
			EmitSignal(nameof(StateFinished), "Idle");
	}

	public override void ReceiveHit(Globals.AttackDetails details)
	{
		ReceiveHitNoBlock(details);
	}

    public override void Exit()
    {
        base.Exit();
		owner.velocity.x = 0;
    }
}


