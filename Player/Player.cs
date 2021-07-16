using Godot;
using System;

public class Player : KinematicBody2D
{
    private State currentState;
    public KinematicBody2D otherPlayer;

    [Export]
    private int speed = 200;

    [Export]
    private int jumpForce = 400;

    [Export]
    private int gravity = 13;

    public Vector2 velocity = new Vector2(0, 0);

    private bool facingRight;
    private bool touchingWall = false;
    private bool grounded;
    private bool hitstopped;

    public int combo = 0;

    public string[] inputBuf = new string[4];

    public override void _Ready()
    {
        Godot.Collections.Array allStates = GetNode<Node>("StateTree").GetChildren();
        foreach (Node state in allStates) 
        {
            state.Connect("StateFinished", this, nameof(ChangeState));
        }
        currentState = GetNode<State>("StateTree/Idle");
        ChangeState("Idle");
    }

    public void ChangeState(string nextStateName) 
    {
        currentState.Exit();
        currentState = GetNode<State>("StateTree/" + nextStateName);
        GetNode<AnimationPlayer>("AnimationPlayer").Play(nextStateName);
        GD.Print("Entering State " + nextStateName);
        currentState.Enter();
    
    }

    public void FrameAdvance() 
    {
        velocity.y += gravity;
        MoveAndSlide(velocity, Vector2.Up);
    }
}
