using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

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

    protected List<NormalGatling> normalGatlings = new List<NormalGatling>();
    protected List<CommandGatling> commandGatlings = new List<CommandGatling>();
    protected delegate bool RequiredConditionCallback();
    protected delegate void PostInputCallback();
    public override void _Ready()
    {
        owner = GetOwner<Player>();
    }

    public virtual void Load(Dictionary<string, int> loadData)
    {

    }

    public virtual Dictionary<string, int> Save()
    {
        return new Dictionary<string, int>();
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

    
    protected struct NormalGatling
    {
        public char[] input;
        public string state;
        public RequiredConditionCallback reqCall; //if this returns true, we can enter the specified state
        public PostInputCallback postCall;
    }

    protected struct CommandGatling
    {
        public List<char[]> inputs;
        public string state;
        public RequiredConditionCallback reqCall; //if this returns true, we can enter the specified state
        public PostInputCallback postCall;
    }

    protected char[] ReverseInput(char[] inp)
    {
        char[] newInp = new char[2];

        inp.CopyTo(newInp, 0);

        if (inp[0] == '4')
        {
            newInp[0] = '6';

        }

        else if (inp[0] == '6')
        {
            newInp[0] = '4';
        }

        return newInp;
    }

    protected List<char[]> ReverseInputs(List<char[]> origInputs)
    {
        var newInputs = new List<char[]>();
        foreach (char[] inp in origInputs)
        {
            newInputs.Add(ReverseInput(inp));
        }

        return newInputs;
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

    protected void AddGatling(char[] input, RequiredConditionCallback reqCall, string state)
    {
        var newGatling = new NormalGatling
        {
            input = input,
            state = state,
            reqCall = reqCall
        };
        normalGatlings.Add(newGatling);
    }

    protected void AddGatling(char[] input, string state, PostInputCallback postCall)
    {
        var newGatling = new NormalGatling
        {
            input = input,
            state = state,
            postCall = postCall
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

    protected void AddGatling(List<char[]> inputs, string state, PostInputCallback postCall)
    {
        var newGatling = new CommandGatling
        {
            inputs = inputs,
            state = state,
            postCall = postCall
        };
        commandGatlings.Add(newGatling);
    }
    public virtual void HandleInput(char[] inputArr)
    {
        foreach (CommandGatling comGat in commandGatlings)
        {
            char[] firstInp = comGat.inputs[comGat.inputs.Count - 1];
            if (!owner.facingRight)
            {
                firstInp = ReverseInput(firstInp);
            }

            if (Enumerable.SequenceEqual(firstInp, inputArr))
            {
                List<char[]> testedInputs = comGat.inputs;

                if (!owner.facingRight)
                {
                    testedInputs = ReverseInputs(testedInputs);
                }


                if (owner.CheckBufferComplex(testedInputs))
                {
                    if (comGat.reqCall != null)
                    {
                        if (!comGat.reqCall())
                        {
                            continue;
                        }
                    }

                    if (comGat.postCall != null)
                    {
                        comGat.postCall();
                    }

                    EmitSignal(nameof(StateFinished), comGat.state);
                    return;
                }
            }
        }
        foreach (NormalGatling normGat in normalGatlings)
        {

            char[] testInp = normGat.input;
            testInp = ReverseInput(testInp);
            if (Enumerable.SequenceEqual(normGat.input, inputArr))
            {
                if (normGat.reqCall != null)
                {
                    if (!normGat.reqCall())
                    {
                        continue;
                    }
                }

                if (normGat.postCall != null)
                {
                    normGat.postCall();
                }

                EmitSignal(nameof(StateFinished), normGat.state);
                return;
            }
        }
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

    /// <summary>
    /// Determines which hitconfirm state to enter
    /// </summary>
    /// <param name="knockdown"></param>
    /// <param name="launch"></param>
    protected virtual void EnterHitState(bool knockdown, Vector2 launch)
    {
        bool launchBool = false;
        bool airState = (launchBool || !owner.grounded);
        owner.ComboUp();
        if (!(launch == Vector2.Zero)) // LAUNCH NEEDS MORE WORK
        {
            //GD.Print("Launch is not zero!");
            owner.velocity = launch;
            launchBool = true;
        }

        if (launchBool && !knockdown)
        {
            EmitSignal(nameof(StateFinished), "Float");
        }
        else if (airState && knockdown)
        {
            EmitSignal(nameof(StateFinished), "AirKnockdown");
        }
        else if (!airState && knockdown)
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
        owner.velocity = new Vector2(0, 0);
        if (!rightAttack)
        {
            launch.x *= -1;
            hitPush *= -1;
        }
        

        owner.hitPushRemaining = hitPush;
        //GD.Print($"Setting hitPush in {Name} to {owner.hitPushRemaining}");

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
                    EnterHitState(knockdown, launch);
                }
            }
            else
            {
                EnterHitState(knockdown, launch);
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
                    EnterHitState(knockdown, launch);
                }
            }
            else
            {
                EnterHitState(knockdown, launch);
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
                EnterHitState(knockdown, launch);
            }
        }
    }

    public virtual void receiveStun(int hitStun, int blockStun)
    {
        stunRemaining = hitStun;
    }

    public virtual void receiveDamage(int dmg)
    {
        owner.DeductHealth(dmg);
    }
}
