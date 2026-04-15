using Godot;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Linq;
using System.Diagnostics.Eventing.Reader;
using System.Runtime.InteropServices;

/// <summary>
/// This object controls all the actual management of gameplay, and passes this information to GGPO
/// </summary>
public class GameStateObjectRedesign : Node
{
	public Player P1;
	public Player P2;
	private GameScene mainScene; // this seems like a bad idea, but the gsobj needs to add and remove nodes to the mainscene
	private MemoryPool memoryPool;

	[Signal]
	public delegate void LevelUp();

	private bool hosting;

	public GameState gState;
	private int hitStopRemaining = 0;

	private int maxHitStop = 14;

	private int levelUpHitStop = 60;
	/// <summary>
	/// Used for synctesting
	/// </summary>
	private GameState lastGs;

	private const int HADOUKENSTATESIZE = 68;
	/// <summary>
	/// Stores all vital data about positions in the game in a single struct
	/// </summary>
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public unsafe struct GameState
	{
		public int frame;
		public Player.PlayerState P1State;
		public Player.PlayerState P2State;
		public int totalHadoukens;
		public int hitStopRemaining;

		// From gameScene
		public int timeMode;
		public int possibleEndingFrame;
		public fixed byte hadoukenStates[HADOUKENSTATESIZE * 20]; // assuming a size of 68 bytes for now.  May need to change
	}

	private const int MAX_HADOUKENS = 20;

	// Dense storage
	private HadoukenPart[] hadoukens = new HadoukenPart[MAX_HADOUKENS];
	private int hadoukenCount = 0;

	// Delete queue (also fixed)
	private HadoukenPart[] deleteQueued = new HadoukenPart[MAX_HADOUKENS];
	private int deleteQueuedCount = 0;

	public GameStateObjectRedesign()
	{
		gState = new GameState();
		unsafe
		{
			memoryPool = new MemoryPool(sizeof(GameState), Globals.ROLLBACKDEPTH * 2 + 3);
		}
	}
	public void config(Player P1, Player P2, GameScene mainScene, bool hosting)
	{
		this.P1 = P1;
		this.P2 = P2;
		

		this.mainScene = mainScene;
		this.hosting = hosting;
		P1.Connect("HitConfirm", this, nameof(HandleHitConfirm));
		P2.Connect("HitConfirm", this, nameof(HandleHitConfirm));

		P1.Connect("LevelUp", this, nameof(OnLevelUp));
		P2.Connect("LevelUp", this, nameof(OnLevelUp));

		P1.Connect("HadoukenCommand", this, nameof(HadoukenCommand));
		P2.Connect("HadoukenCommand", this, nameof(HadoukenCommand));


		P1.otherPlayer = P2;
		P2.otherPlayer = P1;
		P1.internalPos = P1.Position * 100;
		P2.internalPos = P2.Position * 100;
		P1.Reset();
		P2.Reset();

		P1.CheckTurnAround();
		P2.CheckTurnAround();

		ResetHadoukens();
	}

	private int FindHadoukenIndexById(int id)
	{
		for (int i = 0; i < hadoukenCount; i++)
		{
			if (hadoukens[i].id == id)
				return i;
		}
		return -1;
	}

	private unsafe static void SerializeHadoukenState(ref HadoukenPart.HadoukenState value, byte* buffer)
	{
		*(HadoukenPart.HadoukenState*)buffer = value;
	}

	private unsafe static HadoukenPart.HadoukenState DeserializeHadoukenState(byte* buffer)
	{
		return *(HadoukenPart.HadoukenState*)buffer;
	}

	private unsafe void SerializeHadoukens(byte* arr)
	{
		for (int i = 0; i < hadoukenCount; i++)
		{
			var state = hadoukens[i].GetState();
			SerializeHadoukenState(ref state, arr + i * HADOUKENSTATESIZE);
		}
	}

