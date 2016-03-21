#if !UNITY_WEBGL
using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(NetworkView))]
public abstract class NetworkController : UFEController {
	#region public instance properties
	public long currentFrame {get; set;}
	#endregion

	#region public override instance properties
	public override bool isReady{
		get{
			bool ready = 
				UFE.isConnected &&
				this.inputReferences != null &&
				this.inputBuffer != null &&
				this.inputBuffer.Count > this.currentFrameOffset &&
				this.currentFrameOffset > 0;

			return ready;
		}
	}

	public override Dictionary<InputReferences, InputEvents> inputs{
		get{
			return inputBuffer[currentFrameOffset];
		}
		protected set{
			if (currentFrameOffset < 0 || currentFrameOffset >= inputBuffer.Count){
				Debug.LogWarning(
					"Index = " + UFE.currentNetworkFrame + " - " + this.currentFrame + " = " + currentFrameOffset + "\n" +
					"Buffer Size = " + inputBuffer.Count
				);
			}

			inputBuffer[currentFrameOffset] = value;
		}
	}
	
	public override Dictionary<InputReferences, InputEvents> previousInputs{
		get{
			return inputBuffer[currentFrameOffset - 1];
		}
		protected set{
			inputBuffer[currentFrameOffset - 1] = value;
		}
	}
	#endregion

	#region protected instance properties
	protected virtual int currentFrameOffset{
		get{return (int)(UFE.currentNetworkFrame - this.currentFrame);}
	}
	#endregion

	#region protected fields
	protected List<int?> optionSelections = new List<int?>();
	#endregion

	#region public instance methods
	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Returns the current instance as an UFEcontroller. 
	/// This method is required for accessing more easily to this class from the main UFE engine using reflection.
	/// </summary>
	/// <returns>The UFE controller.</returns>
	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	public UFEController AsUFEController(){
		return (UFEController)this;
	}

    public int CalculateFrameDelay() {
        float ping = UFE.multiplayerAPI.Connections > 0 ? Mathf.Max(UFE.multiplayerAPI.GetLastPing(), 1f) : 1f;
        int frameDelay = Mathf.CeilToInt(0.001f * ping / (float)(UFE.config.fps));
        //Debug.Log("Ping: " + ping + " ---> Frame Delay: " + frameDelay);
        return frameDelay;
    }
	#endregion

	#region public override methods
	public override void Initialize (IEnumerable<InputReferences> inputs, int bufferSize){
		base.Initialize (inputs, bufferSize);

		// In network controllers we start with an empty input buffer
		this.inputBuffer.Clear();

		// In addition, we can start without any programmed character (or stage) selection
		this.optionSelections.Clear();
		
		// Set the default "previous input" for the first frame.
		this.currentFrame = -1;
		
		Dictionary<InputReferences, InputEvents> ev = new Dictionary<InputReferences, InputEvents>();
		foreach (InputReferences inputReference in this.inputReferences){
			ev[inputReference] = InputEvents.Default;
		}
		
		this.inputBuffer.Add(ev);
		this.optionSelections.Add(null);
	}
	#endregion

	#region protected override methods
	public override void DoFixedUpdate(){
		// First, check if the network connection is working and 
		// both players have entered their input for the current frame...
		if (this.inputReferences != null && UFE.GetPlayer1Controller().isReady && UFE.GetPlayer2Controller().isReady){
			// In that case, check if we have selected a menu option using the mouse instead of button-presses...
			int? oldSelection = this.optionSelections[this.currentFrameOffset - 1];
			int? newSelection = this.optionSelections[this.currentFrameOffset];
			
			if (oldSelection == null && newSelection != null){
				this.SelectOption(newSelection.Value);
			}
			
			// Finally, remove the obsolete information from the buffer
			while (this.currentFrameOffset > 1){
				++this.currentFrame;
				this.inputBuffer.RemoveAt(0);
				this.optionSelections.RemoveAt(0);
			}
		}
	}
	#endregion

	#region protected abstract methods
	protected virtual void SelectOption(int option){
		if (UFE.currentScreen != null){
			UFE.currentScreen.SelectOption(option, this.player);
		}

		if (UFE.battleGUI != null){
			UFE.battleGUI.SelectOption(option, this.player);
		}
	}
	#endregion
}
#endif
