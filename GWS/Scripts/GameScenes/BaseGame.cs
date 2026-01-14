using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Newtonsoft.Json;
using System.Runtime.InteropServices;



public abstract class BaseGame : Node2D
{

	protected Control HUDText;
	protected Label inputText;
	protected Label inputTextP2;

	public BaseManager manager;
	
	

	/// <summary>
	/// Used only by local game modes
	/// </summary>
	/// <param name="p1Inps"></param>
	/// <param name="p2Inps"></param>
	public virtual void AdvanceFrame(int p1Inputs, int p2Inputs) { }

	/// <summary>
	/// Used for time based changes not called during rollbacks (such as visual and audio effects)
	/// </summary>
	public virtual void TimeAdvance()
	{

	}

	public void HideAll()
	{
		var queue = new Queue<Node>();
		queue.Enqueue(this);
		while (queue.Count > 0)
		{

			var node = queue.Dequeue();
			foreach (Node child in node.GetChildren())
			{
				queue.Enqueue(child);
			}
			if (node.GetType().GetProperty("Visible") != null)
			{
				node.Set("visible", false);
			}
		}
	}

	public void ShowAll(Node root = null)
	{
		var queue = new Queue<Node>();
		if (root != null)
		{
			queue.Enqueue(root);
		}
		else
		{
			queue.Enqueue(this);
		}

		while (queue.Count > 0)
		{

			var node = queue.Dequeue();
			foreach (Node child in node.GetChildren())
			{
				queue.Enqueue(child);
			}
			if (node.GetType().GetProperty("Visible") != null)
			{
				((CanvasItem)node).Visible = true;
			}
		}
	}

	public virtual void Reset() { }

	public void ChangeHUDText(string msg)
	{
		HUDText.Visible = true;
		HUDText.Call("set_text", msg);
	}

	// ----------------
	// Private methods
	// ----------------
	private BinaryFormatter formatter = new BinaryFormatter();
	MemoryStream stream = new MemoryStream();
	private long maxLen = 0;
	
	protected byte[] Serialize<T>(T data)
		where T : struct
	{
		stream.Position = 0;
		stream.SetLength(0);
		formatter.Serialize(stream, data);
		return stream.ToArray();
	}
	protected T Deserialize<T>(byte[] array)
		where T : struct
	{
		stream.Position = 0;
		stream.SetLength(0);
		stream.Write(array, 0, array.Length);
		stream.Position = 0;
		return (T)formatter.Deserialize(stream);
	}
	
	

	protected bool CompareValues(int valueA, int valueB, string name)
	{
		if (valueA != valueB)
		{
			GD.Print($"{name} does not match! new: {valueA}, old: {valueB}");
		}

		return (valueA == valueB);

	}

	protected bool CompareValues(bool valueA, bool valueB, string name)
	{
		if (valueA != valueB)
		{
			GD.Print($"{name} does not match! new: {valueA}, old: {valueB}");
		}

		return (valueA == valueB);
	}


	// ----------------
	// For GGRS and SyncTesting
	// ----------------
	public virtual byte[] SaveState(int frame)
	{
		return new byte[] { 0 };
	}

	public virtual void LoadState(int frame, byte[] buffer, int checksum)
	{

	}

	public virtual void GGRSAdvanceFrame(int p1Inps, int p2Inps)
	{
		AdvanceFrame(p1Inps, p2Inps);
	}

	/// <summary>
	/// Used for Synctesting to compare whether the loaded state matches the new one
	/// </summary>
	/// <param name="serializedNewState"></param>
	public virtual bool CompareStates(byte[] serializedOldState)
	{
		return true;
	}

	/// <summary>
	/// Give the game control of whether it accepts inputs.  This is necessary to avoid unneccesary rollbacks.
	/// </summary>
	/// <returns></returns>
	public virtual bool AcceptingInputs()
	{
		return true;
	}

	/// <summary>
	/// Again, gives the game control.  Used instead of a signal.
	/// </summary>
	/// <returns></returns>
	public virtual bool IsFinished()
	{
		return false;
	}

	protected bool AnyButtonPressed(int inputs, int playerLastFrameInputs)
	{
		foreach (int num in new[] { 16, 32, 64, 128 })
		{
			if ((inputs & num) != 0 && (playerLastFrameInputs & num) == num)
				return true;
		}
		return false;
	}
}

