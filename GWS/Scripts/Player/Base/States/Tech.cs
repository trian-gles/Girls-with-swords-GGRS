using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

public class Tech : Fall
{
	[Export]
	public Vector2 techVector = new Vector2(0, 0);

	public override HashSet<string> tags { get; set; } = new HashSet<string>() {"tech" };

	public int length = 15;
	public override void Enter()
	{
		base.Enter();
		owner.wasOTGHit = false;
		owner.GFXEvent("Tech");
		owner.EmitSignal(nameof(Player.GenericGFX), "Ukemi", owner.Name);
		owner.ResetComboAndProration();
		owner.canDoubleJump = true;
		owner.hasDoubleOrSuperJumped = false;
		owner.CheckTurnAround();
		owner.invulnFrames = length;
		ResetTerminalVelocity();

		if (owner.CheckHeldKey('6'))
			owner.velocity = techVector;
		else if (owner.CheckHeldKey('4'))
		{
			owner.velocity.x = -techVector.x;
			owner.velocity.y = techVector.y;
		}
		else
			owner.velocity.x = 0;

		owner.grounded = false;
	}

    public override void HandleInput(char[] inputArr)
	{
		if (frameCount == 0 && !owner.grounded)
		{
			if (inputArr.SequenceEqual(Globals.RIGHTPRESS))
			{
				owner.velocity = techVector;
			}
			else if (inputArr.SequenceEqual(Globals.LEFTPRESS))
			{
				owner.velocity.x = -techVector.x;
				owner.velocity.y = techVector.y;
			}
				
		}
		
		base.HandleInput(inputArr);
	}

	public override bool CollisionActive()
    {
		return false;
    }
}
