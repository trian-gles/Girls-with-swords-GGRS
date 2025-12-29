using Godot;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Linq;


public class BehaviourState
{
    protected AIBehaviour owner;
    protected int frameCount = 0;

    protected Random rng = new Random();


    protected int DoButtonPress(int button)
    {
        if (Globals.aiDifficulty == Globals.AIDIFFICULTY.LO && rng.Next(40) != 1)
            return 0;

        if (button == Globals.KICK && !owner.CanKickWithoutGrabbing())
            return 0;
        if (button == Globals.SLASH && !owner.CanSlashWithoutGrabbing())
            return 0;
        
        if ((owner.lastInp & button) == 0)
            return button;
        else
            return 0;
    }

    protected int DoGrab()
    {
        if (Globals.aiDifficulty == Globals.AIDIFFICULTY.LO && rng.Next(3) != 1)
            return 0;
        else
            return 32 + 64;
    }

    public void Init(AIBehaviour owner)
    {
        this.owner = owner;
    }
    public virtual int Poll(GameStateObjectRedesign.GameState state)
    {
        return 0;
    }

    public virtual void Enter()
    {
        frameCount = 0;
    }

    public virtual void Exit() { }

    public virtual string GetNextState(GameStateObjectRedesign.GameState state) { return ""; }

    protected int GetForwardInput(GameStateObjectRedesign.GameState state)
    {
        if (state.P2State.facingRight)
            return 4;
        else
            return 8;
    }

}