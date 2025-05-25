using Godot;
using System;
using System.Collections.Generic;
using System.Drawing.Text;
using static TutorialManager;

public class TutorialManager : TrainingManager
{

	public class Challenge {
		public Challenge(string name){
			this.name = name;
		}
		public string name;
		public List<Goal> goals = new List<Goal>();
	}
	public class Goal {
		public Goal(string text, string input1="", string input2="", string input3="")
		{
			this.text = text;
			this.input1 = input1;
			this.input2 = input2;
			this.input3 = input3;
		}
		public delegate bool OtherRequirement();


		public string input1;
		public string input2;
		public string input3;
		public string text;
		public string p1State;
		public string p2State;
		public HashSet<string> p1Tags = new HashSet<string>();
		public HashSet<string> p2Tags = new HashSet<string>();
		public string p2FailState;
		public OtherRequirement otherRequirement;
	}

	private List<Challenge> challenges = new List<Challenge>();

	private int currChallengePtr;
	private Challenge currChallenge;

	private int currGoalPtr;
	private Goal currGoal;

	private Node tutorialContainer;

	private bool shouldAdvance = false;
	private int advanceFrame;


	public override void _Ready()
	{
		base._Ready();

		tutorialContainer = gameScene.GetNode("HUD/TutorialContainer");

		// Walk
		Challenge moveChallenge = new Challenge("Basic Movement");


		Goal jumpGoal = new Goal("Jump", "up");
		jumpGoal.p1State = "Jump";
		moveChallenge.goals.Add(jumpGoal);

		Goal walkForwardGoal = new Goal("Walk", "right", "left");
		walkForwardGoal.p1State = "Walk";
		moveChallenge.goals.Add(walkForwardGoal);

		Goal fJumpGoal = new Goal("Forward/Back Jump", "right", "left", "up");
		fJumpGoal.p1State = "Jump";
		moveChallenge.goals.Add(fJumpGoal);

		Goal crouchGoal = new Goal("Crouch", "down");
		crouchGoal.p1State = "Crouch";
		moveChallenge.goals.Add(crouchGoal);

		Challenge dashChallenge = new Challenge("Dashing");

		Goal runGoal = new Goal("Run", "right", "dash");
		runGoal.p1State = "Run";
		dashChallenge.goals.Add(runGoal);

		Goal backdashGoal = new Goal("Backdash", "left", "dash");
		backdashGoal.p1State = "Backdash";
		dashChallenge.goals.Add(backdashGoal);

		Goal adGoal = new Goal("Airdash", "up", "right", "dash");
		adGoal.p1State = "AirDash";
		dashChallenge.goals.Add(adGoal);

		Goal abdGoal = new Goal("Airbackdash", "up", "left", "dash");
		abdGoal.p1State = "AirBackdash";
		dashChallenge.goals.Add(abdGoal);

		challenges.Add(moveChallenge);
		challenges.Add(dashChallenge);

		// Attack
		Challenge attackChallenge = new Challenge("Basic Attacks");
		Goal jabGoal = new Goal("Punch", "p");
		jabGoal.p1State = "Jab";
		attackChallenge.goals.Add(jabGoal);

		Goal kickGoal = new Goal("Kick", "k");
		kickGoal.p1State = "Kick";
		attackChallenge.goals.Add(kickGoal);

		Goal slashGoal = new Goal("Slash", "s");
		slashGoal.p1State = "Slash";
		attackChallenge.goals.Add(slashGoal);


		Challenge airAttackChallenge = new Challenge("Air Attacks");
		Goal jJabGoal = new Goal("Air Punch", "up", "p");
		jJabGoal.p1State = "JumpA";
		airAttackChallenge.goals.Add(jJabGoal);

		Goal jKickGoal = new Goal("Air Kick", "up", "k");
		jKickGoal.p1State = "JumpB";
		airAttackChallenge.goals.Add(jKickGoal);

		Goal jSlashGoal = new Goal("Air Slash", "up", "s");
		jSlashGoal.p1State = "JumpC";
		airAttackChallenge.goals.Add(jSlashGoal);

		Goal j2SlashGoal = new Goal("Downwards Air Slash", "up", "down", "s");
		j2SlashGoal.p1State = "InstantOverhead";
		airAttackChallenge.goals.Add(j2SlashGoal);

		Challenge attackChallenge2 = new Challenge("Command Attacks");

		Goal AAGoal = new Goal("Anti air", "right", "p");
		AAGoal.p1State = "6P";
		attackChallenge2.goals.Add(AAGoal);

		Goal sixKGoal = new Goal("Moving attack", "right", "k");
		sixKGoal.p1State = "6K";
		attackChallenge2.goals.Add(sixKGoal);

		Goal sixSGoal = new Goal("Heavy slash", "right", "s");
		sixSGoal.p1State = "6S";
		attackChallenge2.goals.Add(sixSGoal);

		Goal dashAttackGoal = new Goal("Dashing slash", "right", "dash", "s");
		dashAttackGoal.p1State = "InstantOverhead";
		attackChallenge2.goals.Add(dashAttackGoal);

		Challenge specialAttackChallenge = new Challenge("Special Attacks");

		string[] allDirections = new string[] {"", "right", "left", "down", "up" };
		string[] olSpecials = new string[] { "CommandRun", "AntiAir", "CommandRunWillTurn", "Hadouken", "AntiAir" };
		for (int i = 0; i < allDirections.Length; i++)
		{
			Goal specialGoal = new Goal("Special Skill", allDirections[i], "special");
			specialGoal.p1State = olSpecials[i];
			specialAttackChallenge.goals.Add(specialGoal);

		}


		

		challenges.Add(attackChallenge);
		challenges.Add(airAttackChallenge);
		challenges.Add(attackChallenge2);
		challenges.Add(specialAttackChallenge);


		playerOne = 0;
		playerTwo = 1;
		colorOne = 0;
		colorTwo = 0;
		currChallenge = challenges[0];
		
		OnNewGame();
		gameScene.ignoreTime = true;
		gameScene.SetDebugVisibility(true);
		gameScene.ConnectTrainingSignals(this);
		gameScene.Reset();
		InitChallenge(currChallenge);
	}

