using Godot;
using System;

public class MainGFX : Node
{
	private int lastLevelUp = 0;
	public void LevelUp(int frame)
	{
		GetNode<Node2D>("Stages").Call("level_up");
		lastLevelUp = frame;
	}

	public void Rollback(int frame)
	{
		if (frame < lastLevelUp)
		{
			GetNode<Node2D>("Stages").Call("rollback");
		}
	}
}
