using Godot;
using System;

public class Kick : State
{
    private bool hitConnect = false;

    [Export]
    protected int hitStun = 10;

    [Export]
    protected Vector2 hitPush = new Vector2();

    [Export]
    protected string height = "mid";

    [Export]
    protected int dmg = 1;
    public override void Enter()
    {
        hitConnect = false;
    }
    public override void AnimationFinished()
    {
        EmitSignal(nameof(StateFinished), "Idle");
    }

    public override void InHurtbox()
    {
        if (!hitConnect) 
        {
            GD.Print("HIT");
            owner.otherPlayer.ReceiveHit(owner.facingRight, dmg, hitStun, height, hitPush);// this needs to be worked out to allow crossups
            hitConnect = true;
        }
              
    }

    public override void HandleInput(string[] inputArr)
    {
        
        if (inputArr[1] == "press" && hitConnect) 
        {
            
            if (inputArr[0] == "k")
            {
                EmitSignal(nameof(StateFinished), "Kick");
            }
            else if (inputArr[0] == "up") 
            {
                EmitSignal(nameof(StateFinished), "Jump");
            }
        }
    }
}

