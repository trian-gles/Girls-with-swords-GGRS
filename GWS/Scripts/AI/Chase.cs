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
	Intent intent;
	int direction;
	Random rng = new Random();


	enum SubState
	{
		ChooseDirection,
		EmptyFrame,
		Dash
	}

	enum Intent
	{
		Grab,
		Kick,
		Punch,
		Slash
	}

	HashSet<string> possibleStates = new HashSet<string>() {"Idle", "Walk", "Run", "PreRun" };

	public override void Enter()
	{
		base.Enter();
		subState = SubState.ChooseDirection;
		intent = (Intent)(rng.Next() % 4);
	}

	public override int Poll(GameStateObjectRedesign.GameState state)
	{
		distance = state.P1State.positionx - state.P2State.positionx;

		if (state.P2State.stunRemaining == 1)
			intent = (Intent)((int)intent + 1 % 4);

		if (!(owner.p2Tags.Contains(Globals.Tags.idle) || owner.p2Tags.Contains(Globals.Tags.movestate)))
			return 0;


		switch (subState)
		{
			case SubState.ChooseDirection:
				return ChooseDirection(distance);
			case SubState.EmptyFrame:
				{
					subState = SubState.Dash;
					return 0;
				}
				
			case SubState.Dash:
			{
				if (rng.Next() % 32 == 0)
					return 1;
				return direction + Globals.DASH;
			}
				

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

		int minDist = 2000;
		if (intent == Intent.Punch)
			minDist = 4000;
		else if (intent == Intent.Kick)
			minDist = 5000;
		else if (intent == Intent.Slash)
			minDist = 7000;
		if (Math.Abs(distance) < minDist)
			{
				return "RandomMash";
			}

		return "";
	}
}
