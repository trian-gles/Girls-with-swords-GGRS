using Godot;
using System;

public class PlusFrames : Label
{
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";

	// Called when the node enters the scene tree for the first time.

	[Export]
	public int speed = 5;

	public override void _Ready()
	{
		
	}

	public override void _PhysicsProcess(float delta)
	{
		base._PhysicsProcess(delta);

		RectPosition = new Vector2(RectPosition.x, RectPosition.y - speed);

		if (RectGlobalPosition.y < -20)
		{
			QueueFree();
		}
	}

	public void Init(int frames)
	{
		if (frames > 0)
		{
			Text = "+" + frames.ToString();
		}
		else
		{
			Text = frames.ToString();
		}
	}
}
