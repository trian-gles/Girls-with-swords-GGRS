using Godot;
using System;
using System.Collections.Generic;

public class CommandRun : State
{
	[Export]
	public int minLen = 10;

	[Export]
	public int maxLen = 80;

	[Export]
	public int speed = 450;

	private bool oneHit = false;

	new protected bool isCounter = true;

	public override void _Ready()
	{
		base._Ready();
		loop = true;
		
	}


	public override void Load(Dictionary<string, int> loadData)
	{
		oneHit = Convert.ToBoolean(loadData["oneHit"]);
	}

	public override Dictionary<string, int> Save()
	{
		var dict = new Dictionary<string, int>();
		dict["oneHit"] = Convert.ToInt32(oneHit);
		return dict;

	}
	public override void Enter()
	{
		base.Enter();
		oneHit = false;
		if (owner.facingRight)
		{
			owner.velocity.x = speed;
		}
		else
		{
			owner.velocity.x = -speed;
		}
	}


	public override void FrameAdvance()
	{
		base.FrameAdvance();
		if (frameCount > maxLen)
		{
			EmitSignal(nameof(StateFinished), "Hojogiri");
		}
		
	}

	public override void HandleInput(char[] inputArr)
	{
		if (Globals.CheckKeyPress(inputArr, 's'))
		{
			if (frameCount > minLen)
			{
				EmitSignal(nameof(StateFinished), "Hojogiri");
			}
		}
	}

    public override void ReceiveHit(Globals.AttackDetails details)
	{
		if (!oneHit)
		{
			owner.ScheduleEvent(EventScheduler.EventType.AUDIO, "HitStun", Name);
			GetNode<Node>("/root/Globals").EmitSignal(nameof(PlayerFXEmitted), details.collisionPnt, "hit", false);
			owner.GFXEvent("Blood");
			oneHit = true;
		}
		else
		{
			base.ReceiveHit(details);
		}
	}

	public override void receiveStun(int hitStun, int blockStun)
	{
		if (oneHit)
		{
			base.receiveStun(hitStun, blockStun);
		}
	}

	/// <summary>
	/// Proration level is ignored here
	/// </summary>
	/// <param name="dmg"></param>
	/// <param name="prorationLevel"></param>
	public override void receiveDamage(int dmg, int prorationLevel)
	{
		owner.DeductHealth(dmg * owner.proration);
	}
}
