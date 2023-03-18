using Godot;
using System;
using System.Collections.Generic;

public class AudioStreamPlayer : Godot.AudioStreamPlayer
{

	private Dictionary<string, Sound> soundDict = new Dictionary<string, Sound>();

	class Sound { public AudioStream audio; public int lastPlayedFrame; }

	private Random random = new Random();

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
		AddSound("Stagger1", LoadAudio("res://Sounds/lick1.ogg"));
		AddSound("Stagger2", LoadAudio("res://Sounds/lick2.ogg"));
		AddSound("Stagger3", LoadAudio("res://Sounds/lick3.ogg"));
		AddSound("Fire1", LoadAudio("res://Sounds/Fire-High.ogg"));
		AddSound("Fire2", LoadAudio("res://Sounds/Fire-Low.ogg"));
		AddSound("Fire3", LoadAudio("res://Sounds/Fire-No_Bend.ogg"));
		AddSound("WarpSpawn", LoadAudio("res://Sounds/Warp_Spawn.ogg"));
	}
	public void PlaySound(string name)
	{
		if (name == "Stagger")
			name = name + random.Next(1, 4).ToString();

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
