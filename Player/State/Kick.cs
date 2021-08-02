using Godot;
using System;
using System.Collections.Generic;

public class Kick : State
{
    

    [Export]
    protected int hitStun = 10;

    [Export]
    protected Vector2 hitPush = new Vector2();

    [Export]
    protected string height = "mid";

    [Export]
    protected int dmg = 1;

    [Signal]
    public delegate void OnHitConnected(Vector2 hitPush);

    List<char[]> kickToKickInput = new List<char[]> { new char[] { 'k', 'p' }, new char[] { 'k', 'p' } };
    char[] jumpInput = new char[] { '8', 'p' };

    public override void _Ready()
    {
        base._Ready();
        Connect("OnHitConnected", owner, nameof(owner.OnHitConnected));
    }
    public override void Enter()
    {
        base.Enter();
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
            GD.Print($"Hit connect on frame {frameCount}");
            EmitSignal(nameof(OnHitConnected), hitPush);
            owner.otherPlayer.ReceiveHit(owner.OtherPlayerOnRight(), dmg, hitStun, height, hitPush);
            hitConnect = true;
        }
              
    }

    public override void FrameAdvance()
    {
        if (hitConnect)
        {
            if (owner.CheckBufferComplex(kickToKickInput))
            {
                EmitSignal(nameof(StateFinished), "Kick");
            }
            if (owner.CheckBuffer(jumpInput))
            {
                EmitSignal(nameof(StateFinished), "Jump");
            }
        }
        base.FrameAdvance();
        

        
    }

    public override void HandleInput(char[] inputArr)
    {
        
        //if (inputArr[1] == 'p' && hitConnect) 
        //{
            
        //    if (inputArr[0] == 'k')
        //    {
        //        EmitSignal(nameof(StateFinished), "Kick");
        //    }
        //    else if (inputArr[0] == '8') 
        //    {
        //        EmitSignal(nameof(StateFinished), "Jump");
        //    }
        //}
    }
}

