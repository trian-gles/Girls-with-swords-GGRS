using Godot;
using System;
using System.Collections.Generic;

public class PreJump : State
{
	[Export]
	public int len = 3;

	public override string animationName { get { return "None"; } }
	public override void _Ready()
	{
		base._Ready();
		stop = false;
		AddEasyGroundSpecials();
	}
    //public override bool DelayInputs()
    //{
    //	return true;
    //}
	public override void FrameAdvance()
	{
		base.FrameAdvance();
		//Globals.Log($"prejump frameAdvance with position {owner.internalPos}, velocity " + owner.velocity.ToString());
		if (frameCount == len)
		{
			if (owner.CanSuperJump())
			{
				owner.hasDoubleOrSuperJumped = true;
				owner.ChangeState("SuperJump");
			}
			else
			{
				owner.ChangeState("Jump");
			}
		}
			
	}

	public override void HandleInput(char[] inputArr)
	{
		
		base.HandleInput(inputArr);
		if (Globals.CompareInput(inputArr, Globals.RIGHTPRESS))
		{
			owner.velocity.x = owner.speed;
		}
		else if (Globals.CompareInput(inputArr, Globals.LEFTPRESS))
		{
			owner.velocity.x = -owner.speed;
		}
			
	}

	public override void ReceiveHit(Globals.AttackDetails details)
	{
		ReceiveHitNoBlock(details);
	}
}


