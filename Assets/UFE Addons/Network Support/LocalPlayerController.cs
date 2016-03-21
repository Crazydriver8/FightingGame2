#if !UNITY_WEBGL
using UnityEngine;
using System;
using System.Collections.Generic;

public class LocalPlayerController : NetworkController {
	#region public instance properties
	public int frameDelay {
		get{
			return this._frameDelay;
		}
		set{
			this._frameDelay = Mathf.Clamp(value, -1, UFE.config.networkOptions.maxFrameDelay);
		}
	}
	#endregion

	#region protected instance fields
	protected int _frameDelay = 3;
	protected int fixedUpdatesSinceLastUpdate = 0;
	protected int? requestedOption = null;
	protected bool synchronizingRandomSeed;
	#endregion

	#region public instance methods
	public virtual void RequestOptionSelection(int option){
		this.requestedOption = option;
	}

    /*public virtual int CalculateFrameDelay() {
        float ping = UFE.multiplayerAPI.Connections > 0 ? Mathf.Max(UFE.multiplayerAPI.GetLastPing(), 1f) : 1f;
        int frameDelay = Mathf.CeilToInt(0.001f * ping / (float)(UFE.config.fps));
        //Debug.Log("Ping: " + ping + " ---> Frame Delay: " + frameDelay);
        return frameDelay;
    }*/

	#endregion

	#region public override methods
	public override void Initialize (IEnumerable<InputReferences> inputs, int frameDelay){
		this.frameDelay = frameDelay;
		this.synchronizingRandomSeed = false;
		base.Initialize(inputs, this.frameDelay + 1);
	}
	#endregion

	#region protected instance methods
	protected virtual void SendNetworkPackage(){
		int count = this.inputBuffer.Count;

		FrameInput[] buffer = new FrameInput[count];
		for (int i = 0; i < count; ++i){
			float horizontalAxis = 0f;
			float horizontalAxisRaw = 0f;
			float verticalAxis = 0f;
			float verticalAxisRaw = 0f;
			NetworkButtonPress buttons = NetworkButtonPress.None;

			foreach (KeyValuePair<InputReferences, InputEvents> pair in this.inputBuffer[i]){
				InputReferences inputReference = pair.Key;
				InputEvents inputEvent = pair.Value;

				if (inputReference.inputType == InputType.HorizontalAxis){
					horizontalAxis = inputEvent.axis;
					horizontalAxisRaw = inputEvent.axisRaw;
				}else if (inputReference.inputType == InputType.VerticalAxis){
					verticalAxis = inputEvent.axis;
					verticalAxisRaw = inputEvent.axisRaw;
				}else if (inputReference.inputType == InputType.Button && inputEvent.button){
					buttons |= inputReference.engineRelatedButton.ToNetworkButtonPress();
				}
			}

			buffer[i] = new FrameInput(
				horizontalAxis, 
				horizontalAxisRaw, 
				verticalAxis, 
				verticalAxisRaw,
				buttons,
				this.optionSelections[i] == null ? -2 : this.optionSelections[i].Value
			);
		}


		InputBufferMessage msg = new InputBufferMessage(this.player, this.currentFrame, buffer);
		if (UFE.config.networkOptions.fakeNetwork){
			RemotePlayerController remoteController=UFE.GetController(UFE.GetRemotePlayer()) as RemotePlayerController;
			remoteController.OnMessageReceived(msg.Serialize(), new NetworkMessageInfo());
		}else{
			UFE.multiplayerAPI.SendNetworkMessage(msg);
		}
	}
	#endregion
	
	#region override MonoBehaviour methods
	public override void DoFixedUpdate(){
		if (UFE.synchronizedRandomSeed){
			++this.fixedUpdatesSinceLastUpdate;
			base.DoFixedUpdate();
		}
	}

