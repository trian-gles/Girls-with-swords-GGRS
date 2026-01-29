using Godot;
using System;

/// <summary>
/// Never ending untechable air state
/// </summary>
public class AirKnockdown : Float
{
	private string floatString = "Float";
	private string knockdownString = "Knockdown";
	private string downString = "Down";

	public override string animationName { get { return floatString; } }

    public override void Enter()
    {
        base.Enter();
    }

    public override void FrameAdvance()
	{
		frameCount++;
		if (owner.grounded)
		{
			if (owner.health > 0)
				owner.ChangeState(knockdownString);
			else
				owner.ChangeState(downString);
				owner.ResetComboAndProration();
		}
		ApplyGravity();
	}
}
