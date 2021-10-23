using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public abstract class BaseAttack : State
{
    [Export]
    protected int hitStun = 10;

    [Export]
    protected int blockStun = 11;

    [Export]
    protected Vector2 opponentLaunch = new Vector2();

    [Export]
    protected int hitPush = 0;

    [Export]
    protected HEIGHT height = HEIGHT.MID;

    [Export]
    protected int dmg = 1;

    [Export]
    protected bool knockdown = false;

    [Signal]
    public delegate void OnHitConnected(int hitPush);

    

    protected List<NormalGatling> normalGatlings = new List<NormalGatling>();
    protected List<CommandGatling> commandGatlings = new List<CommandGatling>();
    protected struct NormalGatling
    {
        public char[] input;
        public string state;
    }

    protected struct CommandGatling
    {
        public List<char[]> inputs;
        public string state;
    }

    protected void AddGatling(char[] input, string state)
    {
        var newGatling = new NormalGatling
        {
            input = input,
            state = state
        };
        normalGatlings.Add(newGatling);
    }

    protected void AddGatling(List<char[]> inputs, string state)
    {
        var newGatling = new CommandGatling
        {
            inputs = inputs,
            state = state
        };
        commandGatlings.Add(newGatling);
    }
    public override void _Ready()
    {
        base._Ready();
        Connect("OnHitConnected", owner, nameof(owner.OnHitConnected));
    }
    public override void Enter()
    {
        base.Enter();
        hitConnect = false;
        owner.ScheduleEvent(EventScheduler.EventType.AUDIO, "Whiff", Name);
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
            owner.otherPlayer.ReceiveHit(owner.OtherPlayerOnRight(), dmg, blockStun, hitStun, height, hitPush, opponentLaunch, knockdown);
            hitConnect = true;
        }

    }

    public override void HandleInput(char[] inputArr)
    {
        if (!hitConnect)
        {
            return;
        }
        foreach (CommandGatling comGat in commandGatlings)
        {
            if (Enumerable.SequenceEqual(comGat.inputs[comGat.inputs.Count - 1], inputArr))
            {
                if (owner.CheckBufferComplex(comGat.inputs))
                {
                    EmitSignal(nameof(StateFinished), comGat.state);
                    return;
                }
            }
        }
        foreach (NormalGatling normGat in normalGatlings)
        {
            if (Enumerable.SequenceEqual(normGat.input, inputArr))
            {
                EmitSignal(nameof(StateFinished), normGat.state);
                return;
            }
        }
    }

    protected override void EnterHitState(bool knockdown, Vector2 launch)
    {
        bool launchBool = false;

        if (!(launch == Vector2.Zero)) // LAUNCH NEEDS MORE WORK
        {
            GD.Print("Launch is not zero!");
            owner.velocity = launch;
            launchBool = true;
        }

        if (launchBool && !knockdown)
        {
            EmitSignal(nameof(StateFinished), "CounterFloat");
        }
        else if (launchBool && knockdown)
        {
            EmitSignal(nameof(StateFinished), "AirKnockdown");
        }
        else if (!launchBool && knockdown)
        {
            EmitSignal(nameof(StateFinished), "Knockdown");
        }
        else
        {
            EmitSignal(nameof(StateFinished), "CounterHit");
        }
    }
}
