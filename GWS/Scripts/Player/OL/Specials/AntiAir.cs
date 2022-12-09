using Godot;
using System;

public class AntiAir : LaunchAttack
{
    public override void _Ready()
    {
        base._Ready();
        AddRhythmSpecials(owner.rhythmSpecials);
    }
}
