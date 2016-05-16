using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class DecisionAITest : AbstractInputController {
	#region protected instance fields
	protected float timeLastDecision = float.NegativeInfinity;
    #endregion

    BlackBoard bb;
    DecisionTreeAI dta;
    Canvas c;
    GameObject moveText = null;
    Text best = null;
    private float timeLeft = 0;
    public string bestMove = "";
    public string bestDirection = "";

    private bool setDiagnostic;
    private bool setDiagnosticSet = false;
    private bool setRoundBegin;
    private GameObject nameObj;
    private NameHolder nameScript;

    float holdTime = 0.0f,
          waitTime = 0.0f;
    float lastMoveTime = -1.0f,
          lastAttackTime = -1.0f;

    InputEvents lastInputEvent = null;
    InputReferences lastMove = null;

    #region public override methods
    public override void Initialize (IEnumerable<InputReferences> inputs, int bufferSize){
		this.timeLastDecision = float.NegativeInfinity;
		base.Initialize (inputs, bufferSize);
        //bb = GameObject.FindObjectOfType<BlackBoard>();
        bb = GameObject.Find("BlackBoard").GetComponent<BlackBoard>();
        //dta = GameObject.FindObjectOfType<DecisionTreeAI>();
        dta = GameObject.Find("BlackBoard").GetComponent<DecisionTreeAI>();
        dta.LoadJSON("./C45algorithm-master/KOSH.json");

        c = GameObject.FindObjectOfType<Canvas>();

        nameObj = GameObject.Find("Name");
        if (nameObj != null)
        {
            nameScript = nameObj.GetComponent<NameHolder>();
        }
    }

    public override void DoUpdate() {
        //-------------------------
        // Diagnostic mode settings
        //-------------------------
        if (nameScript != null) setRoundBegin = nameScript.getRoundStarted();
        if (setRoundBegin == false) setDiagnosticSet = false;
        else
        {
            setDiagnostic = nameScript.diagnosticMode;
            setDiagnosticSet = true;
        }
        if (moveText != null && setRoundBegin && !setDiagnosticSet) {

            if (nameScript != null)
            {
                Debug.Log("Diagnostic read as " + setDiagnostic.ToString());
                moveText = GameObject.Find("Text_Move");
                if (moveText == null)
                {
                    Debug.Log("Could not find text");
                }
                else
                {
                }
                best = moveText.GetComponent<Text>();
                if (best == null)
                {
                    Debug.Log("Could not find text");
                }
                else
                {
                    best.text = "";
                }
            }
        }

        if (setDiagnostic && setDiagnosticSet) {
            // Timer to prevent too many updates
            if (timeLeft > 0)
            {
                timeLeft -= Time.deltaTime;
            }

            if (moveText == null)
            {
                moveText = GameObject.Find("Text_Move");
                if (moveText == null)
                {
                    Debug.Log("Could not find text");
                } else
                {
                    best = moveText.GetComponent<Text>();
                }
                
                if (best == null)
                {
                    Debug.Log("Could not find text");
                } else
                { 
                    best.text = "";
                }
            } else if (timeLeft < 1)
            {
                //Debug.Log(bestDirection + " " + bestMove);
                best.text = bestDirection + " " + bestMove;
                timeLeft = 1.25f;
            }
        } else
        {
            if (moveText == null)
            {
                GameObject moveHeader = GameObject.Find("Text_MoveTitle");
                if (moveHeader != null)
                {
                    Text head = moveHeader.GetComponent<Text>();
                    head.text = "";
                }
                moveText = GameObject.Find("Text_Move");
                if (moveText != null)
                {
                    Text tex = moveText.GetComponent<Text>();
                    tex.text = "";
                }
            }
        }

		if (this.inputReferences != null){
			//---------------------------------------------------------------------------------------------------------
			// Check the time that has passed since the last update.
			//---------------------------------------------------------------------------------------------------------
			float currentTime = Time.realtimeSinceStartup;

			if (this.timeLastDecision < 0f){
				this.timeLastDecision = currentTime;
			}

			//---------------------------------------------------------------------------------------------------------
			// If the time since the last update is greater than the input frequency, read the AI input.
			// Otherwise, don't press any input.
			//---------------------------------------------------------------------------------------------------------
			this.currentFrameInputs.Clear();
            
            // Attempt to get the blackboard state
            if (bb == null)
            {
                Debug.Log("No blackboard");
            }
            else
            {
                // Decide what the best move is and fire it
                string bestMove = dta.Deliberate(bb);
                //Debug.Log("Best move is " + bestMove);

                // Calculate wait times
                float newHoldTime = dta.timing.ButtonHoldTime(bestMove),
                      newWaitTime = dta.timing.ButtonWaitTime(bestMove);
                float now = Time.time;

                foreach (InputReferences input in this.inputReferences)
                {
                    Debug.Log("Attempting to match ["+ bestMove + "] with Input related button [" + input.engineRelatedButton.ToString() + "]");
                    if(input.engineRelatedButton == Constants.ToButtonPress(bestMove))
                    {
                        Debug.Log("     VALID MOVE [" + bestMove + "]");
                        // Is this a valid move?
                        if (Constants.IsHorizontal(bestMove) || Constants.IsVertical(bestMove))
                        {
                            // Is it OK to fire a move now?
                            // Movements can only fire if no other movement is firing
                            // New movements may fire after attacks if no other movement is firing
                            if (Mathf.Abs(this.lastMoveTime - now) > this.holdTime || (this.lastInputEvent == null && this.lastMoveTime == -1.0f))
                            {
                                // Update with new move information
                                this.lastMove = input;
                                this.lastInputEvent = this.ReadInput(input);
                                this.lastMoveTime = now;
                                this.holdTime = newHoldTime;
                            }

                            // If the old move is still there, use it
                            // Otherwise, use the new move
                            Debug.Log("     Fire 1:" + this.lastInputEvent.ToString());
                            this.currentFrameInputs[this.lastMove] = this.lastInputEvent;
                        }
                        /*else if (Constants.IsVertical(bestMove))
                        {
                            this.lastMove = input;
                            this.lastInputEvent = this.ReadInput(input);
                            this.lastMoveTime = now;
                            this.holdTime = newHoldTime;
                            this.currentFrameInputs[this.lastMove] = this.lastInputEvent;
                        }*/
                        else
                        {
                            // Attacks can fire if there is sufficient time between them
                            // Attacks can interrupt movements; they will not be saved as the last input event
                            if (Mathf.Abs(this.lastAttackTime - now) > this.waitTime || this.lastAttackTime == -1.0f)
                            {
                                // Use the attack if possible
                                this.lastAttackTime = now;
                                this.waitTime = newWaitTime;

                                Debug.Log("     Fire 2:" + input.engineRelatedButton.ToString());
                                this.currentFrameInputs[input] = this.ReadInput(input);
                                this.bestMove = input.inputButtonName.Substring(2);
                            }
                            else
                            {
                                // Otherwise, attempt to move
                                if (this.lastMove == null)
                                {
                                    Debug.Log("     Fire 3:" + input.engineRelatedButton.ToString());
                                    this.currentFrameInputs[input] = this.ReadInput(input);
                                }
                                else
                                {
                                    Debug.Log("     Fire 4:" + this.lastInputEvent.ToString());
                                    this.currentFrameInputs[this.lastMove] = this.lastInputEvent;
                                }
                            }
                        }

                        break;
                    } else
                    {
                        //Debug.Log("INVALID MOVE [" + bestMove + "]");
                    }

                }
            }
		}
	}

	public override InputEvents ReadInput (InputReferences inputReference){
        ControlsScript self = UFE.GetControlsScript(this.player);

        if (self != null)
        {
            ControlsScript opponent = self.opControlsScript;

            if (opponent != null)
            {
                float dx = opponent.transform.position.x - self.transform.position.x;
                float sign = Mathf.Sign(opponent.transform.position.x - self.transform.position.x);
                float axis = 0.0f;
                Debug.Log("Trying to fire " + inputReference.engineRelatedButton.ToString());
                // Decide what button to press
                switch (inputReference.engineRelatedButton)
                {
                    case ButtonPress.Foward:
                        //axis = Mathf.Sign(dx) * 1f;
                        return new InputEvents(1f * sign);
                        //return new InputEvents(axis);

                    case ButtonPress.Back:
                        //axis = Mathf.Sign(dx) * 0f;
                        //return new InputEvents(axis);
                        return new InputEvents(-1f * sign);

                    case ButtonPress.Down:
                        return new InputEvents(-1f);
                        //return new InputEvents(axis);

                    case ButtonPress.Up:
                        axis = 1f;
                        //return new InputEvents(axis);
                        return new InputEvents(1f);

                    case ButtonPress.Button1:
                    case ButtonPress.Button2:
                    case ButtonPress.Button3:
                    case ButtonPress.Button4:
                        return new InputEvents(true);

                    default:
                        return InputEvents.Default;
                }
            }
        }

        return InputEvents.Default;
    }

    #endregion
}