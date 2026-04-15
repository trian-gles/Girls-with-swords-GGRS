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
	private bool fading = false;
	private float fadeT = 0f;

	private const string ModulateAlphaString = "modulate:a";

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Visible = false;
	}

	public void StartFade()
	{
		fadeT = 0f;
		fading = true;
		Visible = true;
	}

	public override void _Process(float delta)
	{
		if (!fading) return;

		fadeT += delta;

		float t = Mathf.Clamp(fadeT / 2f, 0, 1);
		float eased = 1f - Mathf.Pow(2f, -10f * t);

		float alpha = Mathf.Lerp(1f, 0f, eased);

		Modulate = new Color(Modulate.r, Modulate.g, Modulate.b, alpha);

		if (t >= 1f)
		{
			fading = false;
			Visible = false;
		}
			
	}

	public void Run(int frame)
	{
		Visible = true;
		StartFade();
		initFrame = frame;
	}

	public void Rollback(int frame)
	{
		if (frame < initFrame)
		{
			Visible = false;
			fading = false;
		}
	}

	private void _on_Tween_tween_completed(Godot.Object @object, NodePath key)
	{
		
	}
}


