using Godot;
using System;

public class Note : Polygon2D
{
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";

	[Signal]
	public delegate void NoteLand();
	
	private int timing;

	private int window = 5;
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

	/// <summary>
	/// 
	/// </summary>
	/// <param name="frame"></param>
	/// <returns>true if the note should be deleted</returns>
	public bool AdvanceFrame(int frame){
		

		if (status == Status.p1Hit || status == Status.p2Hit || status == Status.bothHit)
		{
			Position = new Vector2(45, Position.y - rate);
			return Position.y < -400;
		}
		else
		{
			Position = new Vector2(Position.x - rate, 6);
		}

		if (Position.x < (rate * window + 60) && Position.x >= (rate * (window - 1) + 60)) // this needs to be made more efficient
		{
			SetTarget();
		}

		else if (Position.x < 60 + rate && Position.x >= (60)) 
		{
			EmitSignal("NoteLand");
		}

		else if (status == Status.onTarget && Position.x < (60 - rate * window))
		{
			SetMissed();
		}
		
		return (Position.x < (-1 * rate * 15));
	}
	
	public bool TryHit(string playerName){
		if (playerName == "P1"){
			if (status == Status.onTarget){
				SetP1Hit();
				return true;
			}
			else if (status == Status.p2Hit && Position.x >= (60 - rate * 3))
			{
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
			else if (status == Status.p1Hit && Position.x >= (60 - rate * 3))
			{
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
