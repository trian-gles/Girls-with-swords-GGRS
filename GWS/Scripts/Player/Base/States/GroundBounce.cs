using Godot;
using System;
using System.Collections.Generic;

public class GroundBounce : Float
{
	private const string FloatAnimString = "Float";
	public override string animationName { get { return FloatAnimString; } }
	private bool bounced = false;
	private int BOUNCEINDEX = 0;
	private const string KnockdownString = "Knockdown";
	private const string GroundBounceGfx = "GroundBounce";


	public override void Enter()
	{
		base.Enter();
		bounced = false;
	}
	public override void Load(int[] loadData)
	{
		bounced = Convert.ToBoolean(loadData[BOUNCEINDEX]);
	}

	public override int[] Save()
	{
		stateStateArray[BOUNCEINDEX] = Convert.ToInt32(bounced);
		return stateStateArray;
	}

    public override int CheckTerminalVelocity()
    {
		if (bounced)
			return base.CheckTerminalVelocity();
		else
			return owner.standardTerminalVelocity;
    }

    public override void FrameAdvance()
	{
		frameCount++;
		if (owner.grounded)
		{
			if (bounced)
			{
				if (stunRemaining > 20)
					owner.ChangeState(KnockdownString);
				else
				{
					owner.grounded = false;
					TryGroundTech();
				}

				owner.ResetComboAndProration();
			}
			else if (owner.canGroundbounce)
			{
					owner.GFXEvent(GroundBounceGfx);
				bounced = true;
				owner.grounded = false;
				owner.velocity.y = (int)Math.Floor(owner.velocity.y * -3 / 5);
				owner.canGroundbounce = false;
			}
			else
			{
				owner.grounded = false;
				TryGroundTech();
			}
			
		}

		stunRemaining--;

		TryTech();


		ApplyGravity();
	}
}

