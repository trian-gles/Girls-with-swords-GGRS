using Godot;
using System;
using System.Collections.Generic;

public class Grab : State
{
	public override HashSet<string> tags { get; set; } = new HashSet<string>() {"grab" };

	[Export]
	public int level = 0;

	protected Globals.AttackDetails hitDetails;
	protected Globals.AttackDetails chDetails;

	[Export]
	public int releaseFrame = 10;

	[Export]
	public Vector2 launch = new Vector2();

	[Export]
	public int slowTerminalVelocity = 0;

	[Export]
	public int dmg = 0;

	[Export]
	public int hitStun = 0;

	[Export]
	public int prorationLevel = 2;

	[Export]
	public BaseAttack.GRAPHICEFFECT effect;

	[Export]
	public bool setShrinkOtherSpriteTrue = false;

	public bool released = false;

	public bool rightGrab = true;

	private int RELEASEINDEX = 0;
	
	public override void _Ready()
	{
		base._Ready();
		AddCancel("Idle");

		isCounter = true;
		hitDetails = Globals.attackLevels[level].hit;
		chDetails = Globals.attackLevels[level].counterHit;
		
		hitDetails.prorationLevel = prorationLevel;
		hitDetails.opponentLaunch = launch;
		hitDetails.hitStun = hitStun;
		chDetails.hitStun = hitStun;
		hitDetails.graphicFX = effect;
		chDetails.graphicFX = effect;

		shrinkOtherSprite = setShrinkOtherSpriteTrue;
	}
	
	public override void Load(int[] loadData)
	{
		released = Convert.ToBoolean(loadData[RELEASEINDEX]);
	}

	public override int[] Save()
	{
		stateStateArray[RELEASEINDEX] = Convert.ToInt32(released);
		return stateStateArray;
	}

	public override void Enter()
	{
		owner.ZIndex = 1;
		base.Enter();
		owner.velocity = Vector2.Zero;

		if (owner.facingRight && owner.internalPos.x + 4000 > Globals.rightWall)
			owner.internalPos.x = Globals.rightWall - 4000;

		if (!owner.facingRight && owner.internalPos.x - 4000 < Globals.leftWall)
			owner.internalPos.x = Globals.leftWall + 4000;


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
			if (slowTerminalVelocity != 0)
			{
				owner.otherPlayer.terminalVelocity = slowTerminalVelocity;
			}
			//if (!rightGrab)
			//{
			//	actualLaunch.x *= -1;
			//}
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
			if (effect == BaseAttack.GRAPHICEFFECT.PURPLE)
			{
				owner.otherPlayer.GFXEvent("Purple");

			}
			
			
			released = true;
			
		}
	}
	public override void Exit()
	{
		base.Exit();
		owner.ZIndex = 0;
	}

	public override void AnimationFinished()
	{
		owner.ChangeState("Idle");
	}
}
