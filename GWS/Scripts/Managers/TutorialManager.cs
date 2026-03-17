using Godot;
using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Runtime.Serialization.Json;
using static TutorialManager;

public class TutorialManager : TrainingManager
{

	protected string RecordingName = "Fundies";
	public class Challenge
	{
		public Challenge(string name, GameScene.ResetPos resetPos = GameScene.ResetPos.ROUNDSTART)
		{
			this.name = name;
			this.resetPos = resetPos;
		}

		public void MakeComboChallenge()
		{
			for (int i = 1; i < goals.Count; i++)
			{
				goals[i] = goals[i].Copy();
				goals[i].p2FailTags.Add(Globals.Tags.recovery);
				goals[i].p2Tags.Add(Globals.Tags.hitstate);
			}
		}
		public GameScene.ResetPos resetPos;
		public string name;
		public string popupText;
		public List<Goal> goals = new List<Goal>();
		public List<int> p2Inputs;

		public string exampleInputFilename;
		public List<int> exampleInputs;
	}
	public class Goal
	{
		public Goal(string text, string input1 = "", string input2 = "", string input3 = "", string input4 = "")
		{
			this.text = text;
			this.input1 = input1;
			this.input2 = input2;
			this.input3 = input3;
			this.input4 = input4;
		}

		public Goal Copy()
		{
			var newGoal = new Goal(text, input1, input2, input3, input4)
			{
				p1State = p1State,
				p2State = p2State,
				p1StateFrame = p1StateFrame,
				p2StateFrame = p2StateFrame,
				minFramesSinceLastGoal = minFramesSinceLastGoal,
				p1FailState = p1FailState,
				p2FailState = p2FailState,
				p1Tags = new HashSet<Globals.Tags>(p1Tags),
				p2Tags = new HashSet<Globals.Tags>(p2Tags),
				p1FailTags = new HashSet<Globals.Tags>(p1FailTags),
				p2FailTags = new HashSet<Globals.Tags>(p2FailTags),
			};
			return newGoal;
		}
		public delegate bool OtherRequirement();


		public string input1;
		public string input2;
		public string input3;
		public string input4;
		public string text;
		public string p1State;
		public string p2State;
		public int p1StateFrame = -1;
		public int p2StateFrame = -1;
		public int minFramesSinceLastGoal = 0;

		public HashSet<Globals.Tags> p1Tags = new HashSet<Globals.Tags>();
		public HashSet<Globals.Tags> p2Tags = new HashSet<Globals.Tags>();
		public string p1FailState;
		public string p2FailState;

		public HashSet<Globals.Tags> p1FailTags = new HashSet<Globals.Tags>();
		public HashSet<Globals.Tags> p2FailTags = new HashSet<Globals.Tags>();
		public OtherRequirement otherRequirement;
	}

	protected List<Challenge> challenges = new List<Challenge>();
	private int lastGoalCompletedFrame;

	private int currChallengePtr = 0;
	private Challenge currChallenge;

	private int currGoalPtr;
	private Goal currGoal;

	private Node tutorialContainer;

	private Node events;



	// Recording / File
	private const string RecordingNameConst = "Fundies";
	private const string FileNotFoundString = "File not found";
	private const string DemoString = "DEMO";
	private const string RecString = "REC";
	private const string SavedRecordingsPathFormat = "res://SavedRecordings/{0}/{1}.json";

	// Actions
	private const string RecordAction = "record";
	private const string PlaybackAction = "playback";
	private const string SaveRecordingAction = "save_recording";
	private const string ResetAction = "reset";

	// Nodes
	private const string TutorialContainerPath = "HUD/TutorialContainer";
	private const string EventsPath = "/root/Events";

	// Tutorial container calls
	private const string TutorialResetCallString = "reset";
	private const string TutorialAddGoalCallString = "add_goal";
	private const string TutorialCurrGoalCallString = "curr_goal";
	private const string TutorialPlaybackFinishedCallString = "playback_finished";
	private const string TutorialSuccessAllCallString = "success_all";
	private const string TutorialFinishCallString = "finish";
	private const string TutorialFailGoalCallString = "fail_goal";
	private const string TutorialSuccessGoalCallString = "success_goal";

	// Events
	private const string EmitSignalString = "emit_signal";
	private const string MainMenuPressedString = "MainMenuPressed";

