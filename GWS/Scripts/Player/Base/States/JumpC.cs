using Godot;
using System;

public class JumpC : AirAttack
{

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
