using Godot;
using System;

public class Hadouken : State
{
    private PackedScene hadoukenScene;
    public override void _Ready()
    {
        base._Ready();
        hadoukenScene = (PackedScene)GD.Load("res://Hadouken/HadoukenPart.tscn");
    }
    public override void AnimationFinished()
    {
        
        var h = hadoukenScene.Instance() as HadoukenPart;
        
        h.Spawn(owner.facingRight, owner.otherPlayer);
        owner.EmitHadouken(h);
        h.GlobalPosition = owner.Position;
        EmitSignal(nameof(StateFinished), "Idle");
    }
}
