using Godot;
using System;

public class HLDP : DP
{
	[Export]
	public int throwFrame = 20;

	private const string TeleportDPString = "TeleportDP";
	private const string AirHatString = "AirHat";

	public override void Enter()
	{
		base.Enter();
		if (!((HL)owner).hatted)
		{
			owner.ChangeState(TeleportDPString);
		}
	}
	public override void FrameAdvance()
	{
		base.FrameAdvance();
		if (frameCount == throwFrame && ((HL)owner).hatted && (owner.CheckHeldKey('a')|| owner.CheckHeldKey('s')))
			owner.ChangeState(AirHatString);
	}
}