	// Common States
	private const string StateJump = "Jump";
	private const string StateWalk = "Walk";
	private const string StateRun = "Run";
	private const string StateBackdash = "Backdash";
	private const string StateCrouch = "Crouch";
	private const string StateBlock = "Block";
	private const string StateCrouchBlock = "CrouchBlock";
	private const string StateIdle = "Idle";
	private const string StateHitStun = "HitStun";
	private const string StateGrab = "Grab";
	private const string StateGrabbed = "Grabbed";
	private const string StateFloat = "Float";
	private const string StateStagger = "Stagger";
	private const string StateShield = "Shield";
	private const string StateGuardCancel = "GuardCancel";
	private const string StateTech = "Tech";
	private const string StateKnockdown = "Knockdown";
	private const string StateBurst = "Burst";
	private const string StateAirDash = "AirDash";
	private const string StateAirGrab = "AirGrab";
	private const string StateDoubleJump = "DoubleJump";
	private const string StateSuper = "Super";
	private const string StateInstantOverhead = "InstantOverhead";
	private const string StatePostRun = "PostRun";

	// Common Inputs
	private const string InputUp = "up";
	private const string InputDown = "down";
	private const string InputLeft = "left";
	private const string InputRight = "right";
	private const string InputDash = "dash";
	private const string InputHold = "hold";
	private const string InputAir = "air";
	private const string InputP = "p";
	private const string InputK = "k";
	private const string InputS = "s";
	private const string InputSpecial = "special";
	private const string InputWait = "wait";

	// Challenge Names
	private const string ChallengeBasicMovement = "Basic Movement";
	private const string ChallengeDashing = "Dashing";
	private const string ChallengeBasicAttacks = "Basic Attacks";
	private const string ChallengeCrouchingAttacks = "Crouching Attacks";
	private const string ChallengeAirAttacks = "Air Attacks";
	private const string ChallengeCommandAttacks = "Command Attacks";
	private const string ChallengeDashAttack = "Dash Attack";
	private const string ChallengeGrab = "Grab";
	private const string ChallengeAirGrab = "Air Grab";
	private const string ChallengeSpecialAttacks = "Special Attacks";
	private const string ChallengeGatlings = "Gatlings";
	private const string ChallengeRapidCancel = "Rapid Cancel";
	private const string ChallengeSuper = "OH SHIT";
	private const string ChallengeBlocking = "Blocking";
	private const string ChallengeOverheadBlocking = "Overhead blocking";
	private const string ChallengeLowBlocking = "Low blocking";
	private const string ChallengeEvadingGrabs = "Evading grabs";
	private const string ChallengeShield = "Shield";
	private const string ChallengeGuardCancel = "Guard Cancel";
	private const string ChallengeTech = "Tech/Ukemi";
	private const string ChallengeBurst = "Burst";


	private bool shouldAdvance = false;
	private bool failed = false;

	protected bool comboTrial = false;

	[Export]
	public bool skipCharSelect = true;

	protected Goal walkGoal;

	protected Goal jabGoal;
	protected Goal kickGoal;
	protected Goal slashGoal;

	protected Goal cjabGoal;
	protected Goal ckickGoal;
	protected Goal cslashGoal;

	protected Goal jJabGoal;
	protected Goal jKickGoal;
	protected Goal jSlashGoal;

	protected Goal jumpGoal;

	protected Goal fJumpGoal;
	protected Goal dFJumpGoal;

	protected Goal grabGoal;

	protected Goal adGoal;

	private Goal[] comboGoals;

	protected override void HandleSpecialInputs(InputEvent @event)
	{
		if (@event.IsActionPressed("record"))
		{
			if (recordingInputs2)
				StopInputRecord();
			else
			{
				RestartChallenge(false);
				StartInputRecord();
			}

		}
		else if (@event.IsActionPressed("playback"))
		{
			LoadRecording();
		}
		else if (@event.IsActionPressed("save_recording"))
		{
			if (recordingInputs2)
			{
				StopInputRecord();
				SaveRecording();
			}

		}
	}

