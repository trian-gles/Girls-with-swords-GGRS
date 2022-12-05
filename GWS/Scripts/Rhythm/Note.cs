using Godot;
using System;

public class Note : Polygon2D
{
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";
	
	private int timing;
	private enum Status {
		waiting,
		missed,
		onTarget,
		p1Hit,
		p2Hit,
		bothHit
	}

	private Status status = Status.waiting;
	
	public int rate;

	public struct NoteState {
		
	}
	

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}
	
	public void Init(int @timing){
		timing = @timing;
		Position = new Vector2((timing) * rate + 60, 6);
	}
	
	private void SetTarget(){
		Color = new Color(255, 255, 255, 255);
		status = Status.onTarget;
		GD.Print("Setting Target ON");
	}
	
	private void SetMissed(){
		Color = new Color(0, 0, 0, 255);
		status = Status.missed;
	}
	
	private void SetP1Hit(){
		Color = new Color(0, 0, 255, 255);
		status = Status.p1Hit;
	}
	
	private void SetP2Hit(){
		Color = new Color(0, 255, 0, 255);
		status = Status.p2Hit;
	}
	
	private void SetBothHit(){
		Color = new Color(255, 0, 0, 255);
		status = Status.bothHit;
	}

	// will return 1 if out of bounds and should be deleted
	public bool AdvanceFrame(int frame){
		Position = new Vector2(Position.x - rate, 6);
		
		if (Position.x < (rate * 3 + 60) && Position.x >= (rate * 2 + 60)){
			SetTarget();
		}
		
		else if (status == Status.onTarget && Position.x < (60 - rate * 3)){
			SetMissed();
		}
		
		return (Position.x < (-1 * rate * 12));
	}
	
	public bool TryHit(string playerName){
		if (playerName == "P1"){
			if (status == Status.onTarget){
				SetP1Hit();
				return true;
			}
			else if (status == Status.p2Hit){
				SetBothHit();
				return true;
			}
			else {
				return false;
			}
		}
		else {
			if (status == Status.onTarget){
				SetP2Hit();
				return true;
			}
			else if (status == Status.p1Hit){
				SetBothHit();
				return true;
			}
			else {
				return false;
			}
		}
	}



//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
