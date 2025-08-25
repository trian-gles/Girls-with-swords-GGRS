using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public class HatSlash : Hadouken
{

	public override string animationName { get { return "Hadouken"; } } // Required as we reuse both this script AND animation
	protected override HadoukenPart EmitHadouken()
	{
        var h = base.EmitHadouken();
        h.Position = new Vector2(((HL)owner).hatCoors) + new Vector2(0, 15);
        return h;
	}
}
