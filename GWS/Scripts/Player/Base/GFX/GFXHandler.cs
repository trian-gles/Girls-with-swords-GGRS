using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class GFXHandler : Node
{
	private Dictionary<string, PlayerParticle> particlesCPUDict;
	private Dictionary<string, PlayerParticleGPU> particlesGPUDict;

	private PlayerParticle[] particlesCPU;
	private PlayerParticleGPU[] particlesGPU;
	private const string SlashString = "Slash";
	private DrawFX drawFX;
	
	
	//private PlayerParticle blood;
	//private PlayerParticle cancel;
	//private PlayerParticleGPU light;


	public override void _Ready()
	{
		particlesCPUDict = new Dictionary<string, PlayerParticle>();
		particlesGPUDict = new Dictionary<string, PlayerParticleGPU>();
		drawFX = GetNode<DrawFX>("DrawFX");
		

		foreach (object node in GetChildren())
		{
			if (node.GetType() == typeof(PlayerParticle))
			{
				particlesCPUDict.Add(((PlayerParticle)node).Name, (PlayerParticle)node);
			}
			else if (node.GetType() == typeof(PlayerParticleGPU))
			{
				particlesGPUDict.Add(((PlayerParticleGPU)node).Name, (PlayerParticleGPU)node);
			}
		}
		particlesCPU = particlesCPUDict.Values.ToArray();
		particlesGPU = particlesGPUDict.Values.ToArray();
		//blood = GetNode<PlayerParticle>("Blood");
		//cancel = GetNode<PlayerParticle>("Cancel");
		//light = GetNode<PlayerParticleGPU>("Light");
	}

	public void Effect(string name, Vector2 pos, bool facingRight)
	{
		if (Globals.DISABLEPARTICLES)
			return;
		if (particlesCPUDict.ContainsKey(name))
		{
			particlesCPUDict[name].Trigger(0, pos, facingRight);
		}
		else if (particlesGPUDict.ContainsKey(name))
		{
			particlesGPUDict[name].Trigger(0, pos, facingRight);
		}
		else if (name == SlashString)
		{
			drawFX.Slash(pos);
		}
		else
		{ 
			// throw new Exception($"'{name}' is not a valid graphic effect"); 
		}
	}

	public void Rollback(int frame)
	{
		for (int i = 0; i < particlesCPU.Length; i ++)
			particlesCPU[i].Rollback(frame);

		for (int i = 0; i < particlesCPU.Length; i ++)
			particlesGPU[i].Rollback(frame);
	}


}