	public unsafe GameState GetGameState()
	{
		gState.frame = Globals.frame;
		
		gState.P1State = P1.GetState();
		gState.P2State = P2.GetState();
		gState.totalHadoukens = hadoukenCount;
		fixed (byte* b = gState.hadoukenStates){
			SerializeHadoukens(b);
		}
		
		
		gState.hitStopRemaining = hitStopRemaining;

		// From gameScene
		gState.possibleEndingFrame = mainScene.possibleEndingFrame;
		gState.timeMode = (int)mainScene.currTime; // cast enum to int for storage


		return gState;
	}

	/// <summary>
	/// Return the serialized game state for GGPO to hold on to
	/// </summary>
	/// <returns></returns>
	public unsafe byte[] SaveGameState()
	{
		var buf = memoryPool.Get();
		var state = GetGameState();
		fixed (byte *p = buf)
		{
			SerializeGameState(ref state, p);
		}
			
		return buf;
	}

	private unsafe static void SerializeGameState(ref GameState value, byte* buffer)
	{
		*(GameState*)buffer = value;
	}

	private unsafe static GameState DeserializeGameState(byte* buffer)
	{
		return *(GameState*)buffer;
	}

	private unsafe void DeserializeHadoukens(byte* arr, int totalHadoukens)
{
	for (int i = 0; i < totalHadoukens; i++)
	{
		var hState = DeserializeHadoukenState(arr + i * HADOUKENSTATESIZE);
		int index = FindHadoukenIndexById(hState.id);
		if (index >= 0)
		{
			hadoukens[index].SetState(hState);
		}
	}
}

	private unsafe void SetGameState(GameState gState)
	{
		hitStopRemaining = gState.hitStopRemaining;
		P1.SetState(gState.P1State);
		P2.SetState(gState.P2State);
		DeserializeHadoukens(gState.hadoukenStates, gState.totalHadoukens);
		if (Globals.logOn)
			Globals.Log($"Setting gamestate for {hadoukenCount} hadoukens because of rollback to {gState.frame}");
		for (int i = 0; i < hadoukenCount; i++)
		{
			HadoukenPart h = hadoukens[i];
			if (h.creationFrame > gState.frame)
			{
				RemoveHadouken(h);
			}
		}

		CleanupHadoukens();

		mainScene.currTime = (GameScene.TimeStatus)gState.timeMode;
		mainScene.possibleEndingFrame = gState.possibleEndingFrame;

	}

	/// <summary>
	/// Load the game state provided by GGPO
	/// </summary>
	/// <param name="stream"></param>
	public unsafe void LoadGameState(byte[] stream)
	{
		GameState newState;
		fixed (byte* p = stream)
		{
			newState = DeserializeGameState(p);
		}

		SetGameState(newState);
	}

	 public unsafe bool RedesignCompareStates(byte[] buffer)
	{
		GameState oldState;
		fixed (byte* p = buffer)
		{
			oldState = DeserializeGameState(p);
		}
		string error = CompareGameStates(oldState, GetGameState());
		if (error != "")
		{
			GD.Print($"Frame {Globals.frame} {error}");
			return false;
		}
		return true;
	}

