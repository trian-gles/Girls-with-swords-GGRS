using Godot;
using System;
using System.Collections.Generic;

public class TutorialManager : TrainingManager
{
	public struct Goal {
        public string text;
        public string p1State;
        public string p2State;
        public HashSet<string> p1Tags;
        public HashSet<string> p2Tags;
        public string p2FailState;
    }

    private List<Goal> goals;

    private int currGoalPtr;
    private Goal currGoal;

    private bool CheckFail(){
        return currGoal.p2State == gameScene.P2.currentState.Name;
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

    private void CompleteAllGoals(){
        gameScene.ResetTraining();
    }

    private void CompleteGoal(){
        // gameScene.ClearGoal(currGoalPtr)
        currGoalPtr++;
        

        if (currGoalPtr == goals.Count){
            CompleteAllGoals();
        }
        else {
            currGoal = goals[currGoalPtr];
            // gameScene.highlightGoal(currGoalPtr)
        }
    }

	
}
