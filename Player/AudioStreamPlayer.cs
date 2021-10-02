using Godot;
using System;
using System.Collections.Generic;

public class AudioStreamPlayer : Godot.AudioStreamPlayer
{

    private Dictionary<string, AudioStream> soundDict = new Dictionary<string, AudioStream>();

    public override void _Ready()
    {
        soundDict.Add("HitStun", LoadAudio("res://Sounds/clap.ogg"));
        soundDict.Add("Knockdown", LoadAudio("res://Sounds/knockdown.ogg"));
        soundDict.Add("Jump", LoadAudio("res://Sounds/jump.ogg"));
        soundDict.Add("Run", LoadAudio("res://Sounds/dash.ogg"));
        soundDict.Add("Hadouken", LoadAudio("res://Sounds/hadouken.ogg"));
        soundDict.Add("Landing", LoadAudio("res://Sounds/landing.ogg"));
    }
    public void PlaySound(string name)
    {
        GD.Print("Playing sound");
        Stream = soundDict[name];
        Play();
    }

    private AudioStream LoadAudio(string path)
    {
        AudioStream astr = ResourceLoader.Load(path) as AudioStream;
        return astr;
    }
}
