using Godot;
using System;
using System.Collections.Generic;

public class GFXHandler : Node
{
	private Blood blood;

	public override void _Ready()
	{
		blood = GetNode<Blood>("Blood");
	}

	public void Effect(string name, Vector2 pos, bool facingRight)
	{
		if (name == "Blood")
		{
			blood.Trigger(0, pos, facingRight);
		}
	}

	public void Rollback(int frame)
	{
		blood.Rollback(frame);
	}


}
