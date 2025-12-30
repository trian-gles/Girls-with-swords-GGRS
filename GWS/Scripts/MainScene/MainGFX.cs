using Godot;
using System;
using System.Collections.Generic;

public class MainGFX : Node
{
	private int lastLevelUp = -100;
	private List<Sprite> ghosts = new List<Sprite>();
	private PackedScene dashGhost = (PackedScene) ResourceLoader.Load("res://Scenes/DashGhost.tscn");
	private Dictionary<string, PackedScene> particleSprites = new Dictionary<string, PackedScene>();
	public override void _Ready()
	{
		GetNode("/root/Globals").Connect("GhostEmitted", this, nameof(OnGhostEmitted));
		GetNode("/root/Globals").Connect("PlayerFXEmitted", this, nameof(OnGFXParticleEmitted));

// store referencecs to all particles
		particleSprites.Add("hit", (PackedScene)ResourceLoader.Load("res://Scenes/Particles/HitFX.tscn"));
		particleSprites.Add("block", (PackedScene)ResourceLoader.Load("res://Scenes/Particles/BlockFX.tscn"));
		particleSprites.Add("shield", (PackedScene)ResourceLoader.Load("res://Scenes/Particles/ShieldFX.tscn"));
		particleSprites.Add("dust", (PackedScene)ResourceLoader.Load("res://Scenes/Particles/DustFX.tscn"));
		particleSprites.Add("burst", (PackedScene)ResourceLoader.Load("res://Scenes/Particles/Burst.tscn"));
		particleSprites.Add("coffee", (PackedScene)ResourceLoader.Load("res://Scenes/Particles/CoffeeExplosion.tscn"));

// render all particles NOW since C# has no preload
		var dummy = dashGhost.Instance();
		dummy.QueueFree();

		foreach (var sprite in particleSprites.Keys)
		{
			for (int i = 0; i < 4; i++)
				ReleaseNewParticle(new Vector2(0, 0), sprite, true);
		}

	}
	
	public void Init(int background){
		GetNode("Stages").Call("set_bkg", background);
	}

	public void LevelUp(int frame)
	{
		GetNode<Node2D>("Stages").Call("level_up");
		lastLevelUp = frame;
	}

	public void OnGFXParticleEmitted(Vector2 location, string particleName, bool flipH)
	{
		location /= 100;
		foreach (var child in GetChildren())
		{
			var partSprite = child as ParticleSprite;
			if (partSprite != null && partSprite.type == particleName && !partSprite.Visible) // try to reassign the particle to save on GC
			{
				partSprite.Reassign();
				partSprite.initFrame = Globals.frame;
				partSprite.FlipH = flipH;
				partSprite.Position = location;

				return;
			}

		}

		ReleaseNewParticle(location, particleName, flipH);
	}

	private void ReleaseNewParticle(Vector2 location, string particleName, bool flipH)
	{
		GD.Print("new particle");
		var newPart = (ParticleSprite)particleSprites[particleName].Instance();
		newPart.type = particleName;
		newPart.initFrame = Globals.frame;
		AddChild(newPart);
		newPart.FlipH = flipH;
		newPart.Position = location;
	}

	public void OnGhostEmitted(Player p)
	{
		var newGhost = (Sprite) dashGhost.Instance();
		AddChild(newGhost);
		ghosts.Add(newGhost);
		newGhost.ZIndex = -1;
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
		foreach (var child in GetChildren())
		{
			var partSprite = child as ParticleSprite;
			if (partSprite != null)
			{
				partSprite.Rollback(frame);
			}

		}
	}
}
