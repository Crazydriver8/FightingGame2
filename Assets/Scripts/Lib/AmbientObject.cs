using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AmbientObject : SmartObject {

    // Basic information
    protected string selfKey = null;
    protected bool registered = false;
    
    // A reference to the Blackboard
    public GameObject blackBoard;
    protected BlackBoard bb;

    // A reference to scripts that this object can interact with
    public GameObject[] stripControl;
    protected Dictionary<string, Strip> strips = new Dictionary<string, Strip>();


    // Happens on initialization
    void Awake()
    {
        // Reference self
        self = gameObject;

        // Set up the SmartObject
        states.Add(BAD_STATE, LeaveBadState);
        states.Add(INIT_STATE, InitialState);

        SetInitData();

        // Get a reference to the Blackboard when this script starts up
        bb = blackBoard.GetComponent<BlackBoard>();

        // Grab all STRIPs that this object can use
        foreach (GameObject g in stripControl)
        {
            foreach (Strip s in g.GetComponents<Strip>())
            {
                strips.Add(s.id, s);
            }
        }
    }


    /* Essential methods for functionality
     */
    protected virtual bool RegisterToBlackBoard()
    {
        selfKey = bb.Register(self, null, objectName);

        // Successful if selfKey is no longer null
        return selfKey != null;
    }

    // Get the key to make BlackBoard queries
    public string GetKey()
    {
        return selfKey;
    }
}
