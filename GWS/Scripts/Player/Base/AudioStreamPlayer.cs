using Godot;
using System;
using System.Collections.Generic;

public class AudioStreamPlayer : Godot.AudioStreamPlayer
{

	private Dictionary<string, Sound> soundDict = new Dictionary<string, Sound>();

	class Sound { public AudioStream audio; public int lastPlayedFrame; }

	private void AddSound(string name, AudioStream stream)
    {
		soundDict.Add(name, new Sound() { audio = stream , lastPlayedFrame = - 1000});

	}
	public override void _Ready()
	{
		AddSound("HitStun", LoadAudio("res://Sounds/hit.ogg"));
		AddSound("Block", LoadAudio("res://Sounds/block.ogg"));
		AddSound("Knockdown", LoadAudio("res://Sounds/knockdown.ogg"));
		AddSound("Jump", LoadAudio("res://Sounds/jump.ogg"));
		AddSound("MovingJump", LoadAudio("res://Sounds/jump.ogg"));
		AddSound("Step", LoadAudio("res://Sounds/walk.ogg"));
		AddSound("Backdash", LoadAudio("res://Sounds/dash.ogg"));
		AddSound("Hadouken", LoadAudio("res://Sounds/hadouken.ogg"));
		AddSound("Landing", LoadAudio("res://Sounds/landing.ogg"));
		AddSound("Whiff", LoadAudio("res://Sounds/whiff.ogg"));
	}
	public void PlaySound(string name)
	{
		if (!soundDict.ContainsKey(name))
			return;
		Sound queuedSound = soundDict[name];
		int frame = Globals.frame;
		if (frame < queuedSound.lastPlayedFrame + 10)
        {
			return;
		}

		Stream = queuedSound.audio;
		Play();
		queuedSound.lastPlayedFrame = frame;
	}

	private AudioStream LoadAudio(string path)
	{
		AudioStream astr = ResourceLoader.Load(path) as AudioStream;
		return astr;
	}
}