	public override void DoUpdate(){
		if (UFE.synchronizedRandomSeed){
			int frameDelayOffset = (this.frameDelay >= 0 ? this.frameDelay : this.CalculateFrameDelay()) + 2;
			this.synchronizingRandomSeed = false;
			
			if(
				this.inputReferences != null &&
				UFE.isConnected &&
				(
					this.currentFrameOffset < 2
					||
					this.inputBuffer.Count < frameDelayOffset
					|| 
					this.fixedUpdatesSinceLastUpdate > 0 &&
					UFE.GetPlayer1Controller().isReady && 
					UFE.GetPlayer2Controller().isReady
				)
			){
				//-----------------------------------------------------------------------------------------------------
				// In that case, read the player input
				//-----------------------------------------------------------------------------------------------------
				this.currentFrameInputs.Clear();
				base.DoUpdate();

				foreach (InputReferences input in this.inputReferences){
					if (this.cpuController != null && this.isCPU && UFE.gameRunning && !UFE.isPaused()){
						this.currentFrameInputs[input] = this.cpuController.ReadInput(input);
					}else if (this.humanController != null){
						this.currentFrameInputs[input] = this.humanController.ReadInput(input);
					}else{
						this.currentFrameInputs[input] = InputEvents.Default;
					}
				}
				
				//-----------------------------------------------------------------------------------------------------
				// Calculate the start and the end frame where the current input should be copied.
				//-----------------------------------------------------------------------------------------------------
				int targetFrame = (int)(this.currentFrameOffset) + frameDelayOffset;
				if (this.inputBuffer.Count < targetFrame){
					//-------------------------------------------------------------------------------------------------
					// Check if there are some "empty slots" between the current frame and the target frame
					// and fill those "slots" with the "Default Input Event".
					//-------------------------------------------------------------------------------------------------
					while (this.inputBuffer.Count + this.fixedUpdatesSinceLastUpdate < targetFrame - 1){
//						Debug.Log(
//							UFE.currentNetworkFrame + " >>> " +
//							this.inputBuffer.Count + " + " +
//							this.fixedUpdatesSinceLastUpdate + " < " +
//							(targetFrame - 1)
//						);

						int? previousOption = this.optionSelections[this.optionSelections.Count - 1];
						Dictionary<InputReferences, InputEvents> previous = this.inputBuffer[this.inputBuffer.Count - 1];
						Dictionary<InputReferences, InputEvents> current = new Dictionary<InputReferences, InputEvents>();

						foreach (InputReferences input in this.inputReferences){
							current[input] = new InputEvents(previous[input]);
						}

						this.inputBuffer.Add(previous);
						this.optionSelections.Add(previousOption != null ? new int?(previousOption.Value) : null);
					}
					
					//-------------------------------------------------------------------------------------------------
					// Read the input for the target frames, checking if we have already entered the input 
					// for those frames (for example: if the game is paused waiting for the other player).
					//-------------------------------------------------------------------------------------------------
					for (int i = 0; i < this.fixedUpdatesSinceLastUpdate; ++i){
						Dictionary<InputReferences, InputEvents> dict = new Dictionary<InputReferences, InputEvents>();
						foreach (InputReferences input in this.inputReferences){ 
							dict[input] = this.currentFrameInputs[input];
						}

						this.inputBuffer.Add(dict);
						this.optionSelections.Add(this.requestedOption);
						this.requestedOption = null;
					}
				}
			}
			
			//---------------------------------------------------------------------------------------------------------
			// It doesn't matter whether we have read the inputs for a new frame or not, if we haven't lose the 
			// connection, we send (or resend) the input buffer to the other player every frame
			//---------------------------------------------------------------------------------------------------------
			if (UFE.isConnected){
				this.SendNetworkPackage();
			}
			
			//-------------------------------------------------------------------------------------------------------------
			// Finally, start counting the number of FixedUpdate() calls before the next Update() call.
			//-------------------------------------------------------------------------------------------------------------
			this.fixedUpdatesSinceLastUpdate = 0;
		}else if (!this.synchronizingRandomSeed && this.player == 1){
			// If the random seed hasn't been synchronized and the local player is the host of the game
			this.synchronizingRandomSeed = true;
			int randomSeed = (int)DateTime.UtcNow.Ticks;

			if (UFE.config.networkOptions.fakeNetwork){
				UFE.SetSynchronizedRandomSeed(randomSeed);
			}else{
				UFE.multiplayerAPI.SendNetworkMessage(
					new RandomSeedSynchronizationMessage(this.player, this.currentFrame, randomSeed)
				);
			}
		}
	}
	#endregion
}
#endif