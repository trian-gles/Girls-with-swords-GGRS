using Godot;
using System;
using System.Collections.Generic;

public class Player : Node2D
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
    public int speed = 4;

    [Export]
    public int jumpForce = 7;

    [Export]
    public int gravityDenom = 5;

    [Export]
    public bool dummy = false;

    public InputHandler inputHandler;

    // All of these will be stored in gamestate
    private int health = 100;
    public Vector2 velocity = new Vector2(0, 0);
    public int gravityPos = 0;
    public bool facingRight = true;
    public bool grounded;
    private int combo = 0;

    public struct PlayerState
    {
        public List<List<char[]>> inputBuffer { get; set; }
        public List<char> heldKeys { get; set; }
        public List<char[]> unhandledInputs { get; set; }
        public string currentState { get; set; }
        public bool hitConnect { get; set; }
        public int frameCount { get; set; }
        public int stunRemaining { get; set; }

        public bool flipH { get; set; }
        public int health { get; set; }
        public int[] position { get; set; }
        public int gravityPos { get; set; }
        public int[] velocity { get; set; }
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
    private Sprite sprite;
    

    public override void _Ready()
    {
        hitBoxes = GetNode<Area2D>("HitBoxes");
        hurtBoxes = GetNode<Area2D>("HurtBoxes");
        colBox = GetNode<CollisionShape2D>("CollisionBox");
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        sprite = GetNode<Sprite>("Sprite");
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
        pState.hitConnect = currentState.hitConnect;
        pState.stunRemaining = currentState.stunRemaining;
        pState.flipH = sprite.FlipH;

        pState.health = health;
        pState.position = new int[] { (int)Position.x, (int)Position.y };


        pState.velocity = new int[] { (int)velocity.x, (int)velocity.y };
        pState.gravityPos = gravityPos;
        pState.facingRight = facingRight;
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
        currentState.hitConnect = pState.hitConnect;
        currentState.frameCount = pState.frameCount;
        animationPlayer.SetAnimationAndFrame(pState.currentState, pState.frameCount);
        currentState.stunRemaining = pState.stunRemaining;
        sprite.FlipH = pState.flipH;

        health = pState.health;
        Position = new Vector2(pState.position[0], pState.position[1]);
        GD.Print($"setting {Name} position to {Position}");
        gravityPos = pState.gravityPos;
        velocity = new Vector2(pState.velocity[0], pState.velocity[1]);
        facingRight = pState.facingRight;
        grounded = pState.grounded;
        combo = pState.combo;
        EmitSignal(nameof(ComboChanged), Name, combo);

    }

    public class InputHandler 
    {
        public List<List<char[]>> inputBuffer = new List<List<char[]>>();
        public List<char> heldKeys = new List<char>();
        public List<char[]> unhandledInputs = new List<char[]>();

        public void setUnhandledInputs(List<char[]> thisFrameInputs) 
        {
            unhandledInputs = thisFrameInputs;
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

    public void FrameAdvance() 
    {
        bool hitStop = false;
        Update(); //Redraw
        inputHandler.FrameAdvance(hitStop, currentState);
        animationPlayer.FrameAdvance();

        if (CheckHurtRect())
        {
            currentState.InHurtbox();
        }
        currentState.FrameAdvance();

        MoveSlideDeterministicOne();
    }

    private void MoveSlideDeterministicOne()
    {
        int xChange = (int)Math.Floor(velocity.x / 2);
        int yChange = (int)Math.Floor(velocity.y / 2);
        Position += new Vector2(xChange, yChange);
        CorrectPositionBounds();
    }

    public void MoveSlideDeterministicTwo()
    {
        int xChange = (int)Math.Ceiling(velocity.x / 2);
        int yChange = (int)Math.Ceiling(velocity.y / 2);
        Position += new Vector2(xChange, yChange);
        CorrectPositionBounds();
    }

    private void CorrectPositionBounds()
    {
        if (Position.y > 230)
        {
            Position = new Vector2(Position.x, 230);
            grounded = true;
        }

        if (Position.x > 475)
        {
            Position = new Vector2(475, Position.y);
        }
        else if (Position.x < 5)
        {
            Position = new Vector2(5, Position.y);
        }
    }

    public bool CheckTouchingWall()
    {
        if (Position.x > 474 || Position.x < 6)
        {
            return true;
        }
        else
        {
            return false;
        }
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
        if (otherPlayer.CheckTouchingWall())
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

    public bool CheckHurtRect()
    {
        List<Rect2> myRects = GetRects(hurtBoxes, true);
        List<Rect2> otherRects = otherPlayer.GetRects(otherPlayer.hitBoxes, true);
        foreach (Rect2 hurtRect in myRects)
        {
            foreach (Rect2 hitRect in otherRects)
            {
                if (hurtRect.Intersects(hitRect))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public List<Rect2> GetRects(Area2D area, bool globalPosition = false) 
    {
        List<Rect2> allRects = new List<Rect2>();
        foreach (CollisionShape2D colShape in area.GetChildren()) 
        {
            if (!colShape.Disabled)
            {
                allRects.Add(GetRect(colShape, globalPosition));
            }
            
        }
        return allRects;
    }

    public Rect2 GetRect(CollisionShape2D colShape, bool globalPosition = false) 
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
        if (globalPosition)
        {
            position += Position;
        }
        return new Rect2(position, extents);
    }

    public Rect2 GetCollisionRect()
    {
        return GetRect(colBox);
    }

    public void DebugDisplay()
    {
        GetNode<Label>("DebugPos").Text = Position.ToString();
    }
    public override void _Draw()
    {
        return;
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
