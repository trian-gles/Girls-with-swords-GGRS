using Godot;
using System;

public class ParticleSprite : Sprite
{
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";

	public int initFrame;
	public string type;
	private Godot.AnimationPlayer animationPlayer;
	private const string AnimationNameString = "Animation";

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		animationPlayer = GetNode<Godot.AnimationPlayer>("AnimationPlayer");
		animationPlayer.Play(AnimationNameString);
	}

	public void Reassign()
	{
		animationPlayer.Play(AnimationNameString);
		Visible = true;
	}

	public void Rollback(int frame)
	{
		if (frame < initFrame)
		{
			Visible = false;
		}
	}

	public void OnAnimationFinished(String anim_name)
	{
		Visible = false;
	}
}
