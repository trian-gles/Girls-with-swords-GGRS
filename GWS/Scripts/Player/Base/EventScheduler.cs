using Godot;
using System;
using System.Collections.Generic;

public class EventScheduler : Node
{
	public enum EventType
	{
		AUDIO,
		GRAPHIC
	}
	private struct Event
	{
		public string name;
		public string expectedState;
		public EventType type;
		public int scheduledFrame;
		public int creationFrame;
	}

	private List<Event> scheduledEvents = new List<Event>();
	private CharacterAudio audioPlay;


	[Export]
	public int frameDelay = 4;

	public override void _Ready()
	{
		audioPlay = GetNode<CharacterAudio>("CharacterAudioHandler");
	}

	public void TimeAdvance()
	{
	}

	/// <summary>
	/// Schedule a GFX/SFX event to take place provided rollbacks don't interfere (the past is rewritten)
	/// </summary>
	/// <param name="name">Either the name of the current state, or the name of the inherited state that was used to store the expected effect</param>
	/// <param name="expectedState">UNUSED.  IGNORE THIS</param>
	/// <param name="type">audio or graphic</param>
	public void ScheduleEvent(string name, string expectedState, EventType type)
	{
		var ev = new Event();
		ev.name = name;
		ev.expectedState = expectedState;
		ev.type = type;
		ev.scheduledFrame = Globals.frame + frameDelay;
		ev.creationFrame = Globals.frame;
		scheduledEvents.Add(ev);

		// GD.Print($"Scheduling {name} on frame {ev.creationFrame} for frame {ev.scheduledFrame}");
	}

	public void FrameAdvance()
	{
		// GD.Print($"frame in event scheduler {Globals.frame}");
		List<Event> removeEvents = new List<Event>();
		foreach (Event @event in scheduledEvents)
		{
			TryEvent(@event, removeEvents);
		}

		foreach (Event @event in removeEvents)
		{
			scheduledEvents.Remove(@event);
		}
	}

	private void TryEvent(Event @event, List<Event> removeEvents)
	{
		if ((Globals.frame == @event.scheduledFrame))
			
		{
			ExecuteEvent(@event);
			removeEvents.Add(@event);
		}

		else if ((Globals.frame < @event.creationFrame))
		{
			// GD.Print($"Removing event {@event.name}, created on frame {@event.creationFrame} scheduled for frame {@event.scheduledFrame} on frame {Globals.frame}");
			removeEvents.Add(@event);
		}
	}


	private void ExecuteEvent(Event @event)
	{
		if (@event.type == EventType.AUDIO)
		{
			audioPlay.PlaySound(@event.name);
		}

		else if (@event.type == EventType.GRAPHIC)
		{

		}
	}

	/// <summary>
	/// Immediately play a sound.  Useful for landing
	/// </summary>
	/// <param name="name"></param>
	public void ForceEvent(EventType type, string name)
	{
		if (type == EventType.AUDIO)
		{
			audioPlay.PlaySound(name);
		}
		else if (type == EventType.GRAPHIC)
		{

		}
		
	}
}
