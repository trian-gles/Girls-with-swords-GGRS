using Godot;
using System;
using System.Collections.Generic;

public class PlusFrames : Label
{
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";

	// Called when the node enters the scene tree for the first time.

	[Export]
	public int speed = 5;

	private static Dictionary<int, string> plusStrings = new Dictionary<int, string>();

	public override void _Ready()
	{
		for (int i = -100; i < 100; i++)
		{
			if (!plusStrings.ContainsKey(i))
			{
				if (i > 0)
				{
					plusStrings[i] = "+" + i.ToString();
				}
				else
				{
					plusStrings[i] = i.ToString();
				}
			}
		}
	}

	public override void _PhysicsProcess(float delta)
	{
		base._PhysicsProcess(delta);
		if (!Visible)
			return;
		

		if (RectGlobalPosition.y < -20)
			Visible = false;

		RectPosition = new Vector2(RectPosition.x, RectPosition.y - speed);
	}

	public void Init(int frames)
	{
		Visible = true;
		Text = plusStrings[frames];
	}
}
