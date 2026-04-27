using Godot;
using System;
using System.Collections.Generic;

public class CharacterAudio : Node
{
	private List<AudioStreamPlayer> _players = new List<AudioStreamPlayer>();

	private Dictionary<string, Sound> soundDict = new Dictionary<string, Sound>();

	class Sound { public AudioStream audio; public int lastPlayedFrame; public float level; }

	private Random random = new Random();

	private const string StaggerKey = "Stagger";
	private const string Stagger1Key = "Stagger1";
	private const string Stagger2Key = "Stagger2";
	private const string Stagger3Key = "Stagger3";
	private const string Stagger4Key = "Stagger4";

	public override void _EnterTree()
	{
		base._EnterTree();
		foreach (var kvp in soundDict)
		{
			kvp.Value.lastPlayedFrame = -1000;
		}
	}

	private void AddSound(string name, AudioStream stream)
	{
		soundDict.Add(name, new Sound() { audio = stream, lastPlayedFrame = -1000, level = -5.0f });
	}
	private void AddSound(string name, AudioStream stream, float level)
	{
		soundDict.Add(name, new Sound() { audio = stream , lastPlayedFrame = - 1000, level = level});
	}

	public override void _Ready()
	{
		AddSound("HitStun", LoadAudio("res://Sounds/hit.ogg"), 0);
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

		AddSound("LampFireWhiff", LoadAudio("res://Sounds/Lamp-FireWoosh.ogg"));
		AddSound("LampWhiff", LoadAudio("res://Sounds/Lamp-Woosh.ogg"));
		AddSound("LampHit", LoadAudio("res://Sounds/Lamp-Hit.ogg"));

		AddSound("PunchWhiff", LoadAudio("res://Sounds/Punch-Woosh.ogg"));
		AddSound("PunchHit", LoadAudio("res://Sounds/Punch-Impact.ogg"));

		AddSound("SlashWhiff", LoadAudio("res://Sounds/Slash-Woosh.ogg"));
		AddSound("SlashHit", LoadAudio("res://Sounds/Slash-Hit.ogg"));
		
		AddSound("COUNTER", LoadAudio("res://Sounds/COUNTER.ogg"));
		AddSound("SnailRide", LoadAudio("res://Sounds/engine.ogg"));
			
		AddSound("DOWN", LoadAudio("res://Sounds/DOWN.ogg"), 2);
		AddSound("THREE", LoadAudio("res://Sounds/THREE.ogg"));
		AddSound("TWO", LoadAudio("res://Sounds/TWO.ogg"));
		AddSound("ONE", LoadAudio("res://Sounds/ONE.ogg"));
		AddSound("FIGHT", LoadAudio("res://Sounds/FIGHT.ogg"));

		AddSound("BackToss", LoadAudio("res://Sounds/snail-release.ogg"), 0);
		AddSound("AirSnail", LoadAudio("res://Sounds/snail-release.ogg"), 0);
		AddSound("snail-strike", LoadAudio("res://Sounds/snail-strike.ogg"));
		AddSound("snail-walk", LoadAudio("res://Sounds/squishy.ogg"));
		AddSound("shock", LoadAudio("res://Sounds/shock.ogg"));
		AddSound("electricity", LoadAudio("res://Sounds/electricity.ogg"));
			
		AddSound("GuardCancel", LoadAudio("res://Sounds/guard-cancel.ogg"));
		AddSound("RC", LoadAudio("res://Sounds/RC.ogg"));
		AddSound("JoeRogan", LoadAudio("res://Sounds/joerogan.ogg"));

		AddSound("TeleportDownSlash", LoadAudio("res://Sounds/hat-down-teleport.ogg"));
		AddSound("TeleportDP", LoadAudio("res://Sounds/hat-up-teleport.ogg"));

		AddSound("FireThrow", LoadAudio("res://Sounds/hat-throw.ogg"), 2);

		AddSound("Burst", LoadAudio("res://Sounds/burst.ogg"), 3);
		AddSound("OHSHIT", LoadAudio("res://Sounds/OH-SHIT.ogg"), 3);

		AddSound("Talking1", LoadAudio("res://Sounds/talking1.ogg"));
		AddSound("Talking2", LoadAudio("res://Sounds/talking2.ogg"));

		foreach (var child in GetChildren())
		{
			_players.Add((AudioStreamPlayer)child);
		}
	}

	private string[] staggerOptions = new string[] { Stagger1Key, Stagger2Key, Stagger3Key, Stagger4Key };
	public void PlaySound(string name)
	{

		if (Globals.DISABLESFX)
			return;


		if (name == StaggerKey)
			name = staggerOptions[random.Next(1, 4)];

		if (!soundDict.ContainsKey(name))
			return;
		Sound queuedSound = soundDict[name];
		int frame = Globals.frame;
		if (frame < queuedSound.lastPlayedFrame + 6)
		{
			return;
		}


		foreach (var player in _players)
		{
			if (!player.Playing)
			{
				player.Stream = queuedSound.audio;
				player.Play();
				player.VolumeDb = queuedSound.level;
				break;
			}
		}
		queuedSound.lastPlayedFrame = frame;
	}

	private AudioStream LoadAudio(string path)
	{
		AudioStream astr = ResourceLoader.Load(path) as AudioStream;
		return astr;
	}
}
