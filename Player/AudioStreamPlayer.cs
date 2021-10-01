using Godot;
using System;
using System.Collections.Generic;

public class AudioStreamPlayer : Godot.AudioStreamPlayer
{

    private AudioStream hitSound;
    private Dictionary<string, SoundFile> soundFiles = new Dictionary<string, SoundFile>();

    private struct SoundFile
    {
        public string path;
        public AudioStream stream;
    }

    public override void _Ready()
    {
        
        hitSound = ResourceLoader.Load("res://Sounds/clap.ogg") as AudioStream;
    }
    public void PlaySound(string name)
    {
        GD.Print("Playing sound");
        if (name == "HitStun")
        {
            SetStream(hitSound);
            Play();
        }
    }
}