	private bool CheckFail(){
		return currGoal.p2State == gameScene.P2.currentState.Name;
	}

	public override void _PhysicsProcess(float delta)
	{
		base._PhysicsProcess(delta);
		if (!shouldAdvance && CheckGoal()) 
		{ 
			CompleteGoal();
		}

		if (shouldAdvance && advanceFrame < Globals.frame)
			CompleteChallenge();
	}

	private void InitChallenge(Challenge c){
		gameScene.ChangeHUDText(c.name);
		currGoalPtr = 0;
		currGoal = c.goals[currGoalPtr];
		tutorialContainer.Call("reset");

		foreach (var goal in c.goals)
		{
			tutorialContainer.Call("add_goal", goal.text, goal.input1, goal.input2, goal.input3);
		}
		tutorialContainer.Call("curr_goal", 0);
		GD.Print("Resetting challenge");
	}

	private bool CheckGoal(){
		bool result = true;
		if (currGoal.p1State != null){
			result = result && (currGoal.p1State == gameScene.P1.currentState.Name);
		}

		if (currGoal.p2State != null){
			result = result && (currGoal.p1State == gameScene.P2.currentState.Name);
		}

		if (currGoal.p1Tags.Count > 0){
			result = result && GetP1Tags().IsSupersetOf(currGoal.p1Tags);
		}

		if (currGoal.p2Tags.Count > 0){
			result = result && GetP1Tags().IsSupersetOf(currGoal.p2Tags);
		}



		return result;
	}

	private void CompleteAllChallenges(){

		var events = GetNode("/root/Events");
		events.Call("emit_signal", "MainMenuPressed");

	}

	private void CompleteChallenge(){
		shouldAdvance = false;
		gameScene.ResetTraining();
		currChallengePtr++;

		if (currChallengePtr == challenges.Count){
			CompleteAllChallenges();
		}
		else {
			currChallenge = challenges[currChallengePtr];
			InitChallenge(currChallenge);
		}

	}

	private void CompleteAllGoals()
	{
		shouldAdvance = true;
		advanceFrame = Globals.frame + 120;
		tutorialContainer.Call("success_all");
		if (currChallengePtr == challenges.Count - 1)
		{
			advanceFrame = Globals.frame + 240;
			tutorialContainer.Call("finish");
		}
	}


	private void CompleteGoal(){
		tutorialContainer.Call("success_goal", currGoalPtr);
		currGoalPtr++;
		

		if (currGoalPtr == currChallenge.goals.Count){
			CompleteAllGoals();
		}
		else {
			currGoal = currChallenge.goals[currGoalPtr];
			tutorialContainer.Call("curr_goal", currGoalPtr);
			// gameScene.highlightGoal(currGoalPtr)
		}
	}

	
}