	private unsafe string CompareGameStates(GameState firstGs, GameState secondGs)
	{
		string errMsg = "";
		errMsg = AddError(errMsg, "Frame", firstGs.frame, secondGs.frame);
		errMsg = AddError(errMsg, "HitStopRemaining", firstGs.hitStopRemaining, secondGs.hitStopRemaining);
		string[] playerNames = { "p1", "p2" };
		int i = 0;
		foreach (Player.PlayerState[] pStates in new[]{ new[]{firstGs.P1State, secondGs.P1State}, new[]{firstGs.P2State, secondGs.P2State } })
		{
			//var currentStateNew = new string(pStates[0].currentState);
			errMsg = AddError(errMsg, playerNames[i] + " inBuf2Timer", pStates[0].inBuf2Timer, pStates[1].inBuf2Timer);
			//errMsg = AddError(errMsg, playerNames[i] + " currentState", pStates[0].currentState, pStates[1].currentState);
			errMsg = AddError(errMsg, playerNames[i] + " xPos", pStates[0].positionx, pStates[1].positionx);
			errMsg = AddError(errMsg, playerNames[i] + " yPos", pStates[0].positiony, pStates[1].positiony);
			errMsg = AddError(errMsg, playerNames[i] + " hitConnect", pStates[0].hitConnect, pStates[1].hitConnect);
			errMsg = AddError(errMsg, playerNames[i] + " stateFrame", pStates[0].frameCount, pStates[1].frameCount);
			errMsg = AddError(errMsg, playerNames[i] + " xvel", pStates[0].velocityx, pStates[1].velocityx);
			errMsg = AddError(errMsg, playerNames[i] + " yvel", pStates[0].velocityy, pStates[1].velocityy);
			errMsg = AddError(errMsg, playerNames[i] + " health", pStates[0].health, pStates[1].health);
			errMsg = AddError(errMsg, playerNames[i] + " proration", pStates[0].proration, pStates[1].proration);
			errMsg = AddError(errMsg, playerNames[i] + " stun remaining", pStates[0].stunRemaining, pStates[1].stunRemaining);
			i++;
		}

		/*errMsg = AddError(errMsg, $"Hadouken count", firstGs.hadoukenStates.Count, secondGs.hadoukenStates.Count);
		if (firstGs.hadoukenStates.Count != secondGs.hadoukenStates.Count) return errMsg;
		foreach (var hState1 in firstGs.hadoukenStates)
		{
			bool matched = false;
			foreach(var hState2 in secondGs.hadoukenStates)
			{
				if (hState2.name == hState1.name) {
					matched = true;
					errMsg = AddError(errMsg, $"Hadouken {hState1.name}" + " frame", hState1.frame, hState2.frame);
					errMsg = AddError(errMsg, $"Hadouken {hState1.name}" + " xPos", hState1.pos[0], hState2.pos[0]);
					errMsg = AddError(errMsg, $"Hadouken {hState1.name}" + " yPos", hState1.pos[1], hState2.pos[1]);
					errMsg = AddError(errMsg, $"Hadouken {hState1.name}" + " active", hState1.active, hState2.active);
				} 
			}
			if (!matched)
				errMsg += $"Hadouken {hState1.name} has no match";
		}*/
		return errMsg;
	}

	private string AddError(string errMsg, string msg, int val1, int val2)
	{

		int val1c = val1;
		int val2c = val2;
		if (val1c != val2c)
		{
			errMsg += $"{msg} does not match: 1: {val1}, 2: {val2} \n";
		}
		
		return errMsg;
	}

	private string AddError(string errMsg, string msg, bool val1, bool val2)
	{

		bool val1c = val1;
		bool val2c = val2;
		if (val1c != val2c)
		{
			errMsg += $"{msg} does not match: 1: {val1}, 2: {val2} \n";
		}

		return errMsg;
	}

	private string AddError(string errMsg, string msg, string val1, string val2)
	{
		if (val1 == val2)
		{
			return errMsg;
		}
		errMsg += $"{msg} does not match: 1: {val1}, 2: {val2} \n";
		return errMsg;
	}

	public void SyncTestUpdate(Godot.Collections.Array thisFrameInputs)
	{

		Update((int)thisFrameInputs[0], (int)thisFrameInputs[1]);
		

		if (Globals.frame > 1)
		{
			GameState firstGS = GetGameState();
			SetGameState(lastGs);
			Update((int)thisFrameInputs[0], (int)thisFrameInputs[1]);
			string result = CompareGameStates(firstGS, GetGameState());
			if (result != "")
			{
				GD.Print(result);
			}
			
		}
		lastGs = GetGameState();

		

	}

