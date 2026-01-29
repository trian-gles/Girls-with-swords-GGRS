using Godot;
using System;
using System.Collections.Generic;

public class HitState : State
{
	public override HashSet<Globals.Tags> tags { get; set; } = new HashSet<Globals.Tags>() { Globals.Tags.hitstate };

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
        owner.grabInvulnFrames = 7;
	}

    public override bool IsGrabbable()
	{
		return false;
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
	
	protected void TryTech()
	{

		if (stunRemaining == 1 && owner.electrocuted)
			ReceiveElectrocution();

		if (stunRemaining <= 0)
		{
			if (owner.CheckHeldKey('p') || owner.CheckHeldKey('k') || owner.CheckHeldKey('s') || Globals.autoTech)
				owner.ChangeState("Tech");
			else if (owner.wasOTGHit)
			{
				owner.invulnFrames = 8;
				owner.ChangeState("Tech");
			}
		}
	}
}
