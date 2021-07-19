using Godot;
using System;
using System.Collections.Generic;

public class Player : KinematicBody2D
{
    private State currentState;
    public Player otherPlayer;

    [Export]
    public int speed = 200;

    [Export]
    public int jumpForce = 400;

    [Export]
    public int gravity = 13;

    [Export]
    private bool dummy = false;

    [Export]
    private int bufferTimeMax = 12;
    private InputHandler inputHandler;

    public Vector2 velocity = new Vector2(0, 0);

    public bool facingRight;
    private bool touchingWall = false;
    public bool grounded;
    private bool hitstopped;

    public int combo = 0;

    

    public override void _Ready()
    {
        inputHandler = new InputHandler(bufferTimeMax);
        Godot.Collections.Array allStates = GetNode<Node>("StateTree").GetChildren();
        foreach (Node state in allStates) 
        {
            state.Connect("StateFinished", this, nameof(ChangeState));
        }
        currentState = GetNode<State>("StateTree/Idle");
        ChangeState("Idle");
    }

    public class InputHandler 
    {
        private string[] allowableInputs = new string[] { "up", "down", "left", "right", "p", "k", "s" };
        private int bufferTimeMax;
        private int bufferTime;
        private List<InputEventKey> inputBuffer = new List<InputEventKey>();
        public List<string> heldKeys = new List<string>();
        private List<InputEventKey> unhandledInputs = new List<InputEventKey>();
        
        public InputHandler(int bufferTime) 
        {
            bufferTimeMax = bufferTime;
            this.bufferTime = bufferTime;
        }

        public void NewInput(InputEvent @event) 
        {
            if (@event is InputEventKey)
            {
                foreach (string actionName in allowableInputs)
                {
                    if (@event.IsActionPressed(actionName) || @event.IsActionReleased(actionName))
                    {
                        unhandledInputs.Add((InputEventKey)@event);
                    }
                }
            }
        }
        private void AdvanceBufferTimer()
        {
            // the timer will sit at -1 if there are no inputs
            if (bufferTime > -1)
            {
                bufferTime--;
            }

            if (bufferTime == 0)
            {
                inputBuffer.Clear();
                GD.Print("Emptying buffer");
            }
        }
        public void FrameAdvance(State currentState) 
        {
            AdvanceBufferTimer();
            foreach (InputEventKey @event in unhandledInputs)
            {
                
                // Hold or release keys
                foreach (string actionName in allowableInputs)
                {
                    if (@event.IsActionPressed(actionName))
                    {
                        heldKeys.Add(actionName);
                    }
                    else if (@event.IsActionReleased(actionName))
                    {
                        heldKeys.Remove(actionName);
                    }
                }

                currentState.HandleInput(@event);
                inputBuffer.Add(@event);

                bufferTime = bufferTimeMax;
            }

            unhandledInputs.Clear();
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (!dummy)
        {
            inputHandler.NewInput(@event);
        }
    }

    public void ChangeState(string nextStateName) 
    {
        currentState.Exit();
        currentState = GetNode<State>("StateTree/" + nextStateName);
        GetNode<AnimationPlayer>("AnimationPlayer").Play(nextStateName);
        GD.Print("Entering State " + nextStateName);
        currentState.Enter();
    
    }

    public void AnimationFinished() 
    {
        currentState.AnimationFinished();
    }

    public bool CheckHeldKey(string key) 
    {
        return (inputHandler.heldKeys.Contains(key));
    }


    public void FrameAdvance() 
    {
        
        if (!dummy)
        {
            inputHandler.FrameAdvance(currentState); 
        }
        
        MoveAndSlide(velocity, Vector2.Up);

        grounded = false; // will be set true if touching the ground
        for (int i = 0; i < GetSlideCount(); i++) 
        {
            

            KinematicCollision2D collision = GetSlideCollision(i);
            Node collisionObj = (Node)collision.Collider; 
            if (collision.Collider == otherPlayer)
            {
                
                if (collision.Normal.x == 0) 
                {
                    SlideAway();
                    otherPlayer.SlideAway();
                }
                else 
                {
                    otherPlayer.PushMovement(velocity.x);
                }
            }
            else if (collisionObj.Name == "Floor") 
            {
                grounded = true;
            }
            
        }

        currentState.FrameAdvance();
    }

    public void SlideAway() 
    {
        GD.Print("Collided with player head");
        var mod = 1;

        if (facingRight) 
        {
            mod = -1;
        }
        GlobalPosition = new Vector2(GlobalPosition.x + 4 * mod, GlobalPosition.y);
    }

    public void PushMovement(float xVel) 
    {
        currentState.PushMovement(xVel);
    }

}
