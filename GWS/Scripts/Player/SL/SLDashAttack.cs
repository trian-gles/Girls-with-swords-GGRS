using System;
using System.Collections.Generic;
using Godot;

public class SLDashAttack : MovingAttack
{
	[Export]
	public PackedScene hadoukenScene;
	
	/// <summary>
	/// How far below the player the projecctile will be
	/// </summary>
	//[Export]
	public int yOffset = 5;


	//[Export]
	public int xOffset = 0;

	public override void _Ready()
	{
		base._Ready();

		var h = hadoukenScene.Instance() as HadoukenPart;
		h.QueueFree();
		// this looks silly but is necessary so that the hadouken loads at game start
	}

	public override void InHurtbox(Vector2 collisionPnt)
	{
		base.InHurtbox(collisionPnt);
		var h = hadoukenScene.Instance() as HadoukenPart;

		h.Spawn(owner.facingRight, owner.otherPlayer);
		owner.EmitHadouken(h);
		int xPos = (int)Mathf.Floor(owner.internalPos.x / 100);
		int yPos = (int)Mathf.Floor(owner.internalPos.y / 100);
		h.Position = new Vector2(xPos + xOffset, yPos + yOffset);
		Globals.Log($"Emitting hadouken at position {h.Position}, our position = {owner.Position}, our frameCount = {frameCount}");
	}

	public override bool CollisionActive()
	{
		return false;
	}

}
