using Godot;
using System;

public class HitStun : HitState
{
	public override void _Ready()
	{
		base._Ready();
		loop = false;
	}
	public override void Enter()
	{
		base.Enter();
		
		owner.GFXEvent("Blood");
	}


	public override void FrameAdvance()
	{
		base.FrameAdvance();
		if (owner.counterStopFrames == 0)
			stunRemaining--;

		if (stunRemaining == 0)
		{
			ExitHitstun();
		}

	}

	public virtual void ExitHitstun()
	{
        if (owner.electrocuted)
        {
            ReceiveElectrocution();
        }
        else
            EmitSignal(nameof(StateFinished), "Idle");
    }
	
	public override void ReceiveHit(Globals.AttackDetails details)
	{
		ReceiveHitNoBlock(details);
	}
}

