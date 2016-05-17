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

    public bool waiting;
    public string waitingMove;
    public bool waitingDeliberate;

    public string temp;

    #region public override methods
    public override void Initialize (IEnumerable<InputReferences> inputs, int bufferSize){
		this.timeLastDecision = float.NegativeInfinity;
		base.Initialize (inputs, bufferSize);
        //bb = GameObject.FindObjectOfType<BlackBoard>();
        bb = GameObject.Find("BlackBoard").GetComponent<BlackBoard>();
        //dta = GameObject.FindObjectOfType<DecisionTreeAI>();
        dta = GameObject.Find("BlackBoard").GetComponent<DecisionTreeAI>();
        //dta.LoadJSON("./C45algorithm-master/KOSH.json");
        dta.LoadJSON("./C45algorithm-master/shahan.json");

        c = GameObject.FindObjectOfType<Canvas>();

        nameObj = GameObject.Find("Name");
        if (nameObj != null)
        {
            nameScript = nameObj.GetComponent<NameHolder>();
        }

        waiting = false;
        waitingMove = "";
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
            if (!waitingDeliberate)
            {
                temp = dta.Deliberate(bb);
                //Debug.Log("Best move is " + temp);
                waitingDeliberate = true;
                StartCoroutine(StartDeliberate(0.05f));
            }
			this.SetBestMove(temp);
            float tempDelay = 0.0f;
            
            //Debug.Log("not waiting");
            foreach (InputReferences input in this.inputReferences)
            {
                //TEST AREA
                //Debug.Log(input.engineRelatedButton);
                this.currentFrameInputs[input] = this.DoBestMove(input, temp);
                
            }
		}
	}

    public InputEvents DoBestMove(InputReferences inputReference, string bestMove)
    {
        //Debug.Log("Trying to fire " + bestMove);
        //StartCoroutine(Wait(Random.Range(0,1)));

        ControlsScript self = UFE.GetControlsScript(this.player);
        if (self != null)
        {
            ControlsScript opponent = self.opControlsScript;

            if (opponent != null)
            {
                bool isOpponentDown = opponent.currentState == PossibleStates.Down;
                float dx = opponent.transform.position.x - self.transform.position.x;
                float axis = 0f;
                int distance = Mathf.RoundToInt(100f * Mathf.Clamp01(self.normalizedDistance));
                if (bestMove == "Foward")
                {
                    //Debug.Log("Trying to move forward");
                    axis = Mathf.Sign(dx) * 1f;
                    //StartCoroutine(HoldButton(0.5f, 1.0f));
                    return new InputEvents(axis);
                }
                if (bestMove == "Backward")
                {
                    axis = Mathf.Sign(dx) * 0f;
                    return new InputEvents(axis);
                }
                if (bestMove == "Down")
                {
                    return new InputEvents(axis);
                }
                if (bestMove == "Up")
                {
                    axis = 1f;
                    return new InputEvents(axis);
                }
                switch (inputReference.engineRelatedButton) {
                    case ButtonPress.Button1:
                        if (bestMove == "Button1")
                        {
                            //StartCoroutine(Wait(0.05F));
                            //Debug.Log("Waiting to fire Button1");
                            return new InputEvents(true);
                        }
                        break;
                    case ButtonPress.Button2:
                        if (bestMove == "Button2")
                        {
                            //StartCoroutine(Wait(0.05F));
                            //Debug.Log("Waiting to fire Button2");
                            return new InputEvents(true);
                        }
                        break;
                    case ButtonPress.Button3:
                        if (bestMove == "Button3")
                        {
                            //StartCoroutine(Wait(0.05F));
                            //Debug.Log("Waiting to fire Button3");
                            return new InputEvents(true);
                        }
                        break;
                    case ButtonPress.Button4:
                        if (bestMove == "Button4")
                        {
                            //StartCoroutine(Wait(0.05F));
                            //Debug.Log("Waiting to fire Button4");
                            return new InputEvents(true);
                        }
                        break;
                    default:
                        //Debug.Log("Waiting to fire Button ERR");
                        return new InputEvents(true);
                }
            }
        }
        return InputEvents.Default;
    }

    public InputEvents returnTrueThing(float axis)
    {
        return new InputEvents(axis);
    }

	public override InputEvents ReadInput (InputReferences inputReference){
		ControlsScript self = UFE.GetControlsScript(this.player);
		if (self != null){
			ControlsScript opponent = self.opControlsScript;
			
			if (opponent != null){
				bool isOpponentDown = opponent.currentState == PossibleStates.Down;
				float dx = opponent.transform.position.x - self.transform.position.x;
				int distance = Mathf.RoundToInt(100f * Mathf.Clamp01(self.normalizedDistance));

				float maxDistance = float.NegativeInfinity;
				AIDistanceBehaviour behaviour = null;

				// Try to find the correct "Distance Behaviour"
				// If there are several overlapping "Distance Behaviour", we choose the first in the list.
				foreach(AIDistanceBehaviour thisBehaviour in UFE.config.aiOptions.distanceBehaviour){
					if (thisBehaviour != null){
						if (distance >= thisBehaviour.proximityRangeBegins && distance <= thisBehaviour.proximityRangeEnds){
							behaviour = thisBehaviour;
							break;
						}

						if (thisBehaviour.proximityRangeEnds > maxDistance){
							maxDistance = thisBehaviour.proximityRangeEnds;
						}
					}
				}

				// If we don't find the correct "Distance Behaviour", make our best effort...
				if (behaviour == null){
					foreach(AIDistanceBehaviour thisBehaviour in UFE.config.aiOptions.distanceBehaviour){
						if (thisBehaviour != null && thisBehaviour.proximityRangeEnds == maxDistance){
							behaviour = thisBehaviour;
						}
					}
				}

				if (behaviour == null){
					return InputEvents.Default;
				}else if (inputReference.inputType == InputType.HorizontalAxis) {
					float axis = 0f;
					if (UFE.config.aiOptions.moveWhenEnemyIsDown || !isOpponentDown){
						axis =
							Mathf.Sign(dx)
							*
							(
								(Random.Range (0f, 1f) < behaviour.movingForwardProbability ? 1f : 0f) -
								(Random.Range (0f, 1f) < behaviour.movingBackProbability ? 1f : 0f)
							);
					}
					
					return new InputEvents (axis);
				} else if (inputReference.inputType == InputType.VerticalAxis) {
					float axis = 0f;
					if (UFE.config.aiOptions.moveWhenEnemyIsDown || !isOpponentDown){
						axis = 
							(Random.Range (0f, 1f) < behaviour.jumpingProbability ? 1f : 0f) -
							(Random.Range (0f, 1f) < behaviour.movingBackProbability ? 1f : 0f);
					}
					
					return new InputEvents (axis);
				}else{
					if (!UFE.config.aiOptions.attackWhenEnemyIsDown && isOpponentDown){
						return InputEvents.Default;
					} else if (inputReference.engineRelatedButton == ButtonPress.Button1) {
						return new InputEvents (Random.Range (0f, 1f) < behaviour.attackProbability);
					} else if (inputReference.engineRelatedButton == ButtonPress.Button2) {
						return new InputEvents (Random.Range (0f, 1f) < behaviour.attackProbability);
					} else if (inputReference.engineRelatedButton == ButtonPress.Button3) {
						return new InputEvents (Random.Range (0f, 1f) < behaviour.attackProbability);
					} else if (inputReference.engineRelatedButton == ButtonPress.Button4) {
						return new InputEvents (Random.Range (0f, 1f) < behaviour.attackProbability);
					} else if (inputReference.engineRelatedButton == ButtonPress.Button5) {
						return new InputEvents (Random.Range (0f, 1f) < behaviour.attackProbability);
					} else if (inputReference.engineRelatedButton == ButtonPress.Button6) {
						return new InputEvents (Random.Range (0f, 1f) < behaviour.attackProbability);
					} else if (inputReference.engineRelatedButton == ButtonPress.Button7) {
						return new InputEvents (Random.Range (0f, 1f) < behaviour.attackProbability);
					} else if (inputReference.engineRelatedButton == ButtonPress.Button8) {
						return new InputEvents (Random.Range (0f, 1f) < behaviour.attackProbability);
					} else if (inputReference.engineRelatedButton == ButtonPress.Button9) {
						return new InputEvents (Random.Range (0f, 1f) < behaviour.attackProbability);
					} else if (inputReference.engineRelatedButton == ButtonPress.Button10) {
						return new InputEvents (Random.Range (0f, 1f) < behaviour.attackProbability);
					} else if (inputReference.engineRelatedButton == ButtonPress.Button11) {
						return new InputEvents (Random.Range (0f, 1f) < behaviour.attackProbability);
					} else if (inputReference.engineRelatedButton == ButtonPress.Button12) {
						return new InputEvents (Random.Range (0f, 1f) < behaviour.attackProbability);
					}else{
						return InputEvents.Default;
					}
				}
			}
		}
		return InputEvents.Default;
	}

    private void SetBestMove(string move)
    {
        switch (move)
        {
            case "Foward":
            case "Backward":
            case "Up":
            case "Down":
                this.bestDirection = move;
                break;
            case "Button1":
            case "Button2":
            case "Button3":
            case "Button4":
                this.bestMove = move;
                break;
            default:
                break;
        }
    }

    public string GetBestMove()
    {
        return this.bestMove;
    }

    IEnumerator Wait(float f)
    {
        //Debug.Log("f is " + f);
        yield return new WaitForSeconds(1f);
        //Debug.Log(f + " " + Time.time);
    }

    IEnumerator HoldButton(float totalTime, float timeRate)
    {
        for (float i = 0f; i < totalTime; i += timeRate)
        {
            yield return new WaitForSeconds(timeRate);

        }
    }

    IEnumerator StartDeliberate(float f)
    {
        yield return new WaitForSeconds(f);
        waitingDeliberate = false;
    }

    #endregion
}