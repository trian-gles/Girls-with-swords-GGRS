using Godot;
using System;

public class Hadouken : BaseAttack
{
	[Export]
	public int releaseFrame = 18;

	[Export]
	public PackedScene hadoukenScene;

	/// <summary>
	/// How far below the player the projecctile will be
	/// </summary>
	[Export]
	public int yOffset = 5;


	[Export]
	public int xOffset = 0;
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
		if (frameCount == releaseFrame)
		{
			owner.ScheduleEvent(EventScheduler.EventType.AUDIO);
			EmitHadouken();
		}
	}

	/// <summary>
	/// Note that the overriden SnailStrike discards this parent code.
	/// </summary>
	protected virtual void EmitHadouken()
	{
		var h = hadoukenScene.Instance() as HadoukenPart;

		h.Spawn(owner.facingRight, owner.otherPlayer);
		owner.EmitHadouken(h);
		h.Position = new Vector2(owner.Position.x + xOffset, owner.Position.y + yOffset);
		Globals.Log($"Emitting snail at x position {h.Position}, our position = {owner.Position}, animation frame = {frameCount}, vel = {owner.velocity}");

	}

}