	protected void LoadRecording()
	{
		RestartChallenge(false);
		var file = new File();

		Error err = file.Open($"res://SavedRecordings/{RecordingName}/{currChallenge.name}.json", Godot.File.ModeFlags.Read);
		if (err != Error.Ok)
		{
			GD.Print("File not found");
			return;
		}
		var arr = (Godot.Collections.Array)JSON.Parse(file.GetAsText()).Result;
		recordedInputs2 = new List<int>();
		for (int i = 0; i < arr.Count; i++)
		{
			recordedInputs2.Add(Int32.Parse(arr[i].ToString()));
		}

		playbackInputs2 = true;
		inputHead2 = 0;
		file.Close();

		gameScene.SetRecordingText("DEMO");

	}

	protected override void SaveRecording()
	{
		var file = new File();
		file.Open($"res://SavedRecordings/{RecordingName}/{currChallenge.name}.json", Godot.File.ModeFlags.Write);
		file.StoreString(JSON.Print(recordedInputs2));
		file.Close();
	}


	/// <summary>
	/// Overridden for each tutorial or combo trial
	/// </summary>
	public virtual void AddChallenges()
	{
		tutorialContainer.Call(TutorialResetCallString);
		Challenge moveChallenge = new Challenge("Basic Movement");

		moveChallenge.popupText = "Welcome to the Girls with Swords tutorial!  First let's go over some basic movement.  Press START or ESC at any time to see the currently configured controls.";

		moveChallenge.goals.Add(jumpGoal);

		
		moveChallenge.goals.Add(walkGoal);

		moveChallenge.goals.Add(fJumpGoal);

		Goal walkBackGoal = new Goal("Walk backwards", "left");
		walkBackGoal.p1State = "Walk";
		moveChallenge.goals.Add(walkBackGoal);

		Goal bJumpGoal = new Goal("Backwards Jump", "left", "up");
		bJumpGoal.p1State = "Jump";
		moveChallenge.goals.Add(bJumpGoal);

		Goal crouchGoal = new Goal("Crouch", "down");
		crouchGoal.p1State = "Crouch";
		moveChallenge.goals.Add(crouchGoal);

		Challenge dashChallenge = new Challenge("Dashing");

		dashChallenge.popupText = "You can also dash, airdash, double jump and super jump";

		Goal runGoal = new Goal("Run", "right", "dash");
		runGoal.p1State = "Run";
		dashChallenge.goals.Add(runGoal);

		Goal backdashGoal = new Goal("Backdash", "left", "dash");
		backdashGoal.p1State = "Backdash";
		dashChallenge.goals.Add(backdashGoal);


		dashChallenge.goals.Add(adGoal);

		Goal abdGoal = new Goal("Airbackdash", "air", "left", "dash");
		abdGoal.p1State = "AirBackdash";
		dashChallenge.goals.Add(abdGoal);

		Goal sJumpGoal = new Goal("Super Jump", "up", "dash");
		sJumpGoal.p1State = "SuperJump";
		dashChallenge.goals.Add(sJumpGoal);

		challenges.Add(moveChallenge);
		challenges.Add(dashChallenge);

		// Attack

		Challenge attackChallenge = new Challenge("Basic Attacks");
		attackChallenge.popupText = "Let's execute some attacks!";
		attackChallenge.goals.Add(jabGoal);
		attackChallenge.goals.Add(kickGoal);
		attackChallenge.goals.Add(slashGoal);

		Challenge crouchAttackChallenge = new Challenge("Crouching Attacks");
		crouchAttackChallenge.popupText = "Each attack also has a crouching and aerial variant";

		crouchAttackChallenge.goals.Add(cjabGoal);
		crouchAttackChallenge.goals.Add(ckickGoal);
		crouchAttackChallenge.goals.Add(cslashGoal);


		Challenge airAttackChallenge = new Challenge("Air Attacks");

		airAttackChallenge.goals.Add(jJabGoal);
		airAttackChallenge.goals.Add(jKickGoal);
		airAttackChallenge.goals.Add(jSlashGoal);



		Challenge attackChallenge2 = new Challenge("Command Attacks");
		attackChallenge2.popupText = "You can press forward along with attack buttons. In the air, press down along with slash. ";

		Goal AAGoal = new Goal("Anti air", "right", "p");
		AAGoal.p1State = "6P";
		AAGoal.p2State = "Float";
		attackChallenge2.goals.Add(AAGoal);

		Goal sixKGoal = new Goal("Moving attack", "right", "k");
		sixKGoal.p1State = "6K";
		sixKGoal.p2State = "HitStun";
		attackChallenge2.goals.Add(sixKGoal);

		Goal sixSGoal = new Goal("Heavy slash", "right", "s");
		sixSGoal.p1State = "6S";
		sixSGoal.p2State = "Stagger";
		attackChallenge2.goals.Add(sixSGoal);

		Goal j2SlashGoal = new Goal("Downwards Air Slash", "air", "down", "s");
		j2SlashGoal.p1State = "InstantOverhead";
		j2SlashGoal.p2State = "Stagger";
		attackChallenge2.goals.Add(j2SlashGoal);

		Challenge dashAttackChallenge = new Challenge("Dash Attack");
		dashAttackChallenge.popupText = "If you press slash while fully running, you'll perform a special dash attack";

		Goal dashAttackGoal = new Goal("Dashing slash", "right", "hold", "s");
		dashAttackGoal.p1State = "InstantOverhead";
		dashAttackGoal.p2State = "Stagger";
		Goal runGoalNoStop = new Goal("Run", "right", "dash");
		runGoalNoStop.p1State = "Run";
		dashAttackGoal.p1FailState = "PostRun";
		dashAttackChallenge.goals.Add(runGoalNoStop);
		dashAttackChallenge.goals.Add(dashAttackGoal);

		Challenge grabChallenge = new Challenge("Grab");
		grabChallenge.popupText = "Grabs are extremely fast and cannot be blocked, but you must be close to the opponent and they can't be stunned";


		grabChallenge.goals.Add(grabGoal);

		Challenge airGrabChallenge = new Challenge("Air Grab");
		airGrabChallenge.p2Inputs = new List<int>() { 1, 0, 0, 0, 1, 0, 0, 0, 0, 0 };
		airGrabChallenge.popupText = "If the opponent is in the air, you can perform an Air Grab.  The timing is tricky!";
		var airGrabGoal = new Goal("Air Grab", "air", "k", "s");

		airGrabGoal.p1State = "AirGrab";

		airGrabChallenge.goals.Add(airGrabGoal);

		Challenge specialAttackChallenge = new Challenge("Special Attacks");
		specialAttackChallenge.popupText = "Press any direction with the special button to perform a special move.";

		string[] allDirections = new string[] { "", "right", "left", "down", "up" };
		string[] olSpecials = new string[] { "CommandRun", "AntiAir", "CommandRunWillTurn", "Hadouken", "AntiAir" };
		for (int i = 0; i < allDirections.Length; i++)
		{
			Goal specialGoal = new Goal("Special Skill", allDirections[i], "special");
			specialGoal.p1State = olSpecials[i];
			specialAttackChallenge.goals.Add(specialGoal);

		}

		Challenge gatlingChallenge = new Challenge("Gatlings");
		Goal hojogiriGoal = new Goal("Hojogiri", "special")
		{
			p2StateFrame = 0,
			p1State = "Hojogiri"
		};
		gatlingChallenge.goals.Add(jabGoal);
		gatlingChallenge.goals.Add(kickGoal);
		gatlingChallenge.goals.Add(slashGoal);
		gatlingChallenge.goals.Add(cslashGoal);
		gatlingChallenge.goals.Add(hojogiriGoal);
		gatlingChallenge.MakeComboChallenge();
		gatlingChallenge.popupText = "By pressing a heavier attack immediately after a weaker attack connects, you can \"Gatling\" into the heavier attack allowing combos and blockstrings.  Experiment with what works on your character of choice!";

		Goal rcGoal = new Goal("Rapid Cancel", "p", "k", "s")
		{
			p1State = "Idle"
		};
		Challenge rcChallenge = new Challenge("Rapid Cancel");
		rcChallenge.goals.Add(sixSGoal);
		rcChallenge.goals.Add(rcGoal);
		rcChallenge.MakeComboChallenge();
		rcChallenge.popupText = "With a Rapid Cancel, you can spend half a bar of meter to cancel the recovery of any attack that connects with the opponent.";


		Challenge superChallenge = new Challenge("OH SHIT");
		superChallenge.popupText = "OH SHIT attacks cost half a bar of meter (see below your health bar), but make your opponent scream \"OH SHIT!\"";
		Goal superGoal = new Goal("OH SHIT", "right", "s", "special");
		superGoal.p1State = "Super";
		superChallenge.goals.Add(superGoal);



		challenges.Add(attackChallenge);
		challenges.Add(crouchAttackChallenge);
		challenges.Add(airAttackChallenge);
		challenges.Add(attackChallenge2);
		challenges.Add(dashAttackChallenge);
		challenges.Add(grabChallenge);
		challenges.Add(airGrabChallenge);
		challenges.Add(specialAttackChallenge);
		challenges.Add(superChallenge);
		challenges.Add(gatlingChallenge);
		challenges.Add(rcChallenge);

		////
		// DEFENSIVE TUTORIAL
		////

		Challenge midBlockChallenge = new Challenge("Blocking", GameScene.ResetPos.P1CORNEREDLEFT);
		midBlockChallenge.popupText = "You can block your opponent's attacks by holding the direction opposite to them";
		Goal blockGoal = new Goal("block", "left", "hold");
		blockGoal.p1State = "Block";
		midBlockChallenge.goals.Add(blockGoal);
		midBlockChallenge.p2Inputs = new List<int>() { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8, 520, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 18, 0, 18, 0, 18, 0, 18, 0, 18, 0, 18, 0, 18, 0, 18, 0, 18, 0, 18, 0, 18, 0, 18, 0, 18, 0, 18, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
		challenges.Add(midBlockChallenge);

		Challenge overheadChallenge = new Challenge("Overhead blocking", GameScene.ResetPos.P1CORNEREDLEFT);
		overheadChallenge.popupText = "By holding back with no other buttons, you'll block aerial and mid height attacks";
		Goal blockGoalOvr = new Goal("block mid and aerial", "left", "hold");
		blockGoalOvr.p1State = "Block";
		blockGoalOvr.p1FailTags.Add(Globals.Tags.hitstate);
		blockGoalOvr.p1StateFrame = 1;
		blockGoalOvr.minFramesSinceLastGoal = 20;
		overheadChallenge.goals.Add(blockGoalOvr);
		overheadChallenge.goals.Add(blockGoalOvr);
		overheadChallenge.p2Inputs = new List<int>() { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8, 520, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 9, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 66, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 40, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
		challenges.Add(overheadChallenge);

		Challenge lowChallenge = new Challenge("Low blocking", GameScene.ResetPos.P1CORNEREDLEFT);
		lowChallenge.popupText = "By holding back AND down, you'll block low and mid height attacks";
		Goal blockGoalLow = new Goal("block mids and lows", "down", "left", "hold");
		blockGoalLow.p1State = "CrouchBlock";
		blockGoalLow.p1FailTags.Add(Globals.Tags.hitstate);
		blockGoalLow.p1StateFrame = 1;
		blockGoalLow.minFramesSinceLastGoal = 20;
		lowChallenge.goals.Add(blockGoalLow);
		lowChallenge.goals.Add(blockGoalLow);
		lowChallenge.p2Inputs = new List<int>() { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8, 520, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 34, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 66, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
		challenges.Add(lowChallenge);

		Goal loBlockGoal = new Goal("Low block", "left", "down", "hold");
		loBlockGoal.p1State = "CrouchBlock";
		loBlockGoal.p1FailState = "HitStun";
		loBlockGoal.p1StateFrame = 1;

		Goal hiBlockGoal = new Goal("High block", "left", "hold");
		hiBlockGoal.p1State = "Block";
		hiBlockGoal.p1FailState = "HitStun";
		hiBlockGoal.p1StateFrame = 1;

		Challenge grabEvadeChallenge = new Challenge("Evading grabs", GameScene.ResetPos.P1CORNEREDLEFT);
		grabEvadeChallenge.popupText = "Grabs cannot be blocked! Try jumping out of GL's grab";
		Goal grabEvadeJump = new Goal("Jump out of the grab", "up");
		grabEvadeJump.p1State = "Jump";
		grabEvadeJump.p2State = "GrabStart";

		grabEvadeJump.p1FailState = "Grabbed";

		grabEvadeChallenge.goals.Add(grabEvadeJump);
		grabEvadeChallenge.p2Inputs = new List<int>() { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8, 520, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 96, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
		challenges.Add(grabEvadeChallenge);



		Challenge shieldChallenge = new Challenge("Shield", GameScene.ResetPos.P1CORNEREDLEFT);
		shieldChallenge.popupText = "By hold punch and kick while blocking you spend a bit of meter to create a repellant shield, pushing back the opponent extra far when they attack you";
		Goal shieldGoal = new Goal("Shield block", "left", "p", "k", "hold");
		shieldGoal.p1State = "Shield";
		shieldChallenge.goals.Add(shieldGoal);
		shieldChallenge.p2Inputs = new List<int>() { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8, 520, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 18, 0, 18, 0, 18, 0, 18, 0, 18, 0, 18, 0, 18, 0, 18, 0, 18, 0, 18, 0, 18, 0, 18, 0, 18, 0, 18, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
		challenges.Add(shieldChallenge);

		Challenge guardCancelChallenge = new Challenge("Guard Cancel", GameScene.ResetPos.P1CORNEREDLEFT);
		guardCancelChallenge.popupText = "By pressing punch, kick and forward while blocking you can spend half a bar of meter to kick the opponent off of you.";
		Goal gcGoal = new Goal("FUCK OFF", "right", "p", "k");
		gcGoal.p1State = "GuardCancel";
		gcGoal.p1FailState = "Idle";
		guardCancelChallenge.goals.Add(blockGoal);
		guardCancelChallenge.goals.Add(gcGoal);
		guardCancelChallenge.p2Inputs = new List<int>() { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8, 520, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 18, 0, 18, 0, 18, 0, 18, 0, 18, 0, 18, 0, 18, 0, 18, 0, 18, 0, 18, 0, 18, 0, 18, 0, 18, 0, 18, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
		challenges.Add(guardCancelChallenge);

		Challenge techChallenge = new Challenge("Tech/Ukemi", GameScene.ResetPos.P1CORNEREDLEFT);
		techChallenge.popupText = "When recovering from getting hit in/to the air, you can hold any attack button to perform an invincible escape when possible.  You can also hold left or right to escape with momentum.";
		Goal getHitGoal = new Goal("Get hit", "wait");
		getHitGoal.p1State = "Float";
		techChallenge.goals.Add(getHitGoal);
		Goal techGoal = new Goal("Tech", "p", "hold");
		techGoal.p1State = "Tech";
		techGoal.p1FailState = "Knockdown";
		techChallenge.goals.Add(techGoal);
		techChallenge.p2Inputs = new List<int>() { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8, 520, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 24, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
		challenges.Add(techChallenge);


		Goal getHitGoal2 = new Goal("Get hit", "wait");
		getHitGoal2.p1State = "HitStun";
		Challenge burstChallenge = new Challenge("Burst", GameScene.ResetPos.P1CORNEREDLEFT);
		burstChallenge.popupText = "If your SALT meter is full, press punch, kick and special simultaneously to escape a combo";
		burstChallenge.goals.Add(getHitGoal2);
		Goal burst = new Goal("Burst", "p", "k", "special");
		burst.p1State = "Burst";
		burst.p1FailTags.Add(Globals.Tags.recovery);
		burstChallenge.goals.Add(burst);
		burstChallenge.p2Inputs = new List<int>() { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 8, 520, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 16, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 16, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 16, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 16, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 16, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
		challenges.Add(burstChallenge);
	}

    public override void Quit()
    {
        base.Quit();
		currChallengePtr = 0;
    }


	public override void _Ready()
	{
		base._Ready();
		tutorialContainer = gameScene.GetNode("HUD/TutorialContainer");
		events = GetNode("/root/Events");


		// Setting up default goals
		jumpGoal = new Goal("Jump", "up");
		jumpGoal.p1State = "Jump";

		walkGoal = new Goal("Walk forwards", "right");
		walkGoal.p1State = "Walk";

		fJumpGoal = new Goal("Forward Jump", "right", "up");
		fJumpGoal.p1State = "Jump";

		dFJumpGoal = new Goal("Forward Double Jump", "air", "right", "up");
		dFJumpGoal.p1State = "DoubleJump";

		jabGoal = new Goal("Punch", "p")
		{
			p1State = "Jab",
			p2Tags = new HashSet<Globals.Tags> { Globals.Tags.hitstate },
			p2StateFrame = 0
		};

		kickGoal = new Goal("Kick", "k")
		{
			p1State = "Kick",
			p2StateFrame = 0,
			p2Tags = new HashSet<Globals.Tags> { Globals.Tags.hitstate }
		};

		slashGoal = new Goal("Slash", "s")
		{
			p1State = "Slash",
			p2StateFrame = 0,
			p2Tags = new HashSet<Globals.Tags> { Globals.Tags.hitstate }
		};

		cjabGoal = new Goal("Crouching Punch", "down", "p")
		{
			p1State = "CrouchA",
			p2StateFrame = 0,
			p2Tags = new HashSet<Globals.Tags> { Globals.Tags.hitstate }
		};

		ckickGoal = new Goal("Crouching Kick", "down", "k")
		{
			p1State = "CrouchB",
			p2StateFrame = 0,
			p2Tags = new HashSet<Globals.Tags> { Globals.Tags.hitstate }
		};

		cslashGoal = new Goal("Crouching Slash (sweep)", "down", "s")
		{
			p1State = "CrouchC",
			p2StateFrame = 0,
			p2Tags = new HashSet<Globals.Tags> { Globals.Tags.hitstate }
		};

		jJabGoal = new Goal("Air Punch", "air", "p")
		{
			p1State = "JumpA",
			p2StateFrame = 0,
			p2Tags = new HashSet<Globals.Tags> { Globals.Tags.hitstate }
		};

		jKickGoal = new Goal("Air Kick", "air", "k")
		{
			p1State = "JumpB",
			p2StateFrame = 0,
			p2Tags = new HashSet<Globals.Tags> { Globals.Tags.hitstate }
		};

		jSlashGoal = new Goal("Air Slash", "air", "s")
		{
			p1State = "JumpC",
			p2StateFrame = 0,
			p2Tags = new HashSet<Globals.Tags> { Globals.Tags.hitstate }
		};

		adGoal = new Goal("Airdash", "air", "right", "dash");
		adGoal.p1State = "AirDash";

		grabGoal = new Goal("Grab", "k", "s");
		grabGoal.p1State = "Grab";
	}

	public override void Start()
	{
		base.Start();

		Globals.autoTech = false;
		if (comboTrial)
		{
			Globals.autoTech = true;
		}






		if (skipCharSelect)
		{
			playerOne = 0;
			playerTwo = 1;
			colorOne = 0;
			colorTwo = 0;
			OnNewGame();
			charSelectScene.StopMusic();
		}
		else
		{
			charSelectScene.AutoSelectP2GL();
		}


		Globals.mode = Globals.Mode.TUTORIAL;
	}

	public override void OnNewGame()
	{
		base.OnNewGame();
		challenges.Clear();
		AddChallenges();
		currChallenge = challenges[currChallengePtr];
		gameScene.ignoreTime = true;
		gameScene.SetDebugVisibility(true);
		gameScene.ConnectTrainingSignals(this);
		gameScene.ResetRound();
		InitChallenge(currChallenge);
	}


	private bool CheckFail()
	{
		if (currGoal.p2FailState != null)
		{
			if (currGoal.p2FailState == gameScene.P2.currentState.Name)
				return true;
		}

		if (currGoal.p1FailState != null)
		{
			if (currGoal.p1FailState == gameScene.P1.currentState.Name)
				return true;
		}

		if (currGoal.p1FailTags.Overlaps(gameScene.GetP1Tags()))
			return true;

		if (currGoal.p2FailTags.Overlaps(gameScene.GetP2Tags()))
			return true;
		return false;
	}

	public override void _PhysicsProcess(float delta)
	{

		if (currGame.Name == "CharSelectScreen")
		{
			var (p1Inputs, p2Inputs) = GetCharSelectSceneP1Inputs();
			currGame.AdvanceFrame(p1Inputs, p2Inputs);
			return;
		}
		base._PhysicsProcess(delta);

		if (shouldAdvance && Input.IsActionJustPressed("reset"))
			CompleteChallenge();

		if (Input.IsActionJustPressed("reset"))
			RestartChallenge();

		if (shouldAdvance || failed)
			return;
		if (CheckFail() && currGoalPtr < currChallenge.goals.Count)
		{
			FailGoal();
			return;
		}

		if (!shouldAdvance && CheckGoal() && currGoalPtr < currChallenge.goals.Count)
		{
			CompleteGoal();
		}



	}

	private void RestartChallenge(bool showPopup = true)
	{
		playbackInputs2 = false;
		gameScene.SetRecordingText("");
		gameScene.ResetTraining();
		InitChallenge(currChallenge, showPopup);
	}

	private void InitChallenge(Challenge c, bool showPopup = true)
	{
		gameScene.ChangeHUDText(c.name);
		gameScene.ResetTraining();
		gameScene.SetPos(c.resetPos);
		if (c.p2Inputs != null)
		{
			recordedInputs = c.p2Inputs;
			playbackInputs = true;
		}
		else
		{
			playbackInputs = false;
		}

		inputHead = 0;

		currGoalPtr = 0;
		currGoal = c.goals[currGoalPtr];
		tutorialContainer.Call("reset");



		foreach (var goal in c.goals)
		{
			tutorialContainer.Call("add_goal", goal.text, goal.input1, goal.input2, goal.input3, goal.input4);
		}
		tutorialContainer.Call("curr_goal", 0);
		if (showPopup && c.popupText != null)
			Popup(c.popupText);

		failed = false;
	}

	protected override void StopInputPlayback(int num = 1)
	{
		if (num == 1)
		{
			// Auto restarts
			inputHead = 0;
		}
		else
		{
			inputHead = 1;
			playbackInputs2 = false;
		}

	}

	private bool CheckGoal()
	{
		bool result = true;
		if (currGoal.p1State != null)
		{
			result = result && (currGoal.p1State == gameScene.P1.currentState.Name);
		}

		if (currGoal.p2State != null)
		{
			result = result && (currGoal.p2State == gameScene.P2.currentState.Name);
		}

		if (currGoal.p1Tags.Count > 0)
		{
			result = result && GetP1Tags().Overlaps(currGoal.p1Tags);
		}

		if (currGoal.p2Tags.Count > 0)
		{
			result = result && GetP2Tags().Overlaps(currGoal.p2Tags);
		}

		if (currGoal.p1StateFrame != -1)
		{
			result = result && gameScene.P1.currentState.frameCount == currGoal.p1StateFrame;
		}

		if (currGoal.p2StateFrame != -1)
		{
			result = result && gameScene.P2.currentState.frameCount == currGoal.p2StateFrame;
		}

		if (currGoal.minFramesSinceLastGoal != 0)
		{
			result = result && Globals.frame > currGoal.minFramesSinceLastGoal + lastGoalCompletedFrame;
		}

		return result;
	}

	private void CompleteAllChallenges()
	{

		const string MainMenuPressedString = "MainMenuPressed";
		events.Call("emit_signal", MainMenuPressedString);

	}

	private void CompleteChallenge()
	{
		shouldAdvance = false;
		gameScene.ResetTraining();
		inputHead = 0;
		currChallengePtr++;

		if (currChallengePtr == challenges.Count)
		{
			CompleteAllChallenges();
		}
		else
		{
			currChallenge = challenges[currChallengePtr];
			InitChallenge(currChallenge);
		}

	}

	private void CompleteAllGoals()
	{
		if (playbackInputs2)
		{
				tutorialContainer.Call(TutorialPlaybackFinishedCallString);
			return;
		}
		shouldAdvance = true;
			tutorialContainer.Call(TutorialSuccessAllCallString);
		if (currChallengePtr == challenges.Count - 1)
		{
				tutorialContainer.Call(TutorialFinishCallString);
		}
	}

	public override void _Input(InputEvent @event)
	{
		HandleSpecialInputs(@event);
		// overriding this so nothing happens when user presses rec, etc.
	}

	private void FailGoal()
	{
		tutorialContainer.Call(TutorialFailGoalCallString, currGoalPtr);
		failed = true;
	}

	private void CompleteGoal()
	{
		tutorialContainer.Call(TutorialSuccessGoalCallString, currGoalPtr);
		currGoalPtr++;
		lastGoalCompletedFrame = Globals.frame;

		if (currGoalPtr == currChallenge.goals.Count)
		{
			CompleteAllGoals();
		}
		else
		{
			currGoal = currChallenge.goals[currGoalPtr];
				tutorialContainer.Call(TutorialCurrGoalCallString, currGoalPtr);
			// gameScene.highlightGoal(currGoalPtr)
		}
	}

	protected override void StartInputRecord()
	{
		inputHead = 0;
		inputHead2 = 0;
		recordedInputs2.Clear();
		recordingInputs2 = true;
		gameScene.SetRecordingText("REC");
	}
	protected override void StopInputRecord()
	{
		recordingInputs2 = false;
		gameScene.SetRecordingText("");
	}
}
