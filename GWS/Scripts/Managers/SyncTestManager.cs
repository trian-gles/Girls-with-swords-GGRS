using Godot;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using static GameScene;

class SyncTestManager : StateManager
{

	public bool broken = false;
	public sealed class FixedSizedQueue<T>
	{
		private readonly T[] _buffer;
		private int _head;   // index of oldest element
		private int _tail;   // index for next write
		private int _count;

		public int Capacity { get; }
		public int Count { get { return _count; } }
		public bool IsEmpty { get { return _count == 0; } }

		public FixedSizedQueue(int capacity)
		{
			if (capacity <= 0)
				throw new ArgumentOutOfRangeException(nameof(capacity));

			Capacity = capacity;
			_buffer = new T[capacity];
		}

		/// <summary>
		/// Returns true if the queue is saturated (full).
		/// </summary>
		public bool Full()
		{
			return _count == Capacity;
		}

		/// <summary>
		/// Enqueues an item. If the queue is full, the oldest item is overwritten.
		/// </summary>
		public void Enqueue(T item)
		{
			_buffer[_tail] = item;
			_tail = (_tail + 1) % Capacity;

			if (_count == Capacity)
			{
				_head = (_head + 1) % Capacity;
			}
			else
			{
				_count++;
			}
		}

		/// <summary>
		/// Removes and returns the oldest item.
		/// </summary>
		public T Dequeue()
		{
			if (_count == 0)
				throw new InvalidOperationException("Queue is empty.");

			T item = _buffer[_head];
			_buffer[_head] = default(T);
			_head = (_head + 1) % Capacity;
			_count--;
			return item;
		}

		public T Peek()
		{
			if (_count == 0)
				throw new InvalidOperationException("Queue is empty.");

			return _buffer[_head];
		}

		/// <summary>
		/// Gets the element at the specified index (0 = oldest).
		/// </summary>
		public T this[int index]
		{
			get
			{
				if (index < 0 || index >= _count)
					throw new ArgumentOutOfRangeException(nameof(index));

				int actualIndex = _head + index;
				if (actualIndex >= Capacity)
					actualIndex -= Capacity;

				return _buffer[actualIndex];
			}
		}

		public void Clear()
		{
			if (_count > 0)
			{
				Array.Clear(_buffer, 0, Capacity);
				_head = 0;
				_tail = 0;
				_count = 0;
			}
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
	public FixedSizedQueue<CombinedInputs> pastInputs;
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
		pastInputs = new FixedSizedQueue<CombinedInputs>(DEPTH + 1);
		Globals.logOn = false;

		

		if (randomInputs)
		{
			random = new Random();
		}
			

	}
	long prevMem = 0;
	public override void _Process(float delta)
	{
		base._Process(delta);
		long mem = GC.GetTotalMemory(false); // allocated managed memory
		if (mem > prevMem)
			GD.Print(mem / 1024);
		prevMem = mem;
		
	}

	public override void _Input(InputEvent @event)
	{
		if (trainingMode)
			HandleSpecialInputs(@event);
	}

	public override void _PhysicsProcess(float _delta)
	{
		RunFrameLoop();
		if (doubleSpeed)
			RunFrameLoop();
	}
	
	public void RunFrameLoop()
	{
		CombinedInputs combinedInps = new CombinedInputs();
		Globals.frame++;
		Globals.rollbackFrame = 0;
		if (readyForChange && --waitBeforeChangeFrames < 0)
		{
			StartNextGame();
			readyForChange = false;
		}


		if (currGame.AcceptingInputs())
		{
			if (matchFilename != "" && currGame.Name == "GameScene")
				GetMatchInputs();
			else if (randomInputs)
			{
				combinedInps.SetInputs(GetInputs(""), random.Next(255));
			}
			else
				combinedInps.SetInputs(GetInputs(""), GetInputs("b"));
		}
		else
			combinedInps.SetInputs(0, 0);

		if (Globals.logOn)
			Globals.Log($"Sync test on frame {Globals.frame}");

		long startmem = Globals.TestGC1();
		currGame.GGRSAdvanceFrame(combinedInps.p1Inps, combinedInps.p2Inps);
		Globals.TestGC2(startmem, "Initial game frame");
		
		startmem = Globals.TestGC1();
		byte[] serializedGamestate = currGame.SaveState(Globals.frame);
		
		serializedStates.Enqueue(serializedGamestate);
		pastInputs.Enqueue(combinedInps);
		pastInputAcceptance.Enqueue(currGame.AcceptingInputs());
		Globals.TestGC2(startmem, "Saving");
        


        if (!serializedStates.Full()) // we haven't accrued enough states to rollback
			return;

		if (!pastInputAcceptance[0]) // as this frame was not accepting inputs, we do not need to and should not test rolling back from it
		{
			return;
		}
		startmem = Globals.TestGC1();
		currGame.LoadState(Globals.frame - (DEPTH), serializedStates[0], 0);
		Globals.TestGC2(startmem, "Loading");
        
        Globals.frame = Globals.frame - (DEPTH);
		startmem = Globals.TestGC1();
		for (int i = 1; i < DEPTH + 1; i++)
		{
			CombinedInputs tempInputs = pastInputs[i];
			Globals.frame++;
			Globals.rollbackFrame = i;
			currGame.GGRSAdvanceFrame(tempInputs.p1Inps, tempInputs.p2Inps);
		}
		Globals.TestGC2(startmem, "Resimulation");
        return;

        //if (Globals.frame == 1200)
        //	gameScene.WriteLogs();
        startmem = Globals.TestGC1();
		if (!currGame.CompareStates(serializedGamestate) && !broken){
			gameScene.WriteLogs();
			broken = true;
		}
		Globals.TestGC2(startmem, "Comparison");
	}

	public override void OnCharactersSelected(int playerOne, int playerTwo, int colorOne, int colorTwo, int bkgIndex)
	{
		base.OnCharactersSelected(playerOne, playerTwo, colorOne, colorTwo, bkgIndex);
		ReadyForChange(GameType.GAME);
	}

	public override void OnGameWon(string winner, int character)
	{
		OnReselectChar();
	}
}
