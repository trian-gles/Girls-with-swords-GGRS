using Godot;
using System;
using System.Collections.Generic;

public class MainGFX : Node
{
	private int lastLevelUp = 0;
	private List<Sprite> ghosts = new List<Sprite>();
	private PackedScene dashGhost = (PackedScene) ResourceLoader.Load("res://Scenes/DashGhost.tscn");
	private Dictionary<string, PackedScene> particleSprites = new Dictionary<string, PackedScene>();
	public override void _Ready()
	{
		GetNode("/root/Globals").Connect("GhostEmitted", this, nameof(OnGhostEmitted));
		GetNode("/root/Globals").Connect("PlayerFXEmitted", this, nameof(OnGFXParticleEmitted));
		particleSprites.Add("hit", (PackedScene)ResourceLoader.Load("res://Scenes/Particles/HitFX.tscn"));
		particleSprites.Add("block", (PackedScene)ResourceLoader.Load("res://Scenes/Particles/BlockFX.tscn"));

		var dummy = dashGhost.Instance();
		dummy.QueueFree();

		foreach (var sprite in particleSprites.Values)
		{
			var s = sprite.Instance();
			s.QueueFree();
		}

	}

	public void LevelUp(int frame)
	{
		GetNode<Node2D>("Stages").Call("level_up");
		lastLevelUp = frame;
	}

	public void OnGFXParticleEmitted(Vector2 location, string particleName)
	{
		location /= 100;
		GD.Print($"Emitting {particleName} at {location}");
		var newPart = (ParticleSprite) particleSprites[particleName].Instance();
		AddChild(newPart);
		MoveChild(newPart, 0);
		newPart.Position = location;
	}

	public void OnGhostEmitted(Player p)
	{
		GD.Print("maingfx creating ghost");
		var newGhost = (Sprite) dashGhost.Instance();
		AddChild(newGhost);
		MoveChild(newGhost, 0);
		ghosts.Add(newGhost);
		newGhost.GlobalPosition = p.sprite.GlobalPosition;
		newGhost.Texture = p.sprite.Texture;
		newGhost.Vframes = p.sprite.Vframes;
		newGhost.Hframes = p.sprite.Hframes;
		newGhost.Frame = p.sprite.Frame;
		newGhost.Scale = p.sprite.Scale;
		newGhost.FlipH = p.sprite.FlipH;
	}

	public void Rollback(int frame)
	{
		if (frame < lastLevelUp)
		{
			GetNode<Node2D>("Stages").Call("rollback");
		}
	}
}
