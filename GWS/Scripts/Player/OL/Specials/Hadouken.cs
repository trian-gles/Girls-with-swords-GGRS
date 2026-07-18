using Godot;
using System;
using System.Collections.Generic;

public class Hadouken : BaseAttack
{
	[Export]
	public int releaseFrame = 18;

	[Export]
	public Vector2 launch = new Vector2(0, 0);

	[Export]
	public int launchFrame = 1;

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

	protected List<HadoukenPart> cachedHadoukens;

	public override void _Ready()
	{
		base._Ready();
		tags.Add(Globals.Tags.special);
		cachedHadoukens = new List<HadoukenPart>();
		for (int i = 0; i < 24; i++)
		{
			var h = hadoukenScene.Instance() as HadoukenPart;
			cachedHadoukens.Add(h);
		}

		if (launch != Vector2.Zero)
		{
			slowdownSpeed = 0;
		}
	}	

	public override void Enter()
	{
		base.Enter();
		
		if (launch != Vector2.Zero)
		{
			owner.landingRecoveryFramesRemaining = 15;
			owner.velocity.y = 0;
		}
		owner.velocity.x = 0;
	}

	public override void Reset()
	{
		foreach (var h in cachedHadoukens)
		{
			owner.DeleteHadouken(h);
			h.active = false;
		}
	}
	public override void FrameAdvance()
	{
		base.FrameAdvance();

		if (launch != Vector2.Zero && frameCount == launchFrame)
		{
			owner.velocity = launch;
			if (!owner.facingRight)
			{
				owner.velocity.x *= -1;
			}
			owner.grounded = false;
		}

		if (!owner.grounded && frameCount > launchFrame)
		{
			ApplyGravity();
		}

		if (owner.grounded && launch != Vector2.Zero)
		{
			owner.velocity.x = 0;
			if (owner.landingRecoveryFramesRemaining > 0)
				owner.ChangeState("LandingRecovery");
			else
				owner.ChangeState("Landing");
		}

		//if (frameCount == releaseFrame - 6)
			//owner.ScheduleEvent(EventScheduler.EventType.AUDIO, hadoukenSound, Name);
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
				if (owner.facingRight)
					h.Position = new Vector2(xPos + xOffset, yPos + yOffset);
				else
					h.Position = new Vector2(xPos - xOffset, yPos + yOffset);
				if (Globals.logOn)
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
