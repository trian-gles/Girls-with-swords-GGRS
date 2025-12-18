using Godot;
using System;
using System.Collections.Generic;

class StateManager : BaseManager
{
	protected const int WAITBEFORECHANGEMAX = 12;
	protected int waitBeforeChangeFrames = WAITBEFORECHANGEMAX;
	protected bool readyForChange = false;
	protected int potentialEndFrame;
	protected enum GameType
    {
		CHARSELECT,
		GAME,
		WIN
    }

	protected GameType nextGameType;

	protected void ReadyForChange(GameType gameType)
	{
		readyForChange = true;
		waitBeforeChangeFrames = WAITBEFORECHANGEMAX;
		nextGameType = gameType;
	}
	
	protected void StartNextGame()
	{
		switch (nextGameType)
		{
			case GameType.GAME:
				base.OnNewGame();
				break;
			case GameType.CHARSELECT:
				base.OnReselectChar();
				break;
			case GameType.WIN:
				
				break;
		}
		
	}

}