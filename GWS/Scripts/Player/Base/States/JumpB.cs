using Godot;
using System;

public class JumpB : AirAttack
{
	public override void _Ready()
	{
		base._Ready();
		AddGatling(new char[] { 's', 'p' }, "JumpC");
		AddKara(new char[] { 's', 'p' }, "AirGrabStart");
	}

	public override void Enter()
	{
		base.Enter();
		AddJumpCancel();
	}




	public override void AnimationFinished()
	{
		EmitSignal(nameof(StateFinished), "Fall");
	}
}

