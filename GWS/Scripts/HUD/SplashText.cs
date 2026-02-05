using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using Godot;

public class SplashText : Label
{
	[Export]
	public float dissapear_time = 1;

	private int last_display = 0;
	private Timer timer;

	public override void _Ready()
	{
		Visible = false;
		timer = GetNode<Timer>("Timer");
	}

	public void Display(int frame)
	{
		last_display = frame;
		Visible = true;
		timer.Start();
	}

	public void Rollback(int frame)
	{
		if (Visible && frame < last_display)
		{
			Visible = false;
		}
	}

	private void _on_Timer_timeout()
	{
		Visible = false;
	}
}
