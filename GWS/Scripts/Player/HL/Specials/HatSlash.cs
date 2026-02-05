using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public class HatSlash : Hadouken
{
	private const string SlashWhiffString = "SlashWhiff";
	private const string TeleportString = "Teleport";
	protected override HadoukenPart EmitHadouken()
	{
		owner.ScheduleEvent(EventScheduler.EventType.AUDIO, SlashWhiffString, Name);
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
			owner.ChangeState(TeleportString);
		else
			base.AnimationFinished();
	}
}
