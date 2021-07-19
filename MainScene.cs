using Godot;
using System;

public class MainScene : Node2D
{
    public int Frame = 0;
    public Player P1;
    public Player P2;
    public override void _Ready()
    {
        P1 = GetNode<Player>("P1");
        P2 = GetNode<Player>("P2");
        P1.otherPlayer = P2;
        P2.otherPlayer = P1;
        P1.CheckTurnAround();
        P2.CheckTurnAround();

        
    }

    public void FrameAdvance() 
    {
        Frame++;
        // GD.Print($"Advance to frame {Frame}");

    }

    public override void _PhysicsProcess(float _delta)
    {
        FrameAdvance();
        P1.FrameAdvance();
        P2.FrameAdvance();
    }


}
