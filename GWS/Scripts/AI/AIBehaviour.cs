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
        {"Combo", new Combo(new List<int>{ 32, 64, 66 }, new List<int>{1, 36, 60 }) },
        {"RandomMash", new RandomMash() },
        {"Chase", new Chase() },
        {"FloatTech", new FloatTech() },
        {"Zone", new Zone() }
    };

    // Global behaviours that must be saved here
    public static HashSet<string> floatStates = new HashSet<string>() { "Float", "WallBounce", "GroundBounce"};

    public AIBehaviour()
    {
        behaviour = behaviourStates["Chase"];
        behaviour.Init(this);
    }
    public int Poll(GameStateObjectRedesign.GameState state)
    {
        string nextState = behaviour.GetNextState(state);

        // Global handling which must be done here
        if (floatStates.Contains(state.P2State.currentState))
            nextState = "FloatTech";
        // 

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