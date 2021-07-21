using Godot;
using System;

public class Fall : Jump
{
    public override void Enter()
    {
        owner.combo = 0;
    }
}
