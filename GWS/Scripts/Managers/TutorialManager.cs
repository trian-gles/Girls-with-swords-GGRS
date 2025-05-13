using Godot;
using System;
using System.Collections.Generic;
using System.Drawing.Text;

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
		public Goal(string text){
			this.text = text;
		}
		public string text;
		public string p1State;
		public string p2State;
		public HashSet<string> p1Tags = new HashSet<string>();
		public HashSet<string> p2Tags = new HashSet<string>();
		public string p2FailState;
	}

	private List<Challenge> challenges = new List<Challenge>();

	private int currChallengePtr;
	private Challenge currChallenge;

	private int currGoalPtr;
	private Goal currGoal;


	public override void _Ready()
	{
		base._Ready();
		

		// Walk
		Challenge moveChallenge = new Challenge("Basic Movement");
		Goal walkForwardGoal = new Goal("Press forward and back to walk around");
		walkForwardGoal.p1State = "Walk";
		moveChallenge.goals.Add(walkForwardGoal);

		Goal jumpGoal = new Goal("Press up to jump");
		jumpGoal.p1State = "Jump";
		moveChallenge.goals.Add(walkForwardGoal);
		challenges.Add(moveChallenge);

		// Crouch
		Challenge crouchChallenge = new Challenge("crouch");
		Goal crouchGoal = new Goal("Press forward and back to move around!");
		crouchGoal.p1State = "Crouch";
		crouchChallenge.goals.Add(crouchGoal);
		challenges.Add(crouchChallenge);

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
		CheckGoal();
	}

	private void InitChallenge(Challenge c){
		gameScene.ChangeHUDText(c.name);
		currGoal = c.goals[0];
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

	}

	private void CompleteChallenge(){
		gameScene.ResetTraining();
		currChallengePtr++;

		if (currChallengePtr == challenges.Count){
			CompleteAllChallenges();
		}
		else {
			currChallenge = challenges[currGoalPtr];
			// gameScene.highlightGoal(currGoalPtr)
		}

	}

	private void CompleteGoal(){
		// gameScene.ClearGoal(currGoalPtr)
		currGoalPtr++;
		

		if (currGoalPtr == currChallenge.goals.Count){
			CompleteChallenge();
		}
		else {
			currGoal = currChallenge.goals[currGoalPtr];
			// gameScene.highlightGoal(currGoalPtr)
		}
	}

	
}
