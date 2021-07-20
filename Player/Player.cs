using Godot;
using System;
using System.Collections.Generic;

public class Player : KinematicBody2D
{
    private State currentState;
    public Player otherPlayer;

    public int health = 100;

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
    public InputHandler inputHandler;

    public Vector2 velocity = new Vector2(0, 0);
    public int xAccel = 0;

    public bool facingRight = true;
    private bool touchingWall = false;
    public bool grounded;
    private bool hitstopped;
    private bool playerInHurtbox = false;

    public int combo = 0;

    private Area2D hitBoxes;
    private Area2D hurtBoxes;
    private AnimationPlayer animationPlayer;

    [Signal]
    public delegate void HitConfirm();

    public override void _Ready()
    {
        hitBoxes = GetNode<Area2D>("HitBoxes");
        hurtBoxes = GetNode<Area2D>("HurtBoxes");
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        foreach (CollisionShape2D box in hitBoxes.GetChildren()) 
        {
            box.Shape = new RectangleShape2D();
        }
        foreach (CollisionShape2D box in hurtBoxes.GetChildren())
        {
            box.Shape = new RectangleShape2D();
        }

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
        private List<string[]> inputBuffer = new List<string[]>();
        public List<string> heldKeys = new List<string>();
        private List<string[]> unhandledInputs = new List<string[]>();
        
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
                    if (@event.IsActionPressed(actionName))
                    {
                        string[] inputArr = new string[2] { actionName, "press" };
                        unhandledInputs.Add(inputArr);
                    }

                    else if (@event.IsActionReleased(actionName)) 
                    {
                        string[] inputArr = new string[2] { actionName, "release" };
                        unhandledInputs.Add(inputArr);
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
        public void FrameAdvance(bool hitStop, State currentState) 
        {
            AdvanceBufferTimer();
            foreach (string[] inputArr in unhandledInputs)
            {
                
                // Hold or release keys
                if (inputArr[1] == "press")
                {
                    heldKeys.Add(inputArr[0]);
                }
                else if (inputArr[1] == "release")
                {
                    heldKeys.Remove(inputArr[0]);
                }

                currentState.HandleInput(inputArr);
                inputBuffer.Add(inputArr);

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
        animationPlayer.Play(nextStateName);
        animationPlayer.Seek(0);
        GD.Print("Entering State " + nextStateName);
        currentState.Enter();
        CheckTurnAround();

    }

    public void AnimationFinished(string _animName) 
    {
        currentState.AnimationFinished();
    }

    public bool CheckHeldKey(string key) 
    {
        return (inputHandler.heldKeys.Contains(key));
    }


    public void FrameAdvance(bool hitStop) 
    {
        
        if (!dummy)
        {
            inputHandler.FrameAdvance(hitStop, currentState); 
        }
        if (hitStop)
        {
            return;
        }

        MoveAndSlide(velocity, Vector2.Up);

        foreach (Area2D area in hurtBoxes.GetOverlappingAreas()) 
        {
            if (area.Owner == otherPlayer) 
            {
                if (area.Name == "HitBoxes") 
                {
                    currentState.InHurtbox();
                } 
            }
        }

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

        if (Position.x < otherPlayer.Position.x) 
        {
            mod = -1;
        }
        GlobalPosition = new Vector2(GlobalPosition.x + 4 * mod, GlobalPosition.y);
    }

    public void PushMovement(float xVel) 
    {
        currentState.PushMovement(xVel);
    }

    public void CheckTurnAround() 
    {
        if (otherPlayer == null) 
        {
            return;
        }
        if ((Position.x > otherPlayer.Position.x) && facingRight)
        {
            TurnLeft();
        }
        else if ((Position.x < otherPlayer.Position.x) && !facingRight) 
        {
            TurnRight();
        }
    }

    public void TurnRight()
    {
        facingRight = true;

        GetNode<Sprite>("Sprite").FlipH = false;

        flipAll();
    }

    public void TurnLeft()
    {
        facingRight = false;

        GetNode<Sprite>("Sprite").FlipH = true;

        flipAll();
    }

    public void flipAll()
    {
        foreach (CollisionShape2D hurtBox in hurtBoxes.GetChildren())
        {
            hurtBox.Position = new Vector2(-hurtBox.Position.x, hurtBox.Position.y);
        }
        foreach (CollisionShape2D hitBox in hitBoxes.GetChildren())
        {
            hitBox.Position = new Vector2(-hitBox.Position.x, hitBox.Position.y);
        }
    }

    public void CheckBoxFlip() 
    { 
        if (!facingRight) 
        {
            flipAll();
            GD.Print("Flipping box");
        }
    }

    public void ReceiveHit(bool rightAttack, int dmg, int stun, string height, Vector2 push) 
    {
        currentState.ReceiveHit(rightAttack, height, push);
        currentState.receiveStun(stun);
        currentState.receiveDamage(dmg);
        EmitSignal(nameof(HitConfirm));
    }
}
