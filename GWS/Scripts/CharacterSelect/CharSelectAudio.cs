using Godot;
using System;
using System.Collections.Generic;

public class CharSelectAudio : Godot.AudioStreamPlayer
{
	private Dictionary<string, AudioStream> sounds = new Dictionary<string, AudioStream>();
	public override void _Ready()
	{
		sounds.Add("CharSelect", LoadAudio("res://Sounds/char_selected.ogg"));
	}

	public void PlaySound(string name)
    {
		Stream = sounds[name];
		Play();
    }

	private AudioStream LoadAudio(string path)
	{
		AudioStream astr = ResourceLoader.Load(path) as AudioStream;
		return astr;
	}
}
