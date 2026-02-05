using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class MainGFX : Node
{
	private int lastLevelUp = -100;
	private List<Sprite> ghosts = new List<Sprite>();
	private PackedScene dashGhost = (PackedScene) ResourceLoader.Load("res://Scenes/DashGhost.tscn");
	private Dictionary<string, PackedScene> particleSprites = new Dictionary<string, PackedScene>();
	private Node2D stages;
	private Godot.Collections.Array<Node> children = new Godot.Collections.Array<Node>();

	// runtime call strings
	private const string SetBkgCallString = "set_bkg";
	private const string LevelUpCallString = "level_up";
	public override void _Ready()
	{
		Globals.ConnectGhostEmitted(OnGhostEmitted);
		Globals.ConnectGFXParticleEmitted(OnGFXParticleEmitted);
		stages = GetNode<Node2D>("Stages");
		children = new Godot.Collections.Array<Node>();

// store referencecs to all particles
		particleSprites.Add("hit", (PackedScene)ResourceLoader.Load("res://Scenes/Particles/HitFX.tscn"));
		particleSprites.Add("block", (PackedScene)ResourceLoader.Load("res://Scenes/Particles/BlockFX.tscn"));
		particleSprites.Add("shield", (PackedScene)ResourceLoader.Load("res://Scenes/Particles/ShieldFX.tscn"));
		particleSprites.Add("dust", (PackedScene)ResourceLoader.Load("res://Scenes/Particles/DustFX.tscn"));
		particleSprites.Add("burst", (PackedScene)ResourceLoader.Load("res://Scenes/Particles/Burst.tscn"));
		particleSprites.Add("coffee", (PackedScene)ResourceLoader.Load("res://Scenes/Particles/CoffeeExplosion.tscn"));

// render all particles NOW since C# has no preload
		for (int i = 0; i < 15; i++)
		{
			Sprite newGhost = (Sprite)dashGhost.Instance(); // Added to tree and thus freed automatically
			CallDeferred("add_child", newGhost);
			ghosts.Add(newGhost);
			children.Add(newGhost);
		}


		foreach (var sprite in particleSprites.Keys)
		{
			for (int i = 0; i < 4; i++)
			{
				var p = ReleaseNewParticle(new Vector2(0, 0), sprite, true);
				p.Visible = false;
			}
				
		}

	}
	
	public void Init(int background){
		stages.Call(SetBkgCallString, background);
	}

	public void LevelUp(int frame)
	{
		stages.Call(LevelUpCallString);
		lastLevelUp = frame;
	}

	public void OnGFXParticleEmitted(Vector2 location, string particleName, bool flipH)
	{
		if (Globals.DISABLEGFX)
			return;
		location /= 100;
		for (int i = 0; i < children.Count; i++)
		{
			var child = children[i];
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

	private ParticleSprite ReleaseNewParticle(Vector2 location, string particleName, bool flipH)
	{
		var newPart = (ParticleSprite)particleSprites[particleName].Instance(); // Added to tree and thus freed automatically
		newPart.type = particleName;
		newPart.initFrame = Globals.frame;
		CallDeferred("add_child", newPart);
		children.Add(newPart);
		newPart.FlipH = flipH;
		newPart.Position = location;
		return newPart;
	}

	public void OnGhostEmitted(Player p)
	{
		if (Globals.DISABLEGFX)
			return;
		foreach (DashGhost newGhost in ghosts.Cast<DashGhost>())
		{
			if (!newGhost.Visible)
			{
				newGhost.ZIndex = -1;
				newGhost.GlobalPosition = p.sprite.GlobalPosition;
				newGhost.Texture = p.sprite.Texture;
				newGhost.Vframes = p.sprite.Vframes;
				newGhost.Hframes = p.sprite.Hframes;
				newGhost.Frame = p.sprite.Frame;
				newGhost.Scale = p.sprite.Scale;
				newGhost.FlipH = p.sprite.FlipH;
				newGhost.Run(Globals.frame);
				return;
			}
		}
		
	}

	public void Rollback(int frame)
	{
		for (int i = 0; i < children.Count; i++)
		{
			var child = children[i];
			if (child is DashGhost ghost)
			{
				ghost.Rollback(frame);
			}
			
			if (child is ParticleSprite sprite)
			{
				sprite.Rollback(frame);
			}
		}
	}
}
