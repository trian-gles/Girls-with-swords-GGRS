using Godot;
using System;
using System.Collections.Generic;

public class Hadouken : BaseAttack
{
	[Export]
	public int releaseFrame = 18;

	[Export]
	public PackedScene hadoukenScene;

	[Export]
	public string hadoukenSound = "Hadouken";

	/// <summary>
	/// How far below the player the projecctile will be
	/// </summary>
	[Export]
	public int yOffset = 5;


	[Export]
	public int xOffset = 0;

	[Export]
	public bool mustCooldown = false;

	private List<HadoukenPart> cachedHadoukens;

	public override void _Ready()
	{
		base._Ready();
		cachedHadoukens = new List<HadoukenPart>();
		for (int i = 0; i < 10; i++)
		{
			var h = hadoukenScene.Instance() as HadoukenPart;
			cachedHadoukens.Add(h);
			h.Connect("OnHitConnected", owner, nameof(owner.OnHitConnected));
		}
		// this looks silly but is necessary so that the hadouken loads at game start
	}	

	public override void Enter()
	{
		base.Enter();
		owner.velocity.x = 0;
	}
	public override void FrameAdvance()
	{
		base.FrameAdvance();

		if (frameCount == releaseFrame - 6)
			owner.ScheduleEvent(EventScheduler.EventType.AUDIO, hadoukenSound, Name);
		if (frameCount == releaseFrame)
			EmitHadouken();
	}

	/// <summary>
	/// Note that the overriden SnailStrike discards this parent code.
	/// </summary>
	protected virtual HadoukenPart EmitHadouken()
	{
		HadoukenPart h;

		foreach (HadoukenPart cachedPart in cachedHadoukens)
		{
			if (cachedPart.freed)
			{
				h = cachedPart;
				h.active = true;
				h.Spawn(owner.facingRight, owner.otherPlayer);
				owner.EmitHadouken(h);

				int xPos = (int)Mathf.Floor(owner.internalPos.x / 100);
				int yPos = (int)Mathf.Floor(owner.internalPos.y / 100);
				h.Position = new Vector2(xPos + xOffset, yPos + yOffset);
				
				Globals.Log($"Emitting hadouken at position {h.Position}, our position = {owner.Position}, our frameCount = {frameCount}");
				if (mustCooldown ) 
					owner.hadoukenCooldownRemaining = 55;
				return h;
			}
				
		}
		GD.Print("BAD, OUT OF HADOUKENS");
		return null;

		
		

	}

}
