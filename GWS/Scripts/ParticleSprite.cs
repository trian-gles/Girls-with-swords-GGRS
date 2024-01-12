using Godot;
using System;

public class ParticleSprite : Sprite
{
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";

	public int initFrame;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GetNode<Godot.AnimationPlayer>("AnimationPlayer").Play("Animation");
		
	}

	public void Rollback(int frame)
	{
		if (frame < initFrame)
		{
			QueueFree();
		}
	}

	public void OnAnimationFinished(String anim_name)
	{
		QueueFree();
	}
}
