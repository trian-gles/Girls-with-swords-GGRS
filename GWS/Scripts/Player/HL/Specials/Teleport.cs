using Godot;
using System;

public class Teleport : BaseAttack
{
	[Export]
	public int teleFrame;
	private const string IdleString = "Idle";
	private const string FallString = "Fall";
	private const string HatString = "Hat";
	public override void FrameAdvance()
	{

		if (frameCount == teleFrame)
		{
			if (((HL)owner).hatted)
			{
				owner.ChangeState(IdleString);
			}
			else
			{
				((HL)owner).WarpToHat();

				owner.CommandHadouken(HatString, HadoukenPart.ProjectileCommand.DeleteHat);

				owner.grounded = false;
			}
		}
		owner.CheckTurnAround();
		if (frameCount == teleFrame + 1)
		{
			owner.ChangeState(FallString);
		}
		base.FrameAdvance();
	}

	public override bool DelayInputs()
	{
		return true;
	}
}
