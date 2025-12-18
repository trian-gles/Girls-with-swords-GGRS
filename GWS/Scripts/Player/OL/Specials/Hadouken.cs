using Godot;
using System;

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

	public override void _Ready()
	{
		base._Ready();
		
		var h = hadoukenScene.Instance() as HadoukenPart;
		h.QueueFree();
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
		if (mustCooldown && owner.hadoukenCooldownRemaining > 0)
			return null;


		var h = hadoukenScene.Instance() as HadoukenPart;

		h.Spawn(owner.facingRight, owner.otherPlayer);
		owner.EmitHadouken(h);

		int xPos = (int)Mathf.Floor(owner.internalPos.x / 100);
		int yPos = (int)Mathf.Floor(owner.internalPos.y / 100);
		h.Position = new Vector2(xPos + xOffset, yPos + yOffset);
		h.Connect("OnHitConnected", owner, nameof(owner.OnHitConnected));
		Globals.Log($"Emitting hadouken at position {h.Position}, our position = {owner.Position}, our frameCount = {frameCount}");
		if (mustCooldown ) 
			owner.hadoukenCooldownRemaining = 55;
		return h;
		

	}

}
