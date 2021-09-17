using Godot;
using System;
using System.Collections.Generic;

public class HadoukenPart : Node2D
{
    [Signal]
    public delegate void OnHadoukenOffscreen();

    [Export]
    public int hitStun = 10;

    [Export]
    public Vector2 launch = new Vector2();

    [Export]
    public int hitPush = 0;

    [Export]
    public int dmg = 1;

    [Export]
    public Vector2 speed;

    private bool movingRight;

    private Player targetPlayer;

    private bool active = true; // I use this so that when the hadouken collides with the other player it isn't yet deleted, it just turns invisible and inactive.  For rollback reasons.

    static private int totalHads;

    /// <summary>
    /// Method to be called right after instantiation by the player
    /// </summary>
    /// <param name="movingRight"></param>
    /// <param name="targetPlayer"> the targeted player </param>
    public void Spawn(bool movingRight, Player targetPlayer)
    {
        this.movingRight = movingRight;
        this.targetPlayer = targetPlayer;
        if (!movingRight) 
        {
            GetNode<AnimatedSprite>("AnimatedSprite").FlipH = true;
        }
        Name = "Had" + totalHads.ToString(); // provides a unique name for each hadouken that can be accessed by the gamestateobj
        totalHads += 1;
    }

    [Serializable]
    public struct HadoukenState
    {
        public int[] pos { get; set; }
        public bool active { get; set; }
        public string name { get; set; }

    }
    
    public void FrameAdvance()
    {
        if (movingRight)
        {
            Position += speed;
        }

        else
        {
            Position -= speed;
        }

        if (Position.x > 600 || Position.x < -200) // To ensure the fireball isn't deleted before it could be potentially rolled back, these values are quite high.
        {
            targetPlayer.DeleteHadouken(this); // this shouldn't be done this way, but every possible solution is very inelegant...
        }
        if (active)
        {
            if (CheckRect())
            {
                HurtPlayer();
            }
        }
        
    }

    /// <summary>
    /// checks if the targeted player is inside the collision box
    /// </summary>
    /// <returns></returns>
    private bool CheckRect()
    {
        Rect2 myRect = GetRect(GetNode<CollisionShape2D>("CollisionShape2D"), true);
        List<Rect2> otherRects = targetPlayer.GetRects(targetPlayer.hitBoxes, true);
        foreach (Rect2 pRect in otherRects)
        {
            if (myRect.Intersects(pRect))
            {
                return true;
            }
        }
        return false;
    }

    private void HurtPlayer()
    {
        // fill this with harmful stuff!!!!
        targetPlayer.ReceiveHit(movingRight, dmg, hitStun, State.HEIGHT.MID, hitPush, launch, false);
        MakeInactive();
    }

    private void MakeInactive()
    {
        GetNode<AnimatedSprite>("AnimatedSprite").Visible = false;
        active = false;
    }

    private Rect2 GetRect(CollisionShape2D colShape, bool globalPosition = false)
    {
        RectangleShape2D shape = (RectangleShape2D)colShape.Shape;
        Vector2 extents = shape.Extents * 2;
        Vector2 position;
        if (movingRight)
        {
            position = colShape.Position - extents / 2;
        }
        else
        {
            position = new Vector2(-colShape.Position.x - extents.x / 2, colShape.Position.y - extents.y / 2);
        }
        if (globalPosition)
        {
            position += Position;
        }
        return new Rect2(position, extents);
    }

    public HadoukenState GetState()
    {
        HadoukenState hadState = new HadoukenState();
        hadState.pos = new int[] {(int) Position.x, (int) Position.y};
        hadState.active = active;
        hadState.name = Name;
        return hadState;
    }

    public void SetState(HadoukenState newState) 
    {
        Position = new Vector2(newState.pos[0], newState.pos[1]);
        active = newState.active;
        GetNode<AnimatedSprite>("AnimatedSprite").Visible = active;
    }

    
}
