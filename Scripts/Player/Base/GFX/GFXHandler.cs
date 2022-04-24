using Godot;
using System;
using System.Collections.Generic;

public class GFXHandler : Node
{
	private Dictionary<string, PlayerParticle> particlesCPU;
	private Dictionary<string, PlayerParticleGPU> particlesGPU;
	
	//private PlayerParticle blood;
	//private PlayerParticle cancel;
	//private PlayerParticleGPU light;

	public override void _Ready()
	{
		particlesCPU = new Dictionary<string, PlayerParticle>();
		particlesGPU = new Dictionary<string, PlayerParticleGPU>();

		foreach (object node in GetChildren())
		{
			GD.Print(node.GetType());
			if (node.GetType() == typeof(PlayerParticle))
			{
				GD.Print("adding to particles CPU");
				particlesCPU.Add(((PlayerParticle)node).Name, (PlayerParticle)node);
			}
			else if (node.GetType() == typeof(PlayerParticleGPU))
			{
				particlesGPU.Add(((PlayerParticleGPU)node).Name, (PlayerParticleGPU)node);
			}
		}
		//blood = GetNode<PlayerParticle>("Blood");
		//cancel = GetNode<PlayerParticle>("Cancel");
		//light = GetNode<PlayerParticleGPU>("Light");
	}

	public void Effect(string name, Vector2 pos, bool facingRight)
	{
		//if (name == "Blood")
		//{
		//	blood.Trigger(0, pos, facingRight);
		//}
		//else if (name == "Cancel")
		//{
		//	cancel.Trigger(0, pos, false);
		//}
		//else if (name == "Light")
		//{
		//	GD.Print("Triggering light");
		//	light.Trigger(0, pos / 100, false);
		//}	
		if (particlesCPU.ContainsKey(name))
		{
			particlesCPU[name].Trigger(0, pos, facingRight);
		}
		else if (particlesGPU.ContainsKey(name))
		{
			particlesGPU[name].Trigger(0, pos, facingRight);
		}
		else
		{ 
			throw new Exception($"'{name}' is not a valid graphic effect"); 
		}
	}

	public void Rollback(int frame)
	{
		foreach (PlayerParticle p in particlesCPU.Values)
			p.Rollback(frame);

		foreach (PlayerParticleGPU p in particlesGPU.Values)
			p.Rollback(frame);
	}


}
