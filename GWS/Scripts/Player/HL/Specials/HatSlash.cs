using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public class HatSlash : Hadouken
{
	protected override HadoukenPart EmitHadouken()
	{
		owner.ScheduleEvent(EventScheduler.EventType.AUDIO, "SlashWhiff", Name);
		var h = base.EmitHadouken();
		h.Position = new Vector2(((HL)owner).hatCoors) + new Vector2(0, 15);
		return h;
	}

	public override bool DelayInputs()
	{
		return frameCount > 20;
	}

	public override void AnimationFinished()
	{
		if (owner.CheckHeldKey('a'))
			owner.ChangeState("Teleport");
		else
			base.AnimationFinished();
	}
}
