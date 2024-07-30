using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public class HatThrow : Hadouken
{
	[Export]
	public string negEdgeButton = "p";

	public override string animationName { get { return "Hadouken"; } } // Required as we reuse both this script AND animation

	public override void Enter()
	{
		base.Enter();
		((HL)owner).hatKey = negEdgeButton[0];
	}
	protected override void EmitHadouken()
	{
		if (((HL)owner).hatted)
		{
			base.EmitHadouken();
			((HL)owner).hatted = false;
		}
	}
}
