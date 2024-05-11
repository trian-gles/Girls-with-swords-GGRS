using Godot;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Linq;



public abstract class BaseGame : Node2D
{

	protected Label HUDText;
	protected Label inputText;
	protected Label inputTextP2;

	/// <summary>
	/// Used only by local game modes
	/// </summary>
	/// <param name="p1Inps"></param>
	/// <param name="p2Inps"></param>
	public virtual void AdvanceFrame(int p1Inputs, int p2Inputs){}

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
				// GD.Print("TEST");
				// GD.Print(node);
				((CanvasItem) node).Visible = false;
			}
		}
	}

	public void ShowAll(Node root=null)
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
				// GD.Print("TEST");
				// GD.Print(node);
				((CanvasItem)node).Visible = true;
			}
		}
	}

	public virtual void Reset() { }

	public void ChangeHUDText(string msg) {
		HUDText.Text = msg;
	}

	// ----------------
	// Private methods
	// ----------------

	protected byte[] Serialize<T>(T data)
	where T : struct
	{
		var formatter = new BinaryFormatter();
		var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		return stream.ToArray();
	}
	protected T Deserialize<T>(byte[] array)
		where T : struct
	{
		var stream = new MemoryStream(array);
		var formatter = new BinaryFormatter();
		return (T)formatter.Deserialize(stream);
	}

	protected void CompareValues(int valueA, int valueB, string name)
	{
		if (valueA != valueB)
		{
			GD.Print($"{name} does not match! new: {valueA}, old: {valueB}");
		}
	}

	protected void CompareValues(bool valueA, bool valueB, string name)
	{
		if (valueA != valueB)
		{
			GD.Print($"{name} does not match! new: {valueA}, old: {valueB}");
		}
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
	public virtual void CompareStates(byte[] serializedOldState)
	{

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
}

