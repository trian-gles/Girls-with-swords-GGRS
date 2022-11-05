using Godot;
using System;
using System.Collections.Generic;

public class Grab : State
{
	[Export]
	protected int level = 0;

	protected Globals.AttackDetails hitDetails;
	protected Globals.AttackDetails chDetails;

	[Export]
	public int releaseFrame = 10;

	[Export]
	public Vector2 launch = new Vector2();

	[Export]
	public int dmg = 0;

	[Export]
	public int hitStun = 0;

	[Export]
	public int prorationLevel = 2;

	public bool released = false;

	public bool rightGrab = true;

    public override void _Ready()
    {
        base._Ready();
		AddCancel("Idle");

		isCounter = true;
		hitDetails = Globals.attackLevels[level].hit;
		chDetails = Globals.attackLevels[level].counterHit;

		hitDetails.opponentLaunch = launch;
		hitDetails.hitStun = hitStun;
		chDetails.hitStun = hitStun;
	}

	public override void Load(Dictionary<string, int> loadData)
	{
		released = Convert.ToBoolean(loadData["released"]);
	}

	public override Dictionary<string, int> Save()
	{
		var dict = new Dictionary<string, int>();
		dict["released"] = Convert.ToInt32(released);
		return dict;

	}

	public override void Enter()
	{
		base.Enter();
		owner.velocity = new Vector2(0, 0);
		released = false;
		owner.otherPlayer.ChangeState("Grabbed");

		rightGrab = owner.facingRight;
	}

	public override void HandleInput(char[] inputArr)
	{
		if (frameCount < releaseFrame)
		{
			return;
		}
		base.HandleInput(inputArr);
	}

	public override void FrameAdvance()
	{
		base.FrameAdvance();
		if (frameCount < releaseFrame)
		{
			if (frameCount < 3)
            {
				if (owner.CheckHeldKey('6'))
				{
					owner.TurnRight();
					rightGrab = true;
				}
				if (owner.CheckHeldKey('4'))
				{
					owner.TurnLeft();
					rightGrab = false;
				}
			}

			Vector2 relGrabPosition = owner.grabPos.Position * 100;
			if (!rightGrab)
			{
				relGrabPosition.x *= -1;
			}

			Vector2 absGrabPosition = relGrabPosition + owner.internalPos;

			owner.otherPlayer.internalPos =  absGrabPosition;
		}
		
		else if ((frameCount == releaseFrame) && !released)
		{
			Vector2 actualLaunch = launch;
			//if (!rightGrab)
			//{
			//	actualLaunch.x *= -1;
			//}
			GD.Print("Grab hitting other player");
			var direction = BaseAttack.ATTACKDIR.EQUAL;

			if (owner.OtherPlayerOnRight())
			{
				direction = BaseAttack.ATTACKDIR.RIGHT;
			}
			else if (owner.OtherPlayerOnLeft())
			{
				direction = BaseAttack.ATTACKDIR.LEFT;
			}

			hitDetails.dir = direction;
			chDetails.dir = direction;
			hitDetails.opponentLaunch = actualLaunch;
			chDetails.opponentLaunch = actualLaunch;

			owner.otherPlayer.ReceiveHit(hitDetails, chDetails);
			released = true;
			
		}
	}
	public override void AnimationFinished()
	{
		EmitSignal(nameof(StateFinished), "Idle");
	}
}
