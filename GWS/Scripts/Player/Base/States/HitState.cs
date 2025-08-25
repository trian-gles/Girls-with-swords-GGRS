using Godot;
using System;
using System.Collections.Generic;

public class HitState : State
{
	public override HashSet<string> tags { get; set; } = new HashSet<string>() { "hitstate" };

	public override bool wasHit
	{ get { return true; } }

    public override void Enter()
    {
        base.Enter();
		stunRemaining = 0;
    }

    public override bool DelayInputs()
	{
		return frameCount > stunRemaining - 3;
	}

	public override void Exit()
	{
		base.Exit();
        owner.grabInvulnFrames = 5;
	}

	protected void ReceiveElectrocution()
	{
		var hit = Globals.electrocuteDetails;
		if (owner.grounded)
		{
			hit.opponentLaunch = Vector2.Zero;
		}
		else
		{
			hit.hitStun = 32;
		}
			
		owner.ReceiveHit(hit, hit);
		owner.electrocuted = false;
	}
}
