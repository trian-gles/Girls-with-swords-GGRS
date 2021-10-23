using Godot;
using System;
using System.Collections.Generic;

public class CommandRun : State
{
    [Export]
    public int minLen = 10;

    [Export]
    public int maxLen = 80;

    [Export]
    public int speed = 500;

    private bool exited = false;
    private bool oneHit = false;

    public override void _Ready()
    {
        base._Ready();
        loop = true;
        
    }
    public override void Enter()
    {
        base.Enter();
        exited = false;
        oneHit = false;
        if (owner.facingRight)
        {
            owner.velocity.x = speed;
        }
        else
        {
            owner.velocity.x = -speed;
        }
    }


    public override void FrameAdvance()
    {
        base.FrameAdvance();
        if (frameCount > maxLen)
        {
            EmitSignal(nameof(StateFinished), "Idle");
        }
        if ((frameCount > minLen) && exited)
        {
            EmitSignal(nameof(StateFinished), "Hojogiri");
        }
    }

    public override void HandleInput(char[] inputArr)
    {
        if (Globals.CheckKeyPress(inputArr, 's'))
        {
            exited = true;
            GD.Print("Set to exit");
        }
    }

    public override void ReceiveHit(bool rightAttack, HEIGHT height, int hitPush, Vector2 launch, bool knockdown)
    {
        if (!oneHit)
        {
            owner.ScheduleEvent(EventScheduler.EventType.AUDIO, "HitStun", Name);
            owner.GFXEvent("Blood");
            oneHit = true;
        }
        else
        {
            base.ReceiveHit(rightAttack, height, hitPush, launch, knockdown);
        }
    }

    public override void receiveStun(int hitStun, int blockStun)
    {
        if (oneHit)
        {
            base.receiveStun(hitStun, blockStun);
        }
    }
}