	/// <summary>
	/// For now, both players release all held keys
	/// </summary>
	public void EndGame()
	{
		P1.RemoveAllHeld();
		P2.RemoveAllHeld();
	}

	/// <summary>
	/// Updates the gamestate by one frame with the given inputs
	/// </summary>
	/// <param name="thisFrameInputs"></param>
	public void Update(int p1inps, int p2inps)
	{
		//GD.Print($"Advancing frame to {Frame}");
		
		AdvanceFrameAndHitstop();
		
		FrameAdvancePlayers(p1inps, p2inps);
		
	}

	private void AdvanceFrameAndHitstop()
	{
		
		if (hitStopRemaining > 0)
		{
			hitStopRemaining--;
		}

		for (int i = 0; i < hadoukenCount; i++)
		{
			hadoukens[i].AlwaysUpdate();
		}
		HandleHadoukenCollisions();
	}

	/// <summary>
	/// Note the movement step separated into two separate MoveSlide actions, for more accurate collision checking.
	/// </summary>
	private void FrameAdvancePlayers(int p1inp, int p2inp)
	{
		Player hostPlayer = P1;
		Player joinPlayer = P2;
		
		hostPlayer.FrameAdvanceInputs(hitStopRemaining, p1inp);
		joinPlayer.FrameAdvanceInputs(hitStopRemaining, p2inp);
		hostPlayer.AlwaysFrameAdvance();
		joinPlayer.AlwaysFrameAdvance();
		

		if (hitStopRemaining < 1)
		{
			hostPlayer.FrameAdvance();
			joinPlayer.FrameAdvance();
		
			for (int i = 0; i < hadoukenCount; i++)
			{
				hadoukens[i].FrameAdvance();
			}

			
			CleanupHadoukens();
			CheckHits(hostPlayer, joinPlayer);
			
			CheckFixCollision();
			hostPlayer.MoveSlideDeterministicTwo();
			joinPlayer.MoveSlideDeterministicTwo();
			CheckFixCollision();
			hostPlayer.RenderPosition();
			joinPlayer.RenderPosition();
		}
		
	}

	private void CheckHits(Player hostPlayer, Player joinPlayer)
	{
		hostPlayer.CheckHit();
		joinPlayer.CheckHit();
		hostPlayer.CalculateHit();
		joinPlayer.CalculateHit();
	}

	private void CleanupHadoukens()
	{
		for (int i = 0; i < deleteQueuedCount; i++)
		{
			CleanupHadouken(deleteQueued[i]);
		}
		deleteQueuedCount = 0;
	}


	/// <summary>
	/// Check if player collision boxes are colliding and adjust accordingly
	/// If the players have equal x values, this is done via height to allow jump ins
	/// </summary>
	private void CheckFixCollision()
	{
		while (CheckRects())
		{
			if (P1.internalPos.x < P2.internalPos.x)
			{
				P1.internalPos = new Vector2(P1.internalPos.x - 1, P1.internalPos.y);
				P2.internalPos = new Vector2(P2.internalPos.x + 1, P2.internalPos.y);
			}
			else if (P1.internalPos.x > P2.internalPos.x)
			{
				P1.internalPos = new Vector2(P1.internalPos.x + 1, P1.internalPos.y);
				P2.internalPos = new Vector2(P2.internalPos.x - 1, P2.internalPos.y);
			}
			else // same position, corner crossup likely
			{
				bool P1above = P1.internalPos.y < P2.internalPos.y;


				bool P1Hit = P1.currentState.wasHit;
				bool P2Hit = P2.currentState.wasHit;

				bool rightScreen = (P1.internalPos.x > 24000);

				if (rightScreen)
				{
					if ((P1above || P2Hit) && !(P1Hit))
					{
						P1.internalPos = new Vector2(P1.internalPos.x - 1, P1.internalPos.y);
						P2.internalPos = new Vector2(P2.internalPos.x + 1, P2.internalPos.y);
					}
					else
					{
						P1.internalPos = new Vector2(P1.internalPos.x + 1, P1.internalPos.y);
						P2.internalPos = new Vector2(P2.internalPos.x - 1, P2.internalPos.y);
					}
				}
				else
				{
					if ((P1above || P2Hit) && !(P1Hit))
					{
						P1.internalPos = new Vector2(P1.internalPos.x + 1, P1.internalPos.y);
						P2.internalPos = new Vector2(P2.internalPos.x - 1, P2.internalPos.y);
					}
					else
					{
						P1.internalPos = new Vector2(P1.internalPos.x - 1, P1.internalPos.y);
						P2.internalPos = new Vector2(P2.internalPos.x + 1, P2.internalPos.y);
					}
				}
			}
		}
	}
	private bool CheckRects()
	{
		if (!P1.CheckCollisionRectActive() || !P2.CheckCollisionRectActive())
			return false;
		Rect2 P1rect = P1.GetCollisionRect();
		Rect2 P2rect = P2.GetCollisionRect();
		return P1rect.Intersects(P2rect);
	}

