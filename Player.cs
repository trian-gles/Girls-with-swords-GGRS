using Godot;
using System;

public class Player : KinematicBody2D
{
    private Node currentState;
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



    public void FrameAdvance() 
    {
        velocity.y += gravity;
        MoveAndSlide(velocity, Vector2.Up);
    }
}
