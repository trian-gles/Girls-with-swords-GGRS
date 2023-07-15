using Godot;
using System;
using System.Collections.Generic;

class StateManager : BaseManager
{
	protected const int WAITBEFORECHANGEMAX = 12;
	protected int waitBeforeChangeFrames = WAITBEFORECHANGEMAX;
	protected bool readyForChange = false;
	protected int potentialEndFrame;

	protected void ReadyForChange()
	{
		readyForChange = true;
		waitBeforeChangeFrames = WAITBEFORECHANGEMAX;
	}

}