using Godot;
using System;

/// <summary>
/// Base class for all states
/// </summary>
public abstract class State : Node
{
    public Player owner;
    public int frameCount
    { get; set; }
    [Signal]
    public delegate void StateFinished(string nextStateName);

    public int stunRemaining 
    { get; set; }
    public bool loop = false;

    public bool hitConnect = false;

    public enum HEIGHT
    {
        LOW,
        MID,
        HIGH
    }
    public override void _Ready()
    {
        owner = GetOwner<Player>();
    }

    /// <summary>
    /// Called right when switching into this state.  NOT called when a game state is loaded
    /// </summary>
    public virtual void Enter() 
    {
        frameCount = 0;
    }

    /// <summary>
    /// Called right when exiting this state.  NOT called when a game state is loaded
    /// </summary>
    public virtual void Exit()
    {
        hitConnect = false;
    }

    protected void ApplyGravity()
    {
        owner.velocity.y += owner.gravity;
    }
    public virtual void AnimationFinished() 
    { 
    
    }

    public virtual void HandleInput(char[] inputArr)
    {
        GD.Print(inputArr);
    }

    /// <summary>
    /// Just advances the frameCount, please make a base. call anyways though!
    /// </summary>
    public virtual void FrameAdvance()
    {
        frameCount++;
    }

    /// <summary>
    /// Get pushed by the opposing player from pure movement
    /// </summary>
    /// <param name="xVel"></param>
    public virtual void PushMovement(float xVel) 
    {
        owner.velocity.x = xVel / 2;
    }

    /// <summary>
    /// Called if the other player is found in this hurtbox
    /// </summary>
    public virtual void InHurtbox()
    {


    }

    protected void EnterHitState(bool knockdown, bool launch)
    {
        if (launch)
        {
            EmitSignal(nameof(StateFinished), "Float");
        }
        else if (knockdown)
        {
            EmitSignal(nameof(StateFinished), "Knockdown");
        }
        else
        {
            EmitSignal(nameof(StateFinished), "HitStun");
        }
    }

    public virtual void ReceiveHit(bool rightAttack, HEIGHT height, int hitPush, Vector2 launch, bool knockdown)
    {
        GD.Print($"Received attack on side {rightAttack}");
        bool launchBool = false;
        if (!rightAttack)
        {
            launch.x *= -1;
            hitPush *= -1;
        }
        if (!(launch == Vector2.Zero)) // LAUNCH NEEDS MORE WORK
        {
            GD.Print("Launch is not zero!");
            owner.velocity = launch;
            launchBool = true;
        }

        owner.hitPushRemaining = hitPush;

        if (owner.velocity.y < 0)
        {
            owner.grounded = false;
        }
        if (height == HEIGHT.HIGH) 
        {
            if (!owner.CheckHeldKey('2'))
            {
                if ((rightAttack && owner.CheckHeldKey('6')) || (!rightAttack && owner.CheckHeldKey('4')))
                {
                    EmitSignal(nameof(StateFinished), "Block");
                }
                else
                {
                    EnterHitState(knockdown, launchBool);
                }
            }
            else
            {
                EnterHitState(knockdown, launchBool);
            }
            
        }
        else if (height == HEIGHT.LOW) 
        {
            if (owner.CheckHeldKey('2') && owner.grounded)
            {
                if ((rightAttack && owner.CheckHeldKey('6')) || (!rightAttack && owner.CheckHeldKey('4')))
                {
                    EmitSignal(nameof(StateFinished), "CrouchBlock");
                }
                else
                {
                    EnterHitState(knockdown, launchBool);
                }
            }
            else
            {
                EnterHitState(knockdown, launchBool);
            }
        }
        else
        {
            if ((rightAttack && owner.CheckHeldKey('6')) || (!rightAttack && owner.CheckHeldKey('4'))) 
            {
                EmitSignal(nameof(StateFinished), "Block");
            }
            else 
            {
                EnterHitState(knockdown, launchBool);
            }
        }
    }

    public void receiveStun(int stun)
    {
        stunRemaining = stun;
    }

    public virtual void receiveDamage(int dmg)
    {
        owner.DeductHealth(dmg);
    }
}
