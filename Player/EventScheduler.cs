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
        public EventType type;
        public int scheduledFrame;
    }

    private List<Event> scheduledEvents = new List<Event>();
    private AudioStreamPlayer audioPlay;


    [Export]
    public int frameDelay = 7;

    public override void _Ready()
    {
        audioPlay = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
    }

    public void ScheduleEvent(string name, EventType type)
    {
        var ev = new Event();
        ev.name = name;
        ev.type = type;
        ev.scheduledFrame = Globals.frame + frameDelay;

        scheduledEvents.Add(ev);
    }

    public void FrameAdvance()
    {
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
            Type sType = ((Player)Owner).currentState.GetType();
            if (sType.ToString() ==  @event.name)
            {
                GD.Print($"Scheduled event concluded.  Name = {@event.name}");
                ExecuteEvent(@event);
            }
            removeEvents.Add(@event);
        }

        else if ((Globals.frame == @event.scheduledFrame + frameDelay))
        {
            
            GD.Print($"Scheduled event abandoned.  Name = {@event.name}");
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
}
