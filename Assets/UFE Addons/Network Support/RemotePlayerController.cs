#if !UNITY_WEBGL
using UnityEngine;
using System.Collections.Generic;

public class RemotePlayerController : NetworkController {
	#region protected instance fields
	protected List<byte[]> receivedMessages = new List<byte[]>();
	protected Dictionary<long, GameState> gameState = new Dictionary<long, GameState>();
	#endregion

	#region public instance methods: RPC definitions
	public virtual void OnMessageReceived(byte[] serializedMessage, NetworkMessageInfo msgInfo){
		this.receivedMessages.Add(serializedMessage);
	}
	#endregion

    public GUIText debugger;
	
	#region public override methods
	public override void DoFixedUpdate (){
		//-------------------------------------------------------------------------------------------------------------
		// First, store the player positions at the current frame (if they aren't already stored)
		// because we will use them later for synchronization purpose
		//-------------------------------------------------------------------------------------------------------------
		if (this.inputReferences != null && UFE.GetPlayer1Controller().isReady && UFE.GetPlayer2Controller().isReady){
			ControlsScript p1 = UFE.GetPlayer1ControlsScript();
			ControlsScript p2 = UFE.GetPlayer2ControlsScript();
			
			if(
				p1 != null && 
				p2 != null && 
				//UFE.currentNetworkFrame % 100 == 0 && 
				!this.gameState.ContainsKey(UFE.currentNetworkFrame)
			){
				//-----------------------------------------------------------------------------------------------------
				// Send a synchronization message every few frames
				//-----------------------------------------------------------------------------------------------------
				GameState state = new GameState(p1.transform.position, p2.transform.position);
				this.gameState[UFE.currentNetworkFrame] = state;
				UFE.multiplayerAPI.SendNetworkMessage(new SynchronizationMessage(this.player,UFE.currentNetworkFrame,state));
				//Debug.LogWarning("Store State: " + state + "\t(Frame = " + UFE.currentNetworkFrame + ")");
			}
		}

		//-------------------------------------------------------------------------------------------------------------
		// Execute the parent's method
		//-------------------------------------------------------------------------------------------------------------
		base.DoFixedUpdate ();
	}

	public override void DoUpdate(){
		//-------------------------------------------------------------------------------------------------------------
		// After that, process network messages received during the last frame
		//-------------------------------------------------------------------------------------------------------------
		foreach (byte[] serializedMessage in this.receivedMessages){
			if (serializedMessage != null && serializedMessage.Length > 0){
				NetworkMessageType messageType = (NetworkMessageType)serializedMessage[0];
				if (messageType == NetworkMessageType.InputBuffer){
					this.ProcessInputBufferMessage(new InputBufferMessage(serializedMessage));
				}else if (messageType == NetworkMessageType.RandomSeedSynchronization){
					this.ProcessRandomSeedSynchronizationMessage(new RandomSeedSynchronizationMessage(serializedMessage));
				}else if (messageType == NetworkMessageType.RandomSeedSynchronized){
					this.ProcessRandomSeedSynchronizedMessage(new RandomSeedSynchronizedMessage(serializedMessage));
				}else if (messageType == NetworkMessageType.Syncronization){
					this.ProcessSynchronizationMessage(new SynchronizationMessage(serializedMessage));
				}
			}
		}
		this.receivedMessages.Clear();
	}

	public override void Initialize(IEnumerable<InputReferences> inputs, int bufferSize){
		UFE.multiplayerAPI.OnMessageReceived -= this.OnMessageReceived;
		UFE.multiplayerAPI.OnMessageReceived += this.OnMessageReceived;

		this.gameState.Clear();
		this.receivedMessages.Clear();
		base.Initialize(inputs, bufferSize);

		
		if (UFE.config.networkOptions.fakeNetwork){
			Dictionary<InputReferences, InputEvents> ev = new Dictionary<InputReferences, InputEvents>();
			foreach (InputReferences inputReference in this.inputReferences){
				ev[inputReference] = InputEvents.Default;
			}
			
			for (int i = 0; i < bufferSize; ++i){
				this.inputBuffer.Add(ev);
				this.optionSelections.Add(null);
			}
		}

        // DEBUGGER
        GameObject debuggerGO = new GameObject("DebuggerNet");
        debugger = debuggerGO.AddComponent<GUIText>();
        debugger.pixelOffset = new Vector2(55 * ((float)Screen.width / 1280), 180 * ((float)Screen.height / 720));
        debugger.text = "";
        debugger.anchor = TextAnchor.UpperLeft;
        debugger.color = Color.black;
        debugger.richText = true;
        debugger.enabled = false;
	}
	#endregion

