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

	protected List<HadoukenPart> cachedHadoukens;

	public override void _Ready()
	{
		base._Ready();
		cachedHadoukens = new List<HadoukenPart>();
		for (int i = 0; i < 10; i++)
		{
			var h = hadoukenScene.Instance() as HadoukenPart; // freed in exittree
			cachedHadoukens.Add(h);
			h.Connect("OnHitConnected", owner, nameof(owner.OnHitConnected));
		}
	}

	public override void InHurtbox(Vector2 collisionPnt)
	{
		base.InHurtbox(collisionPnt);
		HadoukenPart h = null;
		foreach (HadoukenPart cachedPart in cachedHadoukens)
		{
			if (cachedPart.freed)
			{
				h = cachedPart;
			}
		}
		h.active = true;
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

	public override void _ExitTree()
    {
        base._ExitTree();
		foreach (HadoukenPart cachedPart in cachedHadoukens)
		{
			cachedPart.QueueFree();
		}
    }

}
