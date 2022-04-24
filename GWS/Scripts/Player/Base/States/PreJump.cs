using Godot;
using System;
using System.Collections.Generic;

public class PreJump : State
{
	[Export]
	public int len = 2;
	public override void _Ready()
	{
		base._Ready();
		animationName = "None";
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
			EmitSignal(nameof(StateFinished), "Jump");
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
}