	#region protected instance methods
	protected virtual void ProcessInputBufferMessage(InputBufferMessage msg){
		if (msg.Data != null){
			int offset = (int)(msg.CurrentFrame - this.currentFrame);
			FrameInput frame;
			int index;
			
			for (int i = 0; i < msg.Data.Length; ++i){
				frame = msg.Data[i];
				index = i + offset;
				
				if (index >= 0){
					if (index >= this.inputBuffer.Count){
						this.inputBuffer.Add (new Dictionary<InputReferences, InputEvents>());
						this.optionSelections.Add(frame.selectedOption <= -2 ? null : new int?(frame.selectedOption));
					}
					
					foreach (InputReferences input in this.inputReferences){
						if (input.inputType == InputType.HorizontalAxis){
							this.inputBuffer[index][input] = new InputEvents(
								frame.horizontalAxis,
								frame.horizontalAxisRaw
							);
						}else if (input.inputType == InputType.VerticalAxis){
							this.inputBuffer[index][input] = new InputEvents(
								frame.verticalAxis,
								frame.verticalAxisRaw
							);
						}else{
							this.inputBuffer[index][input] = new InputEvents(
								(frame.buttons & input.engineRelatedButton.ToNetworkButtonPress()) != 0
							);
						}
					}
				}
			}
		}
	}

	protected virtual void ProcessRandomSeedSynchronizationMessage(RandomSeedSynchronizationMessage msg){
		RandomSeedSynchronizedMessage response = new RandomSeedSynchronizedMessage(this.player, msg.CurrentFrame, msg.Data);
		if (UFE.config.networkOptions.fakeNetwork){
			this.ProcessRandomSeedSynchronizedMessage(response);
		}else if (this.player == 1){
			UFE.multiplayerAPI.SendNetworkMessage(response);
			UFE.SetSynchronizedRandomSeed(msg.Data);
		}
	}

	protected virtual void ProcessRandomSeedSynchronizedMessage(RandomSeedSynchronizedMessage msg){
		if (this.player == 2 || UFE.config.networkOptions.fakeNetwork){
			UFE.SetSynchronizedRandomSeed(msg.Data);
		}
	}

	protected virtual void ProcessSynchronizationMessage(SynchronizationMessage msg){
		GameState state;
		if (this.gameState.TryGetValue(msg.CurrentFrame, out state)){

            if (UFE.config.debugOptions.networkToggle) {
                debugger.enabled = true;
                debugger.text = "";
                if (UFE.config.debugOptions.ping) debugger.text += "Ping:" + UFE.multiplayerAPI.GetLastPing() + " ms\n";
                if (UFE.config.debugOptions.frameDelay) debugger.text += "Frame Delay:" + this.CalculateFrameDelay() + "\n";
                if (UFE.config.debugOptions.currentLocalFrame) debugger.text += "Local Frame:" + UFE.currentNetworkFrame + "\n";
                if (UFE.config.debugOptions.currentNetworkFrame) debugger.text += "Network Frame:" + msg.CurrentFrame + "\n";
            } else {
                debugger.enabled = false;
            }

			//if (state.Equals(msg.Data)){
            if (Vector3.Distance(msg.Data.p1Position, state.p1Position) <= 0.1f){
				//-----------------------------------------------------------------------------------------------------
				// If the game state received from the network message is equal to the stored state,
				// everything is ok, we can delete obsolete messages.
				//-----------------------------------------------------------------------------------------------------
//				Debug.Log(
//					"Synchroned!\tFrame = " + msg.CurrentFrame + 
//					"\nExpected State: " + state + 
//					"\nReceived State: " + msg.Data
//				);

				List<long> obsoleteNetworkFrames = new List<long>();
				foreach (long networkFrame in this.gameState.Keys){
					if (networkFrame <= msg.CurrentFrame) {
						obsoleteNetworkFrames.Add(msg.CurrentFrame);
					}
				}

				for (int i = 0; i < obsoleteNetworkFrames.Count; ++i){
					this.gameState.Remove(obsoleteNetworkFrames[i]);
				}
			}else if (UFE.config.networkOptions.disconnectOnDesync) {
				//-----------------------------------------------------------------------------------------------------
				// Otherwise, a desynchronization has happened, so we should exit from the network game.
				//-----------------------------------------------------------------------------------------------------

                Debug.LogError(
					"Synchronization Lost!\tFrame = " + msg.CurrentFrame + 
					"\nExpected State: " + state + 
					"\nReceived State: " + msg.Data
				);

    			if (UFE.multiplayerAPI.IsClient()){
					UFE.multiplayerAPI.Disconnect();
				}else if (UFE.multiplayerAPI.IsServer()){
					UFE.multiplayerAPI.StopServer();
				}
			}
		}
	}
	#endregion
}
#endif
