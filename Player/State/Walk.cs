using Godot;
using System;
using System.Collections.Generic;

public class Walk : State
{
    protected int soundRate = 15;
    public override void _Ready()
    {
        base._Ready();
        loop = true;
    }
    public override void Enter()
    {
        base.Enter();
        if (owner.CheckHeldKey('8'))
        {
            EmitSignal(nameof(StateFinished), "MovingJump");
        }
    }

    public override void HandleInput(char[] inputArr)
    {
        if (Globals.CheckKeyPress(inputArr, 'p'))
        {
            EmitSignal(nameof(StateFinished), "Jab");

            if (owner.facingRight && owner.CheckBufferComplex(new List<char[]>() { new char[] { '2', 'p'}, new char[] { '6', 'p' }, new char[] { '2', 'r' } })) 
            {
                EmitSignal(nameof(StateFinished), "Hadouken");
            }
            else if ((!owner.facingRight) && owner.CheckBufferComplex(new List<char[]>() { new char[] { '2', 'p' }, new char[] { '4', 'p' }, new char[] { '2', 'r' } }))
            {
                EmitSignal(nameof(StateFinished), "Hadouken");
            }
        }
        else if ((inputArr[0] == '6' || inputArr[0] == '4') && inputArr[1] == 'r')
        {
            EmitSignal(nameof(StateFinished), "Idle");
        }
        else if (Globals.CheckKeyPress(inputArr, '6') && !owner.facingRight)
        {
            if (owner.CheckBufferComplex(new List<char[]>() { new char[] { '6', 'p' }, new char[] { '6', 'p' } }))
            {
                owner.velocity.x -= owner.speed;
                EmitSignal(nameof(StateFinished), "Backdash");
            }

        }
        else if (Globals.CheckKeyPress(inputArr, '8'))
        {
            EmitSignal(nameof(StateFinished), "MovingJump");
        }
        else if (Globals.CheckKeyPress(inputArr, 'k'))
        {
            owner.velocity.x = 0;
            EmitSignal(nameof(StateFinished), "Kick");
        }
        else if (Globals.CheckKeyPress(inputArr, '2'))
        {
            EmitSignal(nameof(StateFinished), "Crouch");
        }

        else if (Globals.CheckKeyPress(inputArr, 's'))
        {
            if (owner.Position.DistanceTo(owner.otherPlayer.Position) < 45)
            {
                EmitSignal(nameof(StateFinished), "Grab");
            }
            else
            {
                EmitSignal(nameof(StateFinished), "Slash");
            }
            
        }
    }

    public override void FrameAdvance()
    {
        base.FrameAdvance();
        owner.CheckTurnAround();
        if (frameCount % soundRate == 0)
        {
            owner.ScheduleEvent(EventScheduler.EventType.AUDIO, "Step", Name);
        }
    }

    public override void PushMovement(float _xVel)
    {
    }
}

