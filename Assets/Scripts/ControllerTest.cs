using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class ControllerTest : NetworkBehaviour {
	// Information about the player
	struct PlayerState {
		public int moveNum;
		public float x;
		public float z;
	}


	/* Fields */

	// Controls
	private enum MoveInfo {UD, LR};
	private enum FightInfo { BASIC, STRONG, EVADE, GRAB, COMBO };
	public string[] moveCommands = { "Horizontal", "Vertical" };
	public string[] combatCommands = { "Basic", "Strong", "Evade", "Grab", "Combo" };

	// Information about the player, synchronized across all clients
	[SyncVar(hook = "OnServerStateChanged")] PlayerState state;
	Queue pendingMoves;
	PlayerState predictedState;

	/* Instantiation through MonoBehaviour */

	void Awake () {
		InitState ();
	}

	void Start()
	{
		if (isLocalPlayer) {
			pendingMoves = new Queue();
			UpdatePredictedState ();
		}
		if (gameObject.name == "Player1") {
			
		}
		SyncState (true);
	}
	
	// Update is called once per frame
	void Update () {
		if (isLocalPlayer) {
			foreach (string input in moveCommands) {
				float val = Input.GetAxis (input);
				pendingMoves.Enqueue (input);
				UpdatePredictedState ();
				CmdMove (input, val);
			}
		}

		SyncState (false);
	}


	/* Helper methods for playing the game */

	// Handles movement on the server
	[Command] void CmdMove(string movement, float val)
	{
		state = Move (state, movement, val);
	}
	
	// Set the initial state of the player
	[Server] void InitState () {
		GameObject temp = GameObject.Find ("PlayerWithHitbox(Clone)");
		if (temp == null) {
			state = new PlayerState {
				x = 1.0f,
				z = 0.0f
			};
		} else {
			state = new PlayerState {
				x = -1.0f,
				z = 0.0f
			};
		}
	}

	PlayerState Move(PlayerState curr, string movement, float val)
	{
		float deltaX = 0.0f,
			  deltaZ = 0.0f;

		switch (movement) {
			case "Horizontal":
				deltaX = val;
				break;

			case "Vertical":
				deltaZ = val;
				break;

			default:
				break;
		}
		float newX = deltaX + curr.x;
		float newZ = deltaZ + curr.z;
		return new PlayerState {
			//x = newX < -7 ? -7 : newX >= maxX ? (maxX - 1) : newX,
			//z = newZ < -7 ? -7 : newZ >= maxZ ? (maxZ - 1) : newZ
			x = newX,
			z = newZ
		};
	}

	void OnServerStateChanged (PlayerState newState) {
		state = newState;
		if (pendingMoves != null) {
			while (pendingMoves.Count > (predictedState.moveNum - state.moveNum)) {
				pendingMoves.Dequeue ();
			}
			UpdatePredictedState ();
		}
	}

	// Synchronizes state across clients
	void SyncState(bool init)
	{
		PlayerState stateToRender = isLocalPlayer ? predictedState : state;
		Vector3 target = spacing * (stateToRender.x * Vector3.right + stateToRender.z * Vector3.forward);
		transform.position = init ? target : Vector3.Lerp (transform.position, target, easing); 
	}

	void UpdatePredictedState() {
		predictedState = state;
		foreach (string input in pendingMoves) {
			float val = Input.GetAxis (input);
			predictedState = Move (predictedState, input, val);
		}
	}

	//int maxX = 7;
	//int maxZ = 7;
	float easing = 0.1f;
	float spacing = 0.1f;
}
