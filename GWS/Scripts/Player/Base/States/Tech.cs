using Godot;
using System;
using System.Linq;

public class Tech : Fall
{
	[Export]
	public Vector2 techVector = new Vector2(0, 0);

	public int length = 15;
	public override void Enter()
	{
		base.Enter();
		owner.GFXEvent("Tech");
		owner.ResetComboAndProration();
		owner.canDoubleJump = true;
		owner.CheckTurnAround();
		owner.invulnFrames = length;

		if (owner.CheckHeldKey('6'))
			owner.velocity = techVector;
		else if (owner.CheckHeldKey('4'))
			owner.velocity = new Vector2(-techVector.x, techVector.y);
		else
			owner.velocity = new Vector2(0, techVector.y);

		if (owner.grounded)
		{
			EmitSignal(nameof(StateFinished), "Knockdown");
		}
	}

    public override void HandleInput(char[] inputArr)
	{
		if (frameCount == 0 && !owner.grounded)
		{
			if (inputArr.SequenceEqual(new char[] { '6', 'p' }))
			{
				owner.velocity = techVector;
			}
				
			else if (inputArr.SequenceEqual(new char[] { '4', 'p' }))
				owner.velocity = new Vector2(-techVector.x, techVector.y);
				
		}
		
		base.HandleInput(inputArr);
	}

	public override bool CollisionActive()
    {
		return false;
    }
}
