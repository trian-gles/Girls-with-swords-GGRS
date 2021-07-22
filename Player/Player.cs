using Godot;
using System;
using System.Collections.Generic;

public class Player : KinematicBody2D
{
    private State currentState;
    public Player otherPlayer;

    [Signal]
    public delegate void HealthChanged(string name, int health);
    [Signal]
    public delegate void ComboChanged(string name, int combo);
    [Signal]
    public delegate void HitConfirm();

    [Export]
    public int speed = 200;

    [Export]
    public int jumpForce = 400;

    [Export]
    public int gravity = 13;

    [Export]
    public bool dummy = false;

    public InputHandler inputHandler;

    // All of these will be stored in gamestate
    private int health = 100;
    public Vector2 velocity = new Vector2(0, 0);
    public bool facingRight = true;
    public bool touchingWall = false;
    public bool grounded;
    private int combo = 0;

    public struct PlayerState
    {
        public List<List<char[]>> inputBuffer { get; set; }
        public List<char> heldKeys { get; set; }
        public List<char[]> unhandledInputs { get; set; }
        public string currentState { get; set; }
        public int frameCount { get; set; }
        public int stunRemaining { get; set; }

        public int health { get; set; }
        public float[] position { get; set; }
        public float[] velocity { get; set; }
        public bool facingRight { get; set; }
        public bool touchingWall { get; set; }
        public bool grounded { get; set; }
        public int combo { get; set; }


    }
    

    private Color hitColor = new Color(255, 0, 0, 0.5f);
    private Color hurtColor = new Color(0, 255, 0, 0.5f);
    private Color colColor = new Color(0, 0, 255, 0.5f);

    private Area2D hitBoxes;
    public Area2D hurtBoxes;
    private CollisionShape2D colBox;
    public AnimationPlayer animationPlayer;

    

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

    public PlayerState GetState()
    {
        var pState = new PlayerState();
        pState.inputBuffer = inputHandler.inputBuffer;
        pState.heldKeys = inputHandler.heldKeys;
        pState.unhandledInputs = inputHandler.unhandledInputs;
        pState.currentState = currentState.Name;
        pState.frameCount = currentState.frameCount;
        pState.stunRemaining = currentState.stunRemaining;

        pState.health = health;
        pState.position = new float[] { Position.x, Position.y };


        pState.velocity = new float[] { velocity.x, velocity.y };
        pState.facingRight = facingRight;
        pState.touchingWall = touchingWall;
        pState.grounded = grounded;
        pState.combo = combo;
        return pState;
    }

    public void SetState(PlayerState pState) //adjust this
    {
        inputHandler.inputBuffer = pState.inputBuffer;
        inputHandler.heldKeys = pState.heldKeys;
        inputHandler.unhandledInputs = pState.unhandledInputs;
        currentState = GetNode<State>("StateTree/" + pState.currentState);
        currentState.frameCount = pState.frameCount;
        animationPlayer.SetAnimationAndFrame(pState.currentState, pState.frameCount);
        currentState.stunRemaining = pState.stunRemaining;

        health = pState.health;
        Position = new Vector2(pState.position[0], pState.position[1]);
        velocity = new Vector2(pState.velocity[0], pState.velocity[1]);
        facingRight = pState.facingRight;
        grounded = pState.grounded;
        combo = pState.combo;
        EmitSignal(nameof(ComboChanged), combo, Name);

    }

    public class InputHandler 
    {
        public List<List<char[]>> inputBuffer = new List<List<char[]>>();
        public List<char> heldKeys = new List<char>();
        public List<char[]> unhandledInputs = new List<char[]>();

        public void NewInput(char key, char pressOrRelease) 
        {
            unhandledInputs.Add(new char[] { key, pressOrRelease });
        }
        public void FrameAdvance(bool hitStop, State currentState) 
        {
            List<char[]> curBufStep = new List<char[]>();
            foreach (char[] inputArr in unhandledInputs)
            {
                
                // Hold or release keys
                if (inputArr[1] == 'p')
                {
                    heldKeys.Add(inputArr[0]);
                }
                else if (inputArr[1] == 'r')
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

        public List<char[]> GetBuffer() 
        {
            List<char[]> flatBuf = new List<char[]>();
            foreach (List<char[]> frameInputs in inputBuffer)
            {
                foreach (char[] input in frameInputs) 
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
        animationPlayer.NewAnimation(nextStateName);
        currentState = GetNode<State>("StateTree/" + nextStateName);
        
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

    public bool CheckHeldKey(char key) 
    {
        return (inputHandler.heldKeys.Contains(key));
    }

    public bool CheckBuffer(char[] key) // FIX THIS
    {
        return (inputHandler.GetBuffer().Contains(key));
    }

    public void FrameAdvance(bool hitStop) 
    {
        if (hitStop)
        {
            return;
        }
        Update(); //Redraw
        inputHandler.FrameAdvance(hitStop, currentState);
        animationPlayer.FrameAdvance();
        Vector2 currVel = new Vector2(velocity.x, velocity.y);
        MoveAndSlide(velocity, Vector2.Up, maxSlides: 4);
        if (!velocity.IsEqualApprox(currVel) && Name == "P2")
        {
            GD.Print($"Move and slide changed velocity to {velocity}");
        }

        foreach (Area2D area in hurtBoxes.GetOverlappingAreas()) 
        {
            if (area.Owner == otherPlayer) 
            {
                if (area.Name == "HitBoxes" && animationPlayer.cursor > 1) 
                {
                    currentState.InHurtbox();
                } 
            }
        }

        grounded = false; // will be set true if touching the ground
        if (velocity.x > 0)
        {
            touchingWall = false;
        }
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
            else if (collisionObj.Name == "Walls")
            {
                touchingWall = true;
            }
            
        }
        currentState.FrameAdvance();
    }

    public void SlideAway() 
    {
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

    public void OnHitConnected(Vector2 hitPush) 
    {
        if (otherPlayer.touchingWall)
        {
            GD.Print("Other player at wall, pushing attacker");
            if (OtherPlayerOnRight())
            {
                velocity.x = -hitPush.x;
            }
            else
            {
                velocity.x = hitPush.x;
            }
        }
    }

    public void ResetCombo()
    {
        combo = 0;
        EmitSignal(nameof(ComboChanged), Name, combo);
    }

    public void ComboUp()
    {
        combo++;
        GD.Print($"Combo up to {combo}");
        EmitSignal(nameof(ComboChanged), Name, combo);
    }

    public void DeductHealth(int dmg)
    {
        health -= dmg;
        EmitSignal(nameof(HealthChanged), Name, health);
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
