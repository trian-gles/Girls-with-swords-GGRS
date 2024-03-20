using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class Oki : BehaviourState
{

    private Random random = new Random();
    public override int Poll(GameStateObjectRedesign.GameState state)
    {
        return 0;
    }

    public override string GetNextState(GameStateObjectRedesign.GameState state)
    {
        if (random.Next(2) == 1)
            return "Zone";
        else
            return "Chase";
    }
}