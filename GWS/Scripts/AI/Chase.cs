using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;

public class Chase : BehaviourState
{
    int distance = 10000;
    SubState subState;
    int direction;


    enum SubState
    {
        ChooseDirection,
        EmptyFrame,
        Dash
    }

    HashSet<string> possibleStates = new HashSet<string>() {"Idle", "Walk", "Run", "PreRun" }; 

    public override void Enter()
    {
        base.Enter();
        subState = SubState.ChooseDirection;
    }

    public override int Poll(GameStateObjectRedesign.GameState state)
    {
        distance = state.P1State.position[0] - state.P2State.position[0];
        if (!possibleStates.Contains(state.P2State.currentState))
            return 0;


        switch (subState)
        {
            case SubState.ChooseDirection:
                return ChooseDirection(distance);
            case SubState.EmptyFrame:
                {
                    GD.Print("EMPTY FRAME");
                    subState = SubState.Dash;
                    return 0;
                }
                
            case SubState.Dash:
                return direction;

        }

        return 0;
    }

    private int ChooseDirection(int distance)
    {
        if (distance < 0)
            direction = 8;
        else
            direction = 4;
        subState = SubState.EmptyFrame;
        return direction;
    }

    public override string GetNextState(GameStateObjectRedesign.GameState state)
    {
        if (Math.Abs(distance) < 4000)
        {
            return "RandomMash";
        }

        return "";
    }
}