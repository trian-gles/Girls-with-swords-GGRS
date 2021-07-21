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
    public bool dummy = false;

    public InputHandler inputHandler;

    public Vector2 velocity = new Vector2(0, 0);
    public int xAccel = 0;

    public bool facingRight = true;
    private bool touchingWall = false;
    public bool grounded;

    public int combo = 0;

    private Color hitColor = new Color(255, 0, 0, 0.5f);
    private Color hurtColor = new Color(0, 255, 0, 0.5f);
    private Color colColor = new Color(0, 0, 255, 0.5f);

    private Area2D hitBoxes;
    private Area2D hurtBoxes;
    private CollisionShape2D colBox;
    private AnimationPlayer animationPlayer;

    [Signal]
    public delegate void HitConfirm();

    public override void _Ready()
    {
        hitBoxes = GetNode<Area2D>("HitBoxes");
        hurtBoxes = GetNode<Area2D>("HurtBoxes");
        colBox = GetNode<CollisionShape2D>("CollisionBox");
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        animationPlayer.Connect("AnimationFinished", this, nameof(AnimationFinished));
        foreach (CollisionShape2D box in hitBoxes.GetChildren()) 
        {
            box.Shape = new RectangleShape2D();
        }
        foreach (CollisionShape2D box in hurtBoxes.GetChildren())
        {
            box.Shape = new RectangleShape2D();
        }

        inputHandler = new InputHandler();
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
        private List<List<string[]>> inputBuffer = new List<List<string[]>>();
        public List<string> heldKeys = new List<string>();
        private List<string[]> unhandledInputs = new List<string[]>();

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
        public void FrameAdvance(bool hitStop, State currentState) 
        {
            List<string[]> curBufStep = new List<string[]>();
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
                if (!hitStop)
                {
                    currentState.HandleInput(inputArr);
                }
                
                curBufStep.Add(inputArr);
            }
            inputBuffer.Add(curBufStep);
            if (inputBuffer.Count > 12) 
            {
                inputBuffer.RemoveAt(0);
            }
            unhandledInputs.Clear();
        }

        public List<string[]> GetBuffer() 
        {
            List<string[]> flatBuf = new List<string[]>();
            foreach (List<string[]> frameInputs in inputBuffer)
            {
                foreach (string[] input in frameInputs) 
                {
                    flatBuf.Add(input);
                }
            }
            return flatBuf;
        }
    }//input buffer needs to be tested!!!

    public void ChangeState(string nextStateName) 
    {
        currentState.Exit();
        currentState = GetNode<State>("StateTree/" + nextStateName);
        animationPlayer.NewAnimation(nextStateName);
        GD.Print("Entering State " + nextStateName);
        currentState.Enter();
        CheckTurnAround();

    }

    public void AnimationFinished(string animName) 
    {
        if (currentState.loop) 
        {
            animationPlayer.Restart();
        }
        else
        {
            currentState.AnimationFinished();
        }
    }

    public bool CheckHeldKey(string key) 
    {
        return (inputHandler.heldKeys.Contains(key));
    }


    public void FrameAdvance(bool hitStop) 
    {
        
        inputHandler.FrameAdvance(hitStop, currentState); 
        
        if (hitStop)
        {
            return;
        }
        Update(); //Redraw
        animationPlayer.FrameAdvance();
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

    public bool OtherPlayerOnRight()
    {
        if (Position.x > otherPlayer.Position.x)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public void CheckTurnAround() 
    {
        if (otherPlayer == null) 
        {
            return;
        }
        if (!OtherPlayerOnRight() && facingRight)
        {
            TurnLeft();
        }
        else if (OtherPlayerOnRight() && !facingRight) 
        {
            TurnRight();
        }
    }

    public void TurnRight()
    {
        facingRight = true;

        GetNode<Sprite>("Sprite").FlipH = false;

        hurtBoxes.Scale = new Vector2(1, 1);
        hitBoxes.Scale = new Vector2(1, 1);
    }

    public void TurnLeft()
    {
        facingRight = false;

        GetNode<Sprite>("Sprite").FlipH = true;

        hurtBoxes.Scale = new Vector2(-1, 1);
        hitBoxes.Scale = new Vector2(-1, 1);
    }

    public void ReceiveHit(bool rightAttack, int dmg, int stun, string height, Vector2 push) 
    {
        currentState.ReceiveHit(rightAttack, height, push);
        currentState.receiveStun(stun);
        currentState.receiveDamage(dmg);
        EmitSignal(nameof(HitConfirm));
    }

    public List<Rect2> GetRects(Area2D area) 
    {
        List<Rect2> allRects = new List<Rect2>();
        foreach (CollisionShape2D colShape in area.GetChildren()) 
        {
            if (!colShape.Disabled)
            {
                allRects.Add(GetRect(colShape));
            }
            
        }
        return allRects;
    }

    public Rect2 GetRect(CollisionShape2D colShape) 
    {
        RectangleShape2D shape = (RectangleShape2D)colShape.Shape;
        Vector2 extents = shape.Extents * 2;
        Vector2 position;
        if (facingRight)
        {
            position = colShape.Position - extents / 2;
        }
        else
        {
            position = -colShape.Position - extents / 2;
        }
        return new Rect2(position, extents);
    }

    public override void _Draw()
    {
        List<Rect2> hitRects = GetRects(hitBoxes);
        List<Rect2> hurtRects = GetRects(hurtBoxes);
        Rect2 colRect = GetRect(colBox);
        

        foreach (Rect2 rect in hitRects) 
        {
            DrawRect(rect, hitColor);
        }

        foreach (Rect2 rect in hurtRects)
        {
            DrawRect(rect, hurtColor);
        }

        DrawRect(colRect, colColor);
    }
}
