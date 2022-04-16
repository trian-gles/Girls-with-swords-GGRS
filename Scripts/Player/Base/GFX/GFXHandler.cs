using Godot;
using System;
using System.Collections.Generic;

public class GFXHandler : Node
{
	private PlayerParticle blood;
	private PlayerParticle cancel;

	public override void _Ready()
	{
		blood = GetNode<PlayerParticle>("Blood");
		cancel = GetNode<PlayerParticle>("Cancel");
	}

	public void Effect(string name, Vector2 pos, bool facingRight)
	{
		if (name == "Blood")
		{
			blood.Trigger(0, pos, facingRight);
		}
		else if (name == "Cancel")
        {
			cancel.Trigger(0, pos, false);
        }
	}

	public void Rollback(int frame)
	{
		blood.Rollback(frame);
		cancel.Rollback(frame);
	}


}
