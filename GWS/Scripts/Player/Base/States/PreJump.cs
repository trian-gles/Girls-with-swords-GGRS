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
	}
	//public override bool DelayInputs()
	//{
	//	return true;
	//}

	public override void FrameAdvance()
	{
		base.FrameAdvance();
		if (frameCount == len)
		{
			if (owner.CheckBuffer(new[] { '2', 'p' }))
			{
				EmitSignal(nameof(StateFinished), "SuperJump");
			}
			else
			{
				EmitSignal(nameof(StateFinished), "Jump");
			}
		}
			
	}

	public override void HandleInput(char[] inputArr)
	{
		//GD.Print(inputArr);
		base.HandleInput(inputArr);
		
		if (inputArr == new char[] {'6', 'p'})
		{
			owner.velocity.x = owner.speed;
			//GD.Print("6p during prejump");
		}
			
		else if (inputArr == new char[] { '4', 'p' })
			owner.velocity.x = -owner.speed;
	}

	public override void ReceiveHit(Globals.AttackDetails details)
	{
		ReceiveHitNoBlock(details);
	}
}


