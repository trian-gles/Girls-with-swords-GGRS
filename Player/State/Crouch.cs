using Godot;
using System;
using System.Collections.Generic;

public class Crouch : State
{
    public override void _Ready()
    {
        base._Ready();
        loop = true;

        AddGatling(new[] { '2', 'r' }, "Idle");
        AddGatling(new[] { 'p', 'p' }, "CrouchA");
        AddGatling(new[] { 'k', 'p' }, "CrouchB");
        AddGatling(new[] { 's', 'p' }, "CrouchC");
        AddGatling(new List<char[]>() { new char[] { '6', 'p' }, new char[] { '2', 'p' }, new char[] { '6', 'p' }, new char[] { 'p', 'p' } }, "DP");
		AddGatling(new List<char[]>() { new char[] { '6', 'p' }, new char[] { '2', 'p' }, new char[] { '6', 'r' }, new char[] { '4', 'p' }, new char[] { '2', 'r' }, new[] {'k', 'p' } }, "CommandRun");
        AddGatling(new List<char[]>() { new char[] { '2', 'p' }, new char[] { '2', 'p' }, new char[] { 's', 'p' } }, "AntiAir");
    }
    public override void Enter()
    {
        base.Enter();
        owner.velocity.x = 0;
        owner.velocity.y = 0;
    }

    public override void FrameAdvance()
    {
        base.FrameAdvance();
        owner.velocity.x = 0;
        owner.CheckTurnAround();
    }
}