	/// <summary>
	/// Reset the hitstop counter, called by player signals on hit
	/// </summary>
	public void HandleHitConfirm(int hitStop)
	{
		hitStopRemaining = hitStop;
	}

	public void SuperFreeze(string name)
	{
		if (name == "P1")
			P2.counterStopFrames = 30;
		else
			P1.counterStopFrames = 30;
		
	}

	public void OnLevelUp()
	{
		hitStopRemaining = levelUpHitStop;
	}

	public void ResetHadoukens()
	{
		for (int i = 0; i < hadoukenCount; i++)
		{
			HadoukenPart h = hadoukens[i];
			h.RemoveNum();
			h.freed = true;
			mainScene.RemoveChild(h);
			hadoukens[i] = null;
		}
		hadoukenCount = 0;
	}

	private void CleanupHadouken(HadoukenPart h)
	{
		int index = FindHadoukenIndexById(h.id);
		if (index < 0)
			return;

		// Swap remove
		int last = hadoukenCount - 1;
		hadoukens[index] = hadoukens[last];
		hadoukens[last] = null;
		hadoukenCount--;

		h.freed = true;
		h.RemoveNum();
		mainScene.RemoveChild(h);
	}


	public void NewHadouken(HadoukenPart h)
	{
		if (hadoukenCount >= MAX_HADOUKENS)
			return;

		h.creationFrame = Globals.frame;
		hadoukens[hadoukenCount++] = h;

		if (h is Snail)
			mainScene.ConnectSnail((Snail)h);

		if (Globals.logOn)
			Globals.Log($"Adding hadouken {h.Name} on frame {Globals.frame}");
	}

	public void HadoukenCommand(string playerName, string hadName, HadoukenPart.ProjectileCommand command)
	{
		for (int i = 0; i < hadoukenCount; i++)
		{
			HadoukenPart h = hadoukens[i];
			if (h.hadoukenType == hadName && h.ownerName == playerName)
			{
				h.ReceiveCommand(command);
			}
		}
	}


	public void RemoveHadouken(HadoukenPart h)
	{
		if (deleteQueuedCount < MAX_HADOUKENS)
		{
			deleteQueued[deleteQueuedCount++] = h;
			h.ShouldNotExist();
		}
	}

	private void HandleHadoukenCollisions()
	{
		for (int i = 0; i < hadoukenCount; i++)
		{
			var h = hadoukens[i];
			if (!h.active) continue;

			for (int j = i + 1; j < hadoukenCount; j++)
			{
				var other = hadoukens[j];
				if (!other.active) continue;
				if (h.ownerName == other.ownerName) continue;

				if (h.GetCollisionRect().Intersects(other.GetCollisionRect()))
				{
					h.HandleOverlap();
					other.HandleOverlap();
				}
			}
		}
	}

	
}
