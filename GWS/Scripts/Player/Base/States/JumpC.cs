using Godot;
using System;

public class JumpC : AirAttack
{

    public override void _Ready()
    {
        base._Ready();
		AddKara(new char[] { 'k', 'p' }, "AirGrabStart");
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
