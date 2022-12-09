using Godot;
using System;

public class DP : LaunchAttack
{
    public override void _Ready()
    {
        base._Ready();
        AddRhythmSpecials(owner.rhythmSpecials);
    }

}
