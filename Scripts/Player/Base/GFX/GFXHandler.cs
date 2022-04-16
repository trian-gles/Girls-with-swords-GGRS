using Godot;
using System;
using System.Collections.Generic;

public class GFXHandler : Node
{
	private PlayerParticle blood;
	private PlayerParticle cancel;
	private PlayerParticleGPU light;

	public override void _Ready()
	{
		blood = GetNode<PlayerParticle>("Blood");
		cancel = GetNode<PlayerParticle>("Cancel");
		light = GetNode<PlayerParticleGPU>("Light");
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
		else if (name == "Light")
		{
			GD.Print("Triggering light");
			light.Trigger(0, pos / 100, false);
		}	
	}

	public void Rollback(int frame)
	{
		blood.Rollback(frame);
		cancel.Rollback(frame);
	}


}
