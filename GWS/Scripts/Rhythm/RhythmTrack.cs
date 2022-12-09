using Godot;
using System;
using System.Collections.Generic;

public class RhythmTrack : ReferenceRect
{
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";
	
	private List<Note> notes = new List<Note>();
	private Godot.AudioStreamPlayer audioStreamPlayer;

	public int noteRate = 4;
	
	[Export]
	public PackedScene noteScene;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (!Globals.rhythmGame)
			return;

		audioStreamPlayer = GetNode<Godot.AudioStreamPlayer>("AudioStreamPlayer");
		for (int i = 0; i < 200; i++){
			var note = noteScene.Instance() as Note;
			AddChild(note);
			note.Name = $"note{i}";
			note.rate = noteRate;
			note.Init(i * 20);
			notes.Add(note);
			note.Visible = true;
			note.Connect("NoteLand", this, nameof(OnNoteLand));
			
		}
	}
	
	public void Config(){
	}

	public void AdvanceFrame(int frame){
		if (!Globals.rhythmGame)
			return;
		
		var deleteNotes = new List<Note>();
		
		foreach (var note in notes) {
			if (note.AdvanceFrame(frame)){
				deleteNotes.Add(note);
			}
		}
		
		foreach (var note in deleteNotes) {
			notes.Remove(note);
			note.QueueFree();
		}
		
	}
	
	public bool TryHit(string playerName){
		int i = 0;
		foreach (var note in notes) {
			if (note.TryHit(playerName)){
				return true;
			}
			i++;
			if (i > 20){  // should maybe raise this.  Idk
				return false;
			}
		}
		
		return false;
	}

	public void OnNoteLand()
	{
		audioStreamPlayer.Play();
	}

	
}
