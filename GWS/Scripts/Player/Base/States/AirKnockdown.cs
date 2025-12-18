using Godot;
using System;

/// <summary>
/// Never ending untechable air state
/// </summary>
public class AirKnockdown : Float
{

	public override string animationName { get { return "Float"; } }

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
				EmitSignal(nameof(StateFinished), "Knockdown");
			else
				EmitSignal(nameof(StateFinished), "Down");
				owner.ResetComboAndProration();
		}
		ApplyGravity();
	}
}
