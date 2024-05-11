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
    private string behaviourName;
    private Random random = new Random();
    private Dictionary<string, BehaviourState> behaviourStates = new Dictionary<string, BehaviourState>
    {
        {"WakeupAbare", new WakeupAbare() },
        {"Combo", new Combo() },
        {"RandomMash", new RandomMash() },
        {"Chase", new Chase() },
        {"FloatTech", new FloatTech() },
        {"Zone", new Zone() },
        {"Oki", new Oki() },
        {"WakeupBlock", new WakeupBlock() },
        {"WakeupBackdash", new WakeupBackdash() }
    };

    // Global behaviours that must be saved here
    public static HashSet<string> floatStates = new HashSet<string>() { "Float", "WallBounce", "GroundBounce"};

    public static HashSet<string> groundHitConfirmStates = new HashSet<string>
    {
        "HitStun",
        "Stagger",
        "Block",
        "CrouchBlock"
    };

    public AIBehaviour()
    {
        foreach (var b in behaviourStates.Values)
        {
            b.Init(this);
        }

        behaviour = behaviourStates["Chase"];
        behaviourName = "Chase";
        behaviour.Init(this);
    }
    public int Poll(GameStateObjectRedesign.GameState state)
    {
        string nextState = behaviour.GetNextState(state);

        // Global handling which must be done here
        if (floatStates.Contains(state.P2State.currentState))
            nextState = "FloatTech";

        if (state.P2State.currentState == "Knockdown" && behaviourName.Substr(0, 6) != "Wakeup")
        {
            switch (random.Next(4))
            {
                case 0:
                    nextState = "WakeupAbare";
                    break;
                case 1:
                    nextState = "WakeupBackdash";
                    break;
                default:
                    nextState = "WakeupBlock";
                    break;
            }
            
        }
            
        // 

        if (nextState != "")
        {
            EnterState(nextState);
            
        }
            
        int input = behaviour.Poll(state);
        
        lastInp = input;
        return input;
    }

    private void EnterState(string nextState)
    {
        behaviour.Exit();
        behaviour = behaviourStates[nextState];
        behaviourName = nextState;
        behaviour.Enter();
    }

}