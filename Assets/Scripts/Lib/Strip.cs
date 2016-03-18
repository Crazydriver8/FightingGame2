using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/* Rules for SmartObjects
 * 
 * Files that inherit from this exist somewhere in the scene and can be accessed by the SmartObjects
 * Actions change depending on the SmartObject calling this script
 */
public class Strip : MonoBehaviour
{
    // Basic information
    public string id = "GenericStrip";

    // A reference to the Blackboard
    public GameObject blackBoard;
    protected BlackBoard bb;


    // Happens on initialization
    protected void Init()
    {
        // Get a reference to the Blackboard when this script starts up
        bb = blackBoard.GetComponent<BlackBoard>();
    }


    /* Essential methods for functionality
     */
    // Adds some extra properties to the BlackBoard to assist with interaction
    protected bool RegisterToBlackBoard(Dictionary<string, string> values)
    {
        // Returns true if the id was accepted and put into the dictionary
        return bb.Register(id, values) == id;
    }
    
    // Performs some action if the preconditions are satisfied
    // May change conditions
    public virtual void Action(GameObject caller, string key = null)
    {
        // Check preconditions and state

        // Add/Delete conditions from the BlackBoard

        // Perform some action, maybe change state

        // Deferred-Add/Delete conditions from the BlackBoard
    }
}