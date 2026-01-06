using Godot;
using System;
using System.Collections.Generic;
using static GameScene;

class SyncTestManager : StateManager
{

	public bool broken = false;
	/// <summary>
	/// Yeah Enqueue is O(N) but whatever nerds...
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class FixedSizedQueue<T>
	{
		List<T> q = new List<T>();

		public int Limit { get; set; }
		public FixedSizedQueue(int limit) => Limit = limit;

			
		public void Enqueue(T obj)
		{
			q.Add(obj);
			{
				while (q.Count > Limit)
					q.RemoveAt(0);
			}
		}

		public T this[int index]
		{
			get
			{
				return q[index];
			}
			
		}
		public bool Full()
		{
			return (q.Count == Limit);
		}
	}

	[Export]
	public int DEPTH = 3;

	[Export]
	public bool trainingMode = false;

	[Export]
	public bool doubleSpeed = false;

	[Export]
	public bool replayFile = false;



	public FixedSizedQueue<byte[]> serializedStates;
	public FixedSizedQueue<int[]> pastInputs;
	public FixedSizedQueue<bool> pastInputAcceptance;

	private bool randomInputs = true;
	private Random random;

	[Export]
	private int logFrame = 0;
	

	public override void _Ready()
	{
		base._Ready();
		serializedStates = new FixedSizedQueue<byte[]>(DEPTH + 1);
		pastInputAcceptance = new FixedSizedQueue<bool>(DEPTH + 1);
		pastInputs = new FixedSizedQueue<int[]>(DEPTH + 1);
		Globals.logOn = false;

		

		if (randomInputs)
		{
			random = new Random();
		}
			

	}

	public override void _Input(InputEvent @event)
	{
		if (trainingMode)
			HandleSpecialInputs(@event);
	}

	private int[] GetTrainingModeInputs()
	{
		int p1Inputs;
		int p2Inputs;
		int playerInputs = GetInputs("");
		int otherInputs = 0;

		if (recordingInputs)
			recordedInputs.Add(playerInputs);

		if (playbackInputs)
		{
			if (inputHead < recordedInputs.Count)
			{
				otherInputs = recordedInputs[inputHead];
			}
			else
			{
				StopInputPlayback();
			}
		}


		if (flippedPlayers)
		{
			p1Inputs = otherInputs;
			p2Inputs = playerInputs;
		}
		else
		{
			p1Inputs = playerInputs;
			p2Inputs = otherInputs;
		}

		if (recordingInputs || playbackInputs)
			inputHead++;

		return new int[] { p1Inputs, p2Inputs };
	}

	private int lastGen0Collections = 0;
	private int lastGen1Collections = 0;
	private int lastGen2Collections = 0;

	public override void _Process(float delta)
	{
		int gen0 = GC.CollectionCount(0);
		int gen1 = GC.CollectionCount(1);
		int gen2 = GC.CollectionCount(2);

		if (gen0 != lastGen0Collections)
		{
			GD.Print($"Gen0 collections: {gen0 - lastGen0Collections}");
			lastGen0Collections = gen0;
		}
		if (gen1 != lastGen1Collections)
		{
			GD.Print($"Gen1 collections: {gen1 - lastGen1Collections}");
			lastGen1Collections = gen1;
		}
		if (gen2 != lastGen2Collections)
		{
			GD.Print($"Gen2 collections: {gen2 - lastGen2Collections}");
			lastGen2Collections = gen2;
		}

		long totalMemory = GC.GetTotalMemory(false);
		GD.Print($"Allocated memory: {totalMemory / 1024} KB");
	}

	public override void _PhysicsProcess(float _delta)
	{
		RunFrameLoop();
		if (doubleSpeed)
			RunFrameLoop();
	}

	public void RunFrameLoop()
	{
		

		int[] combinedInps;
		Globals.frame++;
		if (Globals.frame == logFrame)
			Globals.logOn = true;
		Globals.rollbackFrame = 0;
		if (readyForChange && --waitBeforeChangeFrames < 0)
		{
			StartNextGame();
			readyForChange = false;
		}


		if (currGame.AcceptingInputs())
		{
			if (matchFilename != "" && currGame.Name == "GameScene")
				combinedInps = GetMatchInputs();
			else if (randomInputs)
				combinedInps = GetRandomInputs();
			else if (trainingMode)
				combinedInps = GetTrainingModeInputs();
			else
				combinedInps = new int[] { GetInputs(""), GetInputs("b") };
		}
		else
			combinedInps = new int[] { 0, 0 };

		Globals.Log($"Sync test on frame {Globals.frame}");
		
		currGame.GGRSAdvanceFrame(combinedInps[0], combinedInps[1]);
		return;
		byte[] serializedGamestate = currGame.SaveState(Globals.frame);
		serializedStates.Enqueue(serializedGamestate);
		pastInputs.Enqueue(combinedInps);
		pastInputAcceptance.Enqueue(currGame.AcceptingInputs());
		

		if (!serializedStates.Full()) // we haven't accrued enough states to rollback
			return;

		if (!pastInputAcceptance[0]) // as this frame was not accepting inputs, we do not need to and should not test rolling back from it
		{
			return;
		}
		

		currGame.LoadState(Globals.frame - (DEPTH), serializedStates[0], 0);
		Globals.frame = Globals.frame - (DEPTH);
		for (int i = 1; i < DEPTH + 1; i++)
		{
			int[] tempInputs = pastInputs[i];
			Globals.frame++;
			Globals.rollbackFrame = i;
			currGame.GGRSAdvanceFrame(tempInputs[0], tempInputs[1]);
		}

		if (!currGame.CompareStates(serializedGamestate) && !broken){
			gameScene.WriteLogs();
			broken = true;
		}
	}

	public override void OnCharactersSelected(int playerOne, int playerTwo, int colorOne, int colorTwo, int bkgIndex)
	{
		base.OnCharactersSelected(playerOne, playerTwo, colorOne, colorTwo, bkgIndex);
		ReadyForChange(GameType.GAME);
	}

	public override void OnGameWon(string winner, int character)
	{
		GD.Print("Game won");
		OnReselectChar();
	}

	private int[] randInputs = new[] { 0, 0 };
	private int[] GetRandomInputs()
	{
		randInputs[0] = 0;
		randInputs[1] = random.Next(255);
		return randInputs;
	}
}
