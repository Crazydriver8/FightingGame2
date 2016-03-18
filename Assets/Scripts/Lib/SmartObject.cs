using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public abstract class SmartObject : MonoBehaviour
{
    // Information about this object
    public string objectName = "SmartObject";

    // Variables that control how the object interacts with its surroundings
    public float interactDist = 10.0f; // Interact with the target if it comes within this distance
    public float transitionWait = 0.0f; // Wait time between state transitions

    // Objects that this SmartObject can interact with
    protected static int PLAYER = 0;
    public GameObject[] targets;

    // States and transitions
    protected static int NOT_A_STATE = -2; // Constant value that means "not an actual state in the machine"
    protected static int BAD_STATE = -1; // A transition that wasn't supposed to happen leads to a bad state
    protected static int INIT_STATE = 0; // All objects, by default, start out on this state
    public int startState;
    protected int currState;
    protected int nextState = NOT_A_STATE;
    protected Dictionary<int, System.Func<string[], IEnumerator>> states = new Dictionary<int, System.Func<string[], IEnumerator>>();
    protected Dictionary<int, List<int>> transitions = new Dictionary<int, List<int>>
    {
        { BAD_STATE, new List<int> (new int[] { INIT_STATE }) }
    };
    protected Dictionary<int, string> animations;

    // Information about the SmartObject itself
    protected GameObject self;
    protected InitData id;

    // Easing function
    [Range(0, 2)]
    public float easeFactor = 1;


    // Always runs on startup
    void Awake()
    {
        states.Add(BAD_STATE, LeaveBadState);
        states.Add(INIT_STATE, InitialState);

        SetInitData();
    }


    /* Struct that defines basic information about the object's initial state
     */
    protected struct InitData
    {
        Vector3 startPos;
        Quaternion startOrientation;
        int startState;

        // Constructor
        public InitData(Vector3 position, Quaternion rotation, int startState) : this()
        {
            this.startPos = position;
            this.startOrientation = rotation;
            this.startState = startState;
        }

        // Get fields
        public Vector3 GetStartPos()
        {
            return this.startPos;
        }
        public Quaternion GetStartOrientation()
        {
            return this.startOrientation;
        }
        public int GetStartState()
        {
            return this.startState;
        }
    }

    
    /* Default states
     * 
     * These states are included by default
     */
    // A generic state that doesn't really do anything...
    // Override this with an actual initial state, or (even better) add your own
    protected virtual IEnumerator InitialState(string[] args = null)
    {
        while (true)
        {
            print("Ready to do something...");
            yield return null;
        }
    }

    // Gets out of bad states (default to going to INIT_STATE)
    protected virtual IEnumerator LeaveBadState(string[] args = null)
    {
        // Kill everything
        StopAllCoroutines();
        ResetObject();

        // Reset to the initial state
        currState = nextState;
        nextState = NOT_A_STATE;
        System.Func<string[], IEnumerator> next;
        if (states.TryGetValue(currState, out next))
            yield return StartCoroutine(next(null));
        else
            yield return StartCoroutine(InitialState(null));
    }


    /* Transition co-routine (default to going to INIT_STATE)
     * At the very least, override this to change which error-handling co-routine gets called
     */
    // Single action
    protected virtual IEnumerator ChangeState()
    {
        if (!SafeStartCoroutine(nextState, null))
        {
            // Successfully moved to the next state
            nextState = NOT_A_STATE;
            yield return null;
        }
        else
        {
            // Whoops! Try to recover...
            nextState = INIT_STATE;
            yield return StartCoroutine(LeaveBadState(null));
        }
    }

    // Method for externally causing a transition
    public virtual bool InduceTransition(int next)
    {
        // Does nothing until overridden, but use with caution!
        return false;
    }


    /* Common operations
     */
    // On startup, call this to get basic information on the object's initial state
    protected virtual void SetInitData()
    {
        id = new InitData(transform.position, transform.rotation, startState);
    }

    // Find out if a transition is possible
    protected virtual bool ExistsTransition(int nextState)
    {
        // Grab all possible values
        List<int> possible = null;
        if (transitions.TryGetValue(currState, out possible))
        {
            // Try to find nextState in list
            foreach (int state in possible)
            {
                // Exit on find
                if (state == nextState)
                    return true;
            }
        }

        // Not in dictionary
        return false;
    }
    
    // Name of this behavior
    public string GetName()
    {
        return objectName;
    }

    // Resets to defaults, if any are specified
    // Happens when leaving bad state
    public virtual void ResetObject()
    {
        transform.position = id.GetStartPos();
        transform.rotation = id.GetStartOrientation();
        currState = id.GetStartState();
    }

    // Add new states to the machine
    // Must be overridden to have any effect
    protected virtual void AddNewStates()
    {
        // By default, remove the old default start state
        states.Remove(INIT_STATE);
    }

    // Add new transitions to the machine
    // Must be overridden to have any effect
    protected virtual void AddNewTransitions()
    {

    }
    // Alternatively, add key/value pairs
    protected virtual void AddNewTransitions(List<KeyValuePair<int, int>> transitionPairs)
    {
        List<int> nextStates = null;
        int currKey = NOT_A_STATE;

        foreach (KeyValuePair<int, int> transition in transitionPairs)
        {
            // If the key changes and a list was created, add to a new list
            if (transition.Key != currKey)
            {
                if (nextStates != null)
                {
                    // Flush the changes if the list did not already exist in the dictionary
                    if (!transitions.ContainsKey(currKey))
                        transitions.Add(currKey, nextStates);

                    // Check if there's a new list to add to; otherwise, make one
                    currKey = transition.Key;
                    if (transitions.TryGetValue(currKey, out nextStates))
                        nextStates = new List<int>();
                }
                else
                    nextStates = new List<int>();
            }

            // Add the transition on this iteration
            nextStates.Add(transition.Value);
        }
    }

    // Automatically starts a coroutine by looking it up in the dictionary and giving the result some arguments
    protected bool SafeStartCoroutine(int key, string[] args = null)
    {
        System.Func<string[], IEnumerator> next;
        if (states.TryGetValue(key, out next))
        {
            StartCoroutine(next(args));
            return true;
        }

        return false;
    }

    // Get the current state
    public int GetState()
    {
        return currState;
    }
    
    // Movement Easing equation: y = x^a / (x^a + (1-x)^a)
    //
    // Takes x values between 0 and 1 and maps them to y values also between 0 and 1
    //  a = 1 -> straight line
    //  This is a logistic function; as a increases, y increases faster for values of x near .5 and slower for values of x near 0 or 1
    //
    // For animation, 1 < a < 3 is pretty good
    protected float Ease(float x)
    {
        float a = easeFactor + 1.0f;
        return Mathf.Pow(x, a) / (Mathf.Pow(x, a) + Mathf.Pow(1 - x, a));
    }

    // Compare SmartObjects
    public override bool Equals(object obj)
    {
        // Check references
        if (ReferenceEquals(this, obj))
            return true;

        // Check null
        if ((object)this == null ^ obj == null)
            return false;
        if ((object)this == null && obj == null)
            return true;

        // Do the names match?
        if (this.GetName() == ((SmartObject)obj).GetName())
            return true;

        return base.Equals(obj);
    }

    public static bool operator ==(SmartObject a, SmartObject b)
    {
        return a.Equals(b);
    }

    public static bool operator !=(SmartObject a, SmartObject b)
    {
        return !a.Equals(b);
    }

    // Hash code
    public override int GetHashCode()
    {
        return objectName.GetHashCode() ^ self.GetHashCode();
    }
}
