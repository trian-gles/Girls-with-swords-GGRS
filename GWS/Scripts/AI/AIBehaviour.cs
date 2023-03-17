using Godot;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Linq;


public class AIBehaviour
{
    public int lastInp = 0;
    private BehaviourState behaviour;
    private Dictionary<string, BehaviourState> behaviourStates = new Dictionary<string, BehaviourState>
    {
        {"Abare", new Abare() },
        {"Combo", new Combo(new List<int>{ 32, 64, 66 }, new List<int>{1, 36, 60 }) }
    };

    public AIBehaviour()
    {
        behaviour = behaviourStates["Abare"];
        behaviour.Init(this);
    }
    public int Poll(GameStateObjectRedesign.GameState state)
    {
        string nextState = behaviour.GetNextState(state);
        if (nextState != "")
            EnterState(nextState);
        int input = behaviour.Poll(state);
        
        lastInp = input;
        return input;
    }

    private void EnterState(string nextState)
    {
        behaviour.Exit();
        behaviour = behaviourStates[nextState];
        behaviour.Enter();
    }

}