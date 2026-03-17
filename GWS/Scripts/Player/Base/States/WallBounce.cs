using Godot;
using System;
using System.Collections.Generic;

public class WallBounce : AirKnockdown
{
	private bool bounced = false;

	private const string WallBounceString = "WallBounce";
	private const string FloatAnimString = "Float";

	public override string animationName { get { return FloatAnimString; } }

	private int BOUNCEINDEX = 0;


	public override void Enter()
	{
		base.Enter();
		bounced = false;
		owner.velocity.x = CalcXVel();
		if (Math.Abs(owner.velocity.x) < 200)
		{
			int sign = Math.Sign(owner.velocity.x);
			owner.velocity.x = 200;
			if (sign != 0)
				owner.velocity.x *= sign;
		}
	}
	public override void Load(int[] loadData)
	{
		bounced = Convert.ToBoolean(loadData[BOUNCEINDEX]);
	}

	public override int[] Save()
	{
		stateStateArray[BOUNCEINDEX] = Convert.ToInt32(bounced);
		return stateStateArray;
	}

	public override void HitWall()
	{
		if (!bounced)
		{
			bounced = true;
			owner.velocity.x *= -1;
			owner.GFXEvent(WallBounceString);
		}
	}

	private int CalcFramesToGround()
	{ 
		int frame = 0;
		int y = (int)owner.internalPos.y;
		int yVel = (int)owner.velocity.y;
		do
		{
			yVel += owner.gravity;
			y += yVel;
			frame++;
		}
		while (y < Globals.floor);
		return frame;
	}

	private int CalcXVel()
	{
		int distanceToTravel;
		int wallPos;


		if (owner.velocity.x > 0)
		{
			wallPos = (int)Math.Min(Globals.rightWall, owner.otherPlayer.internalPos.x + Player.MAXPLAYERDIST);
			distanceToTravel = (wallPos - (int)owner.internalPos.x) * 2;
		}
		else
		{
			wallPos = (int)Math.Max(Globals.leftWall, owner.otherPlayer.internalPos.x - Player.MAXPLAYERDIST);
			distanceToTravel = (wallPos - (int)owner.internalPos.x) * 2;
		}

		return (int)Mathf.Floor(distanceToTravel / CalcFramesToGround());
	}

}

