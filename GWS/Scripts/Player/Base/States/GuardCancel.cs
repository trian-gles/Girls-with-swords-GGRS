using Godot;
using System;

public class GuardCancel : GroundAttack
{

	public override void _Ready()
	{
		base._Ready();
	}

	public override void Enter()
	{
		base.Enter();
		owner.GFXEvent("GuardCancel");
    }
}
