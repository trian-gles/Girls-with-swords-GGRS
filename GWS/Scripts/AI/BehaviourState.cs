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

    public void Init(AIBehaviour owner)
    {
        this.owner = owner;
    }
    public virtual int Poll(GameStateObjectRedesign.GameState state)
    {
        return 0;
    }

    public virtual void Enter() {
        frameCount = 0;
    }

    public virtual void Exit() { }

    public virtual string GetNextState(GameStateObjectRedesign.GameState state) { return ""; }

}