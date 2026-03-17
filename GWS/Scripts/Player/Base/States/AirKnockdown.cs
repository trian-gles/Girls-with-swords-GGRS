using Godot;
using System;

/// <summary>
/// Never ending untechable air state
/// </summary>
public class AirKnockdown : Float
{
	private const string FloatString = "Float";
	private const string KnockdownString = "Knockdown";
	private const string DownString = "Down";

	public override string animationName { get { return FloatString; } }

    public override void _Ready()
    {
        base._Ready();
		tags.Add(Globals.Tags.knockdown);
    }

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
				owner.ChangeState(KnockdownString);
			else
			{
				owner.ChangeState(DownString);
				owner.ResetComboAndProration();
			}

		}
		ApplyGravity();
	}
}
