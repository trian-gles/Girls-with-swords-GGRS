using Godot;
using System;
using System.Collections.Generic;

public class AudioStreamPlayer : Godot.AudioStreamPlayer
{

    private Dictionary<string, AudioStream> soundDict = new Dictionary<string, AudioStream>();

    public override void _Ready()
    {
        soundDict.Add("HitStun", LoadAudio("res://Sounds/hit.ogg"));
        soundDict.Add("Block", LoadAudio("res://Sounds/block.ogg"));
        soundDict.Add("Knockdown", LoadAudio("res://Sounds/knockdown.ogg"));
        soundDict.Add("Jump", LoadAudio("res://Sounds/jump.ogg"));
        soundDict.Add("MovingJump", LoadAudio("res://Sounds/jump.ogg"));
        soundDict.Add("Step", LoadAudio("res://Sounds/walk.ogg"));
        soundDict.Add("Backdash", LoadAudio("res://Sounds/dash.ogg"));
        soundDict.Add("Hadouken", LoadAudio("res://Sounds/hadouken.ogg"));
        soundDict.Add("Landing", LoadAudio("res://Sounds/landing.ogg"));
        soundDict.Add("Whiff", LoadAudio("res://Sounds/whiff.ogg"));
    }
    public void PlaySound(string name)
    {
        Stream = soundDict[name];
        Play();
    }

    private AudioStream LoadAudio(string path)
    {
        AudioStream astr = ResourceLoader.Load(path) as AudioStream;
        return astr;
    }
}
