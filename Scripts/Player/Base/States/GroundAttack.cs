using Godot;
using System;

public class GroundAttack : BaseAttack
{
    public override void _Ready()
    {
        base._Ready();
        AddCancel("Idle");
    }
    public override void Enter()
	{
		base.Enter();
	}
}

