using Godot;
using System;
using System.Collections.Generic;

public class CommandRunTurn : CommandRunBase
{
    [Export]
    public int turnFrame = 10;

	public override void FrameAdvance()
	{
		base.FrameAdvance();
		if (frameCount == turnFrame)
		{
			
			if (owner.facingRight)
			{
				owner.TurnLeft();
				owner.velocity.x = - speed;
			}
			else
			{
				owner.TurnRight();
				owner.velocity.x = speed;
			}
		}
	}

    public override bool CollisionActive()
    {
		return frameCount > turnFrame;
    }
}
