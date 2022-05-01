using Godot;
using System;

public class Stagger : HitStun
{
	[Export]
	public int dur = 30;
	public override void _Ready()
	{
		base._Ready();
		loop = true;
		animationName = "HitStun";
	}

	/// <summary>
	/// all staggers are the same length
	/// </summary>
	/// <param name="hitStun"></param>
	/// <param name="blockStun"></param>
	public override void receiveStun(int hitStun, int blockStun)
	{
		stunRemaining = dur;
	}
}

