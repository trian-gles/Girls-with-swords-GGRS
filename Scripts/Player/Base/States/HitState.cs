using Godot;
using System;

public class HitState : State
{
	public override void Enter()
	{
		base.Enter();
	}
	public override bool DelayInputs()
	{
		return frameCount > 0;
	}
}
