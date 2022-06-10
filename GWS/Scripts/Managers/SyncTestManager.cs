using Godot;
using System;
using System.Collections.Generic;

class SyncTestManager : BaseManager
{
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

	public FixedSizedQueue<byte[]> serializedStates;
	public FixedSizedQueue<int[]> pastInputs;
	public FixedSizedQueue<bool> pastInputAcceptance;

	private int frame = 0;

	public override void _Ready()
	{
		base._Ready();
		serializedStates = new FixedSizedQueue<byte[]>(DEPTH + 1);
		pastInputAcceptance = new FixedSizedQueue<bool>(DEPTH + 1);
		pastInputs = new FixedSizedQueue<int[]>(DEPTH + 1);
	}
	public override void _PhysicsProcess(float _delta)
	{
		int[] combinedInps;

		if (currGame.AcceptingInputs())
			combinedInps = new int[] { GetInputs(""), GetInputs("b") };
		else
			combinedInps = new int[] { 0, 0 };
		
		currGame.AdvanceFrame(combinedInps[0], combinedInps[1]);
		byte[] serializedGamestate = currGame.SaveState(frame);

		serializedStates.Enqueue(serializedGamestate);
		pastInputs.Enqueue(combinedInps);
		pastInputAcceptance.Enqueue(currGame.AcceptingInputs());

		if (!serializedStates.Full()) // we haven't accrued enough states to rollback
			return;

		if (!pastInputAcceptance[0]) // as this frame was not accepting inputs, we do not need to and should not test rolling back from it
		{
			GD.Print("Ignoring rollback as it would break stuff!");
			return;
		}

		currGame.LoadState(frame - (DEPTH), serializedStates[0], 0);

		for (int i = 1; i < DEPTH + 1; i++)
		{
			int[] tempInputs = pastInputs[i];

			currGame.AdvanceFrame(tempInputs[0], tempInputs[1]);
		}
		currGame.CompareStates(serializedGamestate);
	}
}
