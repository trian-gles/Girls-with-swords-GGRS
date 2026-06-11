using Godot;
using System;

public class HitStun : HitState
{
	private const string BloodGfxString = "Blood";
	private const string IdleString = "Idle";

	public override int maxStun { get { return 20; } }
	public override void _Ready()
	{
		base._Ready();
		loop = false;
		tags.Add(Globals.Tags.hitstate);
	}
	public override void Enter()
	{
		base.Enter();
		
		owner.GFXEvent(BloodGfxString);
		owner.GainMeter(200);
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
			owner.ChangeState(IdleString);
    }
	
	public override void ReceiveHit(Globals.AttackDetails details)
	{
		ReceiveHitNoBlock(details);
	}
}

