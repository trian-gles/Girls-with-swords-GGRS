using Godot;
using System;

public class EventScheduler : Node
{
	public enum EventType
	{
		AUDIO,
		GRAPHIC
	}

	private struct ScheduledEvent
	{
		public string name;
		public EventType type;
		public int scheduledFrame;
		public int creationFrame;
		public bool active;
	}

	private const int MaxEvents = 10;

	private ScheduledEvent[] events = new ScheduledEvent[MaxEvents];
	private int eventCount = 0;

	private CharacterAudio audioPlay;

	[Export]
	public int frameDelay = 4;

	public override void _Ready()
	{
		audioPlay = GetNode<CharacterAudio>("CharacterAudioHandler");
	}

	/// <summary>
	/// Schedule a GFX/SFX event (max 10 total).
	/// </summary>
	public void ScheduleEvent(string name, string expectedState, EventType type)
	{
		if (eventCount >= MaxEvents)
			return; // Or overwrite oldest if desired

		events[eventCount].name = name;
		events[eventCount].type = type;
		events[eventCount].scheduledFrame = Globals.frame + frameDelay;
		events[eventCount].creationFrame = Globals.frame;
		events[eventCount].active = true;

		eventCount++;
	}

	public void FrameAdvance()  // TODO - check that this shouldn't be TimeAdvance instead
	{
		int i = 0;

		while (i < eventCount)
		{
			if (ShouldExecuteOrRemove(ref events[i]))
			{
				RemoveAt(i);
			}
			else
			{
				i++;
			}
		}
	}

	private bool ShouldExecuteOrRemove(ref ScheduledEvent ev)
	{
		if (Globals.frame == ev.scheduledFrame)
		{
			ExecuteEvent(ref ev);
			return true;
		}

		if (Globals.frame < ev.creationFrame)
		{
			return true;
		}

		return false;
	}

	private void RemoveAt(int index)
	{
		eventCount--;

		if (index != eventCount)
		{
			events[index] = events[eventCount];
		}

		// Optional: clear last slot (not required, but cleaner)
		events[eventCount].active = false;
		events[eventCount].name = null;
	}

	private void ExecuteEvent(ref ScheduledEvent ev)
	{
		if (ev.type == EventType.AUDIO)
		{
			audioPlay.PlaySound(ev.name);
		}
		else if (ev.type == EventType.GRAPHIC)
		{
			// Add graphic handling here
		}
	}

	/// <summary>
	/// Immediately play an event.
	/// </summary>
	public void ForceEvent(EventType type, string name)
	{
		if (type == EventType.AUDIO)
		{
			audioPlay.PlaySound(name);
		}
		else if (type == EventType.GRAPHIC)
		{
			// Add graphic handling here
		}
	}
}
