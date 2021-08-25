using Godot;
using System;
using System.Collections.Generic;

public class Crouch : State
{
    public override void _Ready()
    {
        base._Ready();
        loop = true;
    }
    public override void Enter()
    {
        base.Enter();
        owner.gravityPos = 0;
        owner.velocity.x = 0;
        owner.velocity.y = 0;
        
        if (owner.CheckHeldKey('8'))
        {
            EmitSignal(nameof(StateFinished), "Jump");
        }
    } 
    public override void HandleInput(char[] inputArr)
    {
        if (Globals.CheckKeyRelease(inputArr, '2'))
        {
            EmitSignal(nameof(StateFinished), "Idle");
        }

        else if (Globals.CheckKeyPress(inputArr, 'p'))
        {
            if (!owner.facingRight && owner.CheckBufferComplex(new List<char[]>() { new char[] { '4', 'p' }, new char[] { '2', 'p' }, new char[] { '4', 'p' } }))
            {
                EmitSignal(nameof(StateFinished), "DP");
            }
            else if (owner.facingRight && owner.CheckBufferComplex(new List<char[]>() { new char[] { '6', 'p' }, new char[] { '2', 'p' }, new char[] { '6', 'p' } }))
            {
                EmitSignal(nameof(StateFinished), "DP");
            }
            else 
            {
                EmitSignal(nameof(StateFinished), "CrouchJab");
            }
            
        }

        else if (Globals.CheckKeyPress(inputArr, 'k')) 
        {
            EmitSignal(nameof(StateFinished), "Sweep");
        }

        else if (Globals.CheckKeyPress(inputArr, 's'))
        {
            if (owner.CheckBufferComplex(new List<char[]>() { new char[] {'2', 'p' }, new char[] { '2', 'p' } }))
            {
                EmitSignal(nameof(StateFinished), "AntiAir");
            }
            else
            {
                EmitSignal(nameof(StateFinished), "CrouchSlash");
            }
        }

        else if (Globals.CheckKeyPress(inputArr, '8'))
        {
            EmitSignal(nameof(StateFinished), "Jump");
        }
    }

    public override void FrameAdvance()
    {
        base.FrameAdvance();
        owner.velocity.x = 0;
        owner.CheckTurnAround();
    }
}

