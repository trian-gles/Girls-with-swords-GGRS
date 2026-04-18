using System;
using Godot;

public class OhShit : Label
{
	[Export]
	public float dissapear_time = 1;

	public int last_display = 0;
	private Timer timer;

	public override void _Ready()
	{
		Visible = false;
		timer = GetNode<Timer>("Timer");
		timer.WaitTime = dissapear_time;
	}

	public void Display(int frame)
	{
		last_display = frame;
		Visible = true;
		timer.WaitTime = dissapear_time;
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

