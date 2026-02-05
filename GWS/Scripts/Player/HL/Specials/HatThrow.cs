using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public class HatThrow : Hadouken
{
	private const string HadoukenAnimString = "Hadouken";
	[Export]
	public string negEdgeButton = "p";

	[Export]
	public Vector2 targetPos = Vector2.Zero;

	[Export]
	public int landingRecovery = 5;

	[Export]
	public string noHatState = "Teleport";

	public override string animationName { get { return HadoukenAnimString; } } // Required as we reuse both this script AND animation

	public override void Enter()
	{
		base.Enter();
		if (!((HL)owner).hatted)
		{
			owner.ChangeState(noHatState);
		}
			
		
		owner.landingRecoveryFramesRemaining = landingRecovery;
	}
	protected override HadoukenPart EmitHadouken()
	{
		if (((HL)owner).hatted)
		{
			var h = (HatPart) base.EmitHadouken();
			((HL)owner).hatted = false;
			Vector2 transform = new Vector2(targetPos);
			if (!owner.facingRight)
				transform.x *= -1;
			
			h.targetPos = owner.Position + transform;
			return h;
				
		}
		return null;
	}
}
