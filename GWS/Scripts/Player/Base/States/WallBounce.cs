using Godot;
using System;
using System.Collections.Generic;

public class WallBounce : Float
{
	private bool bounced = false;

	public override void _Ready()
	{
		base._Ready();
		animationName = "Float";
	}

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
			GD.Print("Lower than min vel");
		}
		
		GD.Print($"Wallbounce with vel {owner.velocity.x}");
		//if (owner.CheckTouchingWall())
		//{
		//	owner.velocity.x *= -1;
		//}
		
			
	}
	public override void Load(Dictionary<string, int> loadData)
	{
		bounced = Convert.ToBoolean(loadData["bounced"]);
	}

	public override Dictionary<string, int> Save()
	{
		var dict = new Dictionary<string, int>();
		dict["bounced"] = Convert.ToInt32(bounced);
		return dict;
	}

	public override void HitWall()
	{
		if (!bounced)
		{
			bounced = true;
			owner.velocity.x *= -1;
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

		if (owner.velocity.x > 0)
		{
			distanceToTravel = (Globals.rightWall - (int)owner.internalPos.x) * 2;
		}
		else
		{
			distanceToTravel = (Globals.leftWall - (int)owner.internalPos.x) * 2;
		}

		return (int)Mathf.Floor(distanceToTravel / CalcFramesToGround());
	}

}

