using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;

class SnailGrab : AirGrab
{
	[Export]
	Godot.Collections.Array<Vector2> directions;

	[Export]
	Godot.Collections.Array<int> directionFrames;

	public override void FrameAdvance()
	{
		base.FrameAdvance();
		for (int i = directionFrames.Count - 1; i >= 0; i--)
		{
			if (frameCount >= directionFrames[i])
			{
				owner.velocity = directions[i];
			}
		}

	}
}
