using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;

class SnailStrike : Hadouken
{

	private const string SnailStrikeAudioString = "snail-strike";

	[Export]
	public int xOffset = 0;

	[Export]
	public int successiveXOffset = 20;

	[Export]
	public int gapBetweenStrikes = 20;

	public override void FrameAdvance()
	{
		base.FrameAdvance();
		if (frameCount > releaseFrame)
		{
			int strikeCycle = (frameCount - releaseFrame) % gapBetweenStrikes;
			if (strikeCycle == 0)
			{
				EmitHadouken();
			}
		}
	}

	protected override HadoukenPart EmitHadouken()
	{
		owner.ScheduleEvent(EventScheduler.EventType.AUDIO, SnailStrikeAudioString, Name);
		int strikeNum = (frameCount - releaseFrame) / gapBetweenStrikes;
		HadoukenPart h = null;
		foreach (HadoukenPart cachedPart in cachedHadoukens)
		{
			if (cachedPart.freed)
			{
				h = cachedPart;
			}
		}
		h.active = true;
		h.Spawn(owner.facingRight, owner.otherPlayer);
		owner.EmitHadouken(h);

		int displacement = strikeNum * successiveXOffset + xOffset;
		if (!owner.facingRight)
		{
			displacement *= -1;
		}
			
		h.Position = new Vector2(owner.Position.x  + displacement, owner.Position.y + yOffset);
		return h;
	}


}

