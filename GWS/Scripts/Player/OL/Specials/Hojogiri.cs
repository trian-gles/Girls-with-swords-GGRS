using Godot;
using System;

public class Hojogiri : GroundAttack
{
    public override void _Ready()
    {
        base._Ready();
        stop = false;
        slowdownSpeed = 60;
    }

}
