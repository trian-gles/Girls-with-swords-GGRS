using Godot;
using System;
using System.Collections.Generic;

public class PreJump : State
{
	[Export]
	public int len = 3;
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

	public override void ReceiveHit(Globals.AttackDetails details) // no blocking during prejump
	{
		//GD.Print($"Received attack on side {rightAttack}");
		bool launchBool = false;
		switch (details.dir)
		{
			case BaseAttack.ATTACKDIR.RIGHT:
				break;
			case BaseAttack.ATTACKDIR.LEFT:
				details.opponentLaunch.x *= -1;
				details.hitPush *= -1;
				break;
			case BaseAttack.ATTACKDIR.EQUAL:
				details.opponentLaunch.x = 0;
				details.hitPush = 0;
				break;
		}
		owner.hitPushRemaining = details.hitPush;
		//GD.Print($"Setting hitpush in hitstun to {owner.hitPushRemaining}");
		owner.velocity = details.opponentLaunch;
		if (!(details.opponentLaunch == Vector2.Zero))
		{
			owner.velocity = details.opponentLaunch;
			launchBool = true;
		}

		if (owner.velocity.y < 0) // make sure the player is registered as in the air if launched 
		{
			owner.grounded = false;
		}


		EnterHitState(details.knockdown, details.opponentLaunch, details.collisionPnt, details.effect);

	}
}


