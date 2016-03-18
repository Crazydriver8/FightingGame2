using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/* A skill node that handles situations where the enemy is behind the player
 *
 * Actions available:
 *  Assist:
 *      Basic: Elbow back
 *      Strong: Mule kick
 *      Evade: Dodge behind enemy
 *      Grab: Automatically turn around
 *
 * Passives available:
 *  Favor: Chance to gain favor on hits
 */
public class BehindMe : SkillTreeNode {
    /* Fields */

    private enum States { PASSIVE, ACTIVE };
    private enum Properties { CURR_STATE, PLAYER, OPPONENT };

    // The SkillTree that this node is in
    private SkillTree move;

    // Cooldowns on the ambient effects


    /* States
     */
    // Passively assist and/or generate favor
    private IEnumerator Passive(string[] args = null)
    {
        while(true)
        {
            if (currState == (int)States.PASSIVE)
            {

            }
            else
            {

            }
        }
    }

    // Actively change how the move works
    private IEnumerator Active(string[] args = null)
    {
        while (true)
        {
            if (currState == (int)States.ACTIVE)
            {

            }
            else
            {

            }
        }
    }


    /* The action changes state
     */
    
    
    
    /* Utilities
     */
    // Override to add actual information to the BlackBoard
    protected override bool RegisterToBlackBoard()
    {
        // Get basic properties
        Dictionary<string, string> properties = new Dictionary<string, string>
        {
            { "State", currState.ToString() },
            { "NextState", nextState.ToString() }
        };

        // Attempt to register the object
        selfKey = bb.Register(self, properties, objectName);
        return selfKey != null;
    }

    // Override to add new states to the FSM
    protected override void AddNewStates()
    {
        // Remove the old default start state
        states.Remove(INIT_STATE);

        // Replace the old default LeaveBadState()
        states.Remove(BAD_STATE);
        states.Add(BAD_STATE, LeaveBadState);

        // Add the new states
        states.Add((int)States.PASSIVE, Passive);
        states.Add((int)States.ACTIVE, Active);
    }

    // Override to add new transitions to the FSM
    protected override void AddNewTransitions()
    {
        /* All possible transitions:
         *  The interaction mode can be PASSIVE or ACTIVE
         *      (PASSIVE, ACTIVE)
         *      (ACTIVE, PASSIVE)
         */

        transitions.Add((int)States.PASSIVE, new List<int>(new int[] {
            (int)States.ACTIVE
        }));

        transitions.Add((int)States.ACTIVE, new List<int>(new int[] {
            (int)States.PASSIVE
        }));
    }

    // Override the reset function to also turn the light back on
    protected override void SetInitData()
    {
        base.SetInitData();
    }
}
