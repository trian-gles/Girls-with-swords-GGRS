using Godot;
using System;

public class DashGhost : Sprite
{
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";

	public int initFrame;
	public string type;

	private Tween tween;

	private const string ModulateAlphaString = "modulate:a";

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		tween = GetNode<Tween>("Tween");
		Visible = false;
	}

	public void Run(int frame)
	{
		Visible = true;
		tween.InterpolateProperty(this, ModulateAlphaString, 1.0, 0.0, 2, Tween.TransitionType.Expo, Tween.EaseType.Out);
		tween.Start();
		initFrame = frame;
	}

	public void Rollback(int frame)
	{
		if (frame < initFrame)
		{
			Visible = false;
		}
	}

	private void _on_Tween_tween_completed(Godot.Object @object, NodePath key)
	{
		Visible = false;
	}
}


