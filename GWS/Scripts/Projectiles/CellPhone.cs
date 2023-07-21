using Godot;
using System;
using System.Collections.Generic;
class CellPhone : HadoukenPart
{
	[Export]
	public Vector2 launch;

	[Export]
	public int gravity;


	public override void Spawn(bool movingRight, Player targetPlayer)
	{
		base.Spawn(movingRight, targetPlayer);
		speed = new Vector2(launch);
	}
	public override void FrameAdvance()
	{
		if (frame > 0 && (hits == 0) && (frame % 2 == 0)) {
			speed.y += gravity;
		}
		base.FrameAdvance();
	}

	protected override Dictionary<string, int> GetStateSpecific()
	{
		return new Dictionary<string, int>() { { "yVel", (int)speed.y } };
	}

	protected override void SetStateSpecific(Dictionary<string, int> dict)
	{
		speed.y = dict["yVel"];
	}


}
