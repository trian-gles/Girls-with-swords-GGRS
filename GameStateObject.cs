using Godot;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Linq;

/// <summary>
/// This object controls all the actual management of gameplay, and passes this information to GGPO
/// </summary>
public class GameStateObject : Node
{
	public Player P1;
	public Player P2;
	private MainScene mainScene; // this seems like a bad idea, but the gsobj needs to add and remove nodes to the mainscene

	private bool hosting;

	public int Frame = 0;

	private int hitStopRemaining = 0;

	private int maxHitStop = 14;

	private GameState resetState;

	/// <summary>
	/// Used for synctesting
	/// </summary>
	private GameState lastGs;

	/// <summary>
	/// Stores all vital data about positions in the game in a single struct
	/// </summary>
	[Serializable]
	private struct GameState
	{
		public int frame { get; set; }
		public Player.PlayerState P1State { get; set; }
		public Player.PlayerState P2State { get; set; }

		public List<HadoukenPart.HadoukenState> hadoukenStates { get; set; }
		public int hitStopRemaining { get; set; }
	}

	private Dictionary<string, HadoukenPart> hadoukens;
	private List<HadoukenPart> deleteQueued;
	public void config(Player P1, Player P2, MainScene mainScene, bool hosting)
	{
		GD.Print("Starting GameState config");
		this.P1 = P1;
		this.P2 = P2;
		this.mainScene = mainScene;
		this.hosting = hosting;
		P1.Connect("HitConfirm", this, nameof(HitStop));
		P2.Connect("HitConfirm", this, nameof(HitStop));

		P1.otherPlayer = P2;
		P2.otherPlayer = P1;
		P1.internalPos = P1.Position * 100;
		GD.Print(P1.internalPos);
		P2.internalPos = P2.Position * 100;
		GD.Print(P2.internalPos);
		P1.CheckTurnAround();
		P2.CheckTurnAround();

		hadoukens = new Dictionary<string, HadoukenPart>(); // indexed as {name, object}
		deleteQueued = new List<HadoukenPart>(); // I can't remove items from a list while enumerating that list so I use this instead
		GD.Print("GameState config finished");
		// Use this below code to make P2 hold a button
		// P2.SetUnhandledInputs(new List<char[]>() { new char[] { '8', 'p' } });
		resetState = GetGameState();
	}
	private byte[] Serialize<T>(T data)
	where T : struct
	{
		var formatter = new BinaryFormatter();
		var stream = new MemoryStream();
		formatter.Serialize(stream, data);
		return stream.ToArray();
	}
	private T Deserialize<T>(byte[] array)
		where T : struct
	{
		var stream = new MemoryStream(array);
		var formatter = new BinaryFormatter();
		return (T)formatter.Deserialize(stream);
	}
	private GameState GetGameState()
	{
		GameState gState = new GameState();
		gState.frame = Frame;
		
		gState.P1State = P1.GetState();
		gState.P2State = P2.GetState();
		gState.hadoukenStates = new List<HadoukenPart.HadoukenState>();
		foreach (var entry in hadoukens)
		{
			gState.hadoukenStates.Add(entry.Value.GetState());
		}
		gState.hitStopRemaining = hitStopRemaining;

		return gState;
	}
	private StreamPeerBuffer SerializeGamestate(GameState gameState)
	{
		
		byte[] arr = Serialize(gameState);

		

		var stream = new StreamPeerBuffer();
		stream.Clear();
		stream.Put32(0); // for the checksum
		stream.Put32(arr.Length); // to know how much data to pull out

		foreach (byte b in arr)
		{
			stream.PutU8(b);
		}

		int checkSum = CalcFletcher32(stream);
		stream.Seek(0);
		stream.Put32(checkSum);

		return stream;

	}

	/// <summary>
	/// Return the serialized game state for GGPO to hold on to
	/// </summary>
	/// <returns></returns>
	public StreamPeerBuffer SaveGameState()
	{
		return SerializeGamestate(GetGameState());
	}
	private int CalcFletcher32(StreamPeerBuffer stream)
	{
		int sum1 = 0;
		int sum2 = 0;
		var index = stream.DataArray;
		for (int i = 0; i < index.Length; i++)
		{
			sum1 = (sum1 + index[i] % 0xffff);
			sum2 = (sum1 + sum2) % 0xffff;

		}
		return ((sum2 << 16) | sum1);
	}

