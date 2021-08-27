using Godot;
using System;

public class Slash : BaseAttack
{
    // Declare member variables here. Examples:
    // private int a = 2;
    // private string b = "text";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        base._Ready();
        AddGatling(new char[] { '8', 'p' }, "Jump");
    }

}
