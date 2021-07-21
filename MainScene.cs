using Godot;
using System;

public class MainScene : Node2D
{
    public int Frame = 0;
    public Player P1;
    public Player P2;
    private int hitStopRemaining = 0;
    [Export]
    private int maxHitStop = 10;
    public override void _Ready()
    {
        P1 = GetNode<Player>("P1");
        P2 = GetNode<Player>("P2");
        P1.otherPlayer = P2;
        P2.otherPlayer = P1;
        P1.CheckTurnAround();
        P2.CheckTurnAround();

        P1.Connect("HitConfirm", this, nameof(HitStop));
        P2.Connect("HitConfirm", this, nameof(HitStop));
    }

    public void FrameAdvance() 
    {
        Frame++;
        if (hitStopRemaining > 0) 
        { 
            hitStopRemaining--; 
        }
        // GD.Print($"Advance to frame {Frame}");

    }

    public void HitStop() 
    {
        hitStopRemaining = maxHitStop;
        GD.Print("HitStop");
    }

    

    public override void _PhysicsProcess(float _delta)
    {
        FrameAdvance();
        P1.FrameAdvance((hitStopRemaining > 0));
        P2.FrameAdvance((hitStopRemaining > 0));
    }

    public override void _Input(InputEvent @event)
    {
        P1.inputHandler.NewInput(@event);
    }


}