	private string CompareGameStates(GameState firstGs, GameState secondGs)
    {
		string errMsg = "";
		errMsg = AddError(errMsg, "Frame", firstGs.frame, secondGs.frame);
		errMsg = AddError(errMsg, "HitStopRemaining", firstGs.hitStopRemaining, secondGs.hitStopRemaining);
		string[] playerNames = { "p1", "p2" };
		int i = 0;
		foreach (Player.PlayerState[] pStates in new[]{ new[]{firstGs.P1State, secondGs.P1State}, new[]{firstGs.P2State, secondGs.P2State } })
        {
			errMsg = AddError(errMsg, playerNames[i] + " inBuf2Timer", pStates[0].inBuf2Timer, pStates[1].inBuf2Timer);
			errMsg = AddError(errMsg, playerNames[i] + " currentState", pStates[0].currentState, pStates[1].currentState);
			errMsg = AddError(errMsg, playerNames[i] + " xPos", pStates[0].position[0], pStates[1].position[0]);
			errMsg = AddError(errMsg, playerNames[i] + " yPos", pStates[0].position[1], pStates[1].position[1]);
			errMsg = AddError(errMsg, playerNames[i] + " currState", pStates[0].currentState, pStates[1].currentState);
			errMsg = AddError(errMsg, playerNames[i] + " hitConnect", pStates[0].hitConnect, pStates[1].hitConnect);
			errMsg = AddError(errMsg, playerNames[i] + " stateFrame", pStates[0].frameCount, pStates[1].frameCount);
			errMsg = AddError(errMsg, playerNames[i] + " xvel", pStates[0].velocity[0], pStates[1].velocity[0]);
			errMsg = AddError(errMsg, playerNames[i] + " yvel", pStates[0].velocity[1], pStates[1].velocity[1]);
			errMsg = AddError(errMsg, playerNames[i] + " health", pStates[0].health, pStates[1].health);
			i++;
        }

		for (int j = 0; j < firstGs.hadoukenStates.Count; j++)
        {
			errMsg = AddError(errMsg, $"Hadouken {j}" + " xPos", firstGs.hadoukenStates[j].pos[0], firstGs.hadoukenStates[j].pos[0]);
		}
        



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

	private void SetGameState(GameState gState)
	{
		Frame = gState.frame;
		//GD.Print($"Rolling back to frame {Frame}");
		Globals.frame = Frame;
		hitStopRemaining = gState.hitStopRemaining;
		P1.SetState(gState.P1State);
		P2.SetState(gState.P2State);
		foreach (HadoukenPart.HadoukenState hState in gState.hadoukenStates) // only update each saved hadouken if it still exists
		{
			if (hadoukens.ContainsKey(hState.name))
			{
				hadoukens[hState.name].SetState(hState);
			}
		}
		foreach (var entry in hadoukens)
		{
			HadoukenPart thisHadouken = entry.Value;

			if (thisHadouken.creationFrame > Frame)
			{
				deleteQueued.Add(thisHadouken);
			}
		}
		CleanupHadoukens();

	}
	private GameState DeserializeGamestate(StreamPeerBuffer stream)
	{
		stream.Seek(0);
		stream.Get32();
		int length = stream.Get32();
		byte[] arr = new byte[length];
		for (int i = 0; i < length; i++)
		{
			arr[i] = stream.GetU8();
		}

		var retrievedGamestate = Deserialize<GameState>(arr);

		return retrievedGamestate;
	}

	/// <summary>
	/// Load the game state provided by GGPO
	/// </summary>
	/// <param name="stream"></param>
	public void LoadGameState(StreamPeerBuffer stream)
	{
		SetGameState(DeserializeGamestate(stream));
	}

	public void SyncTestUpdate(Godot.Collections.Array thisFrameInputs)
    {
		//GD.Print($"Synctesting on frame {Frame}");

		Update(thisFrameInputs);
		

		if (Frame > 1)
		{
			GameState firstGS = GetGameState();
			SetGameState(lastGs);
			Update(thisFrameInputs);
			string result = CompareGameStates(firstGS, GetGameState());
			if (result != "")
            {
				GD.Print(result);
			}
			
		}
		lastGs = GetGameState();

		

	}

	/// <summary>
	/// Updates the gamestate by one frame with the given inputs
	/// </summary>
	/// <param name="thisFrameInputs"></param>
	public void Update(Godot.Collections.Array thisFrameInputs)
	{
		Frame++;
		//GD.Print($"Advancing frame to {Frame}");
		Globals.frame++;
		List<char[]> p1inps;
		List<char[]> p2inps;

		if (hosting)
		{
			p1inps = ConvertInputs((int)thisFrameInputs[0]);
			
			
			p2inps = ConvertInputs((int)thisFrameInputs[1]);
		}
		else
		{
			p2inps = ConvertInputs((int)thisFrameInputs[0]);
			p1inps = ConvertInputs((int)thisFrameInputs[1]);
		}
		

		AdvanceFrameAndHitstop();
		FrameAdvancePlayers(p1inps, p2inps);
		

	}

	/// <summary>
	/// Takes the inputs int and turns it back into List<char[]>
	/// </summary>
	/// <param name="inputs"></param>
	/// <returns></returns>
	private List<char[]> ConvertInputs(int inputs)
	{
		var convertedInputs = new List<char[]>();
		if (inputs == 0)
		{
			return convertedInputs; //no inputs, don't do anything
		}

		string stringInputs = inputs.ToString();
		int count = stringInputs.Length / 2;
		for (int i = 0; i < count; i++)
		{
			int key = int.Parse(stringInputs[i * 2].ToString());
			int press = int.Parse(stringInputs[i * 2 + 1].ToString());
			char[] convertedInput = ConvertInput(key, press);
			convertedInputs.Add(convertedInput);
		}
		return convertedInputs;

	}

	public void ResetGameState()
    {
		SetGameState(resetState);
    }

	private char[] ConvertInput(int key, int press)
	{
		var input = new char[2];
		if (key == (int)Globals.Inputs.UP)
		{
			input[0] = '8';
		}
		else if (key == (int)Globals.Inputs.DOWN)
		{
			input[0] = '2';
		}
		else if (key == (int)Globals.Inputs.LEFT)
		{
			input[0] = '4';
		}
		else if (key == (int)Globals.Inputs.RIGHT)
		{
			input[0] = '6';
		}
		else if (key == (int)Globals.Inputs.PUNCH)
		{
			input[0] = 'p';
		}
		else if (key == (int)Globals.Inputs.KICK)
		{
			input[0] = 'k';
		}
		else if (key == (int)Globals.Inputs.SLASH)
		{
			input[0] = 's';
		}

		if (press == (int)Globals.Press.PRESS)
		{
			input[1] = 'p';
		}
		else
		{
			input[1] = 'r';
		}

		return input;
	}

	public void InformPlayersRollback()
	{
		P1.Rollback(Frame);
		P2.Rollback(Frame);
	}
	private void AdvanceFrameAndHitstop()
	{
		
		if (hitStopRemaining > 0)
		{
			hitStopRemaining--;
		}
	}

	/// <summary>
	/// Note the movement step separated into two separate MoveSlide actions, for more accurate collision checking.
	/// </summary>
	private void FrameAdvancePlayers(List<char[]>p1inp, List<char[]>p2inp)
	{
		P1.FrameAdvanceInputs(hitStopRemaining, p1inp);
		P2.FrameAdvanceInputs(hitStopRemaining, p2inp);
		P1.AlwaysFrameAdvance();
		P2.AlwaysFrameAdvance();

		if (hitStopRemaining < 1)
		{
			P1.FrameAdvance();
			P2.FrameAdvance();
			CheckFixCollision();
			P1.MoveSlideDeterministicTwo();
			P2.MoveSlideDeterministicTwo();
			CheckFixCollision();
			P1.RenderPosition();
			P2.RenderPosition();
		}
		foreach (var entry in hadoukens)
		{
			entry.Value.FrameAdvance();
		}

		CleanupHadoukens();
		
	}

	private void CleanupHadoukens()
    {
		foreach (HadoukenPart h in deleteQueued)
		{
			CleanupHadouken(h);
		}
		if (deleteQueued.Count > 0)
		{
			deleteQueued = new List<HadoukenPart>();
		}
	}

	/// <summary>
	/// Check if player collision boxes are colliding and adjust accordingly
	/// </summary>
	private void CheckFixCollision()
	{
		while (CheckRects())
		{
			if (P1.internalPos < P2.internalPos)
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
	}
	private bool CheckRects()
	{
		Rect2 P1rect = P1.GetCollisionRect();
		Rect2 P2rect = P2.GetCollisionRect();
		return P1rect.Intersects(P2rect);
	}

	/// <summary>
	/// Reset the hitstop counter, called by player signals on hit
	/// </summary>
	public void HitStop()
	{
		hitStopRemaining = maxHitStop;
		GD.Print("HitStop");
	}

	private void CleanupHadouken(HadoukenPart h) //completely remove a Hadouken
	{
		hadoukens.Remove(h.Name);
		h.QueueFree();
		mainScene.RemoveChild(h);
		
	}
	public void NewHadouken(HadoukenPart h)
	{
		hadoukens.Add(h.Name, h); 
		//GD.Print($"New hadouken on frame {Frame}");
		h.creationFrame = Frame;
	}

	public void RemoveHadouken(HadoukenPart h)
	{
		deleteQueued.Add(h);
	}
	
}
