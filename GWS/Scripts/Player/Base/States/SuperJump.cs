using Godot;
using System;

public class DoubleJump : Jump
{
    public override void Enter()
    {
        base.Enter();
        owner.hasDoubleOrSuperJumped = true;
    }
}


