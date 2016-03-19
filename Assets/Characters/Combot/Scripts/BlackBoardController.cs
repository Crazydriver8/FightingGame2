using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


/* Pushes updates about this object to the BlackBoard
 *
 * In addition, exports collected data at the end of each round
 */
public class BlackBoardController : MonoBehaviour {

    /* Fields */

    // SkillTree handlers
    Dictionary<string, Action<bool>> handlers = new Dictionary<string, Action<bool>>();
    Evade evade;
    

    // Happens when the object is enabled
    void OnEnable()
    {
        // Subscribe to UFE events
        UFE.OnMove += OnMove;
        UFE.OnHit += OnHit;
    }

    // Happens when the object is disabled
    void OnDisable()
    {
        // Subscribe to UFE events
        UFE.OnMove -= OnMove;
        UFE.OnHit -= OnHit;
    }


    // Use this for initialization
    void Start () {
        //Debug.Log("BlackBoard stuff");

        // Get all of the moves' SkillTree handlers
        Evade evade = GetComponent<Evade>();
        handlers["Evade"] = evade.Resolve;
    }

    // Update is called once per frame
    void Update () {
	    
	}


    /* UFE events
     */
    void OnMove(MoveInfo move, CharacterInfo player)
    {
        // Record the button that was pressed
        Debug.Log("Player " + player.GetInstanceID() + " inputted " + (int)move.buttonExecution[0]);

        // Record the current time
        Debug.Log(Time.time);

        // Save the state of the BlackBoard
        Debug.Log("Saved BlackBoard state");
    }

    void OnHit(HitBox strokeHitBox, MoveInfo move, CharacterInfo hitter)
    {
        // Calculate passive effects
        Action<bool> resolver;
        if (handlers.TryGetValue(move.moveName, out resolver))
            resolver(true);

        // Record the amount of damage done
        Debug.Log("Hit damage: " + move.hits[0].damageOnHit);
    }
}