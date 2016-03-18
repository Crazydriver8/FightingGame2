using UnityEngine;
using System.Collections;


/* Pushes updates about this object to the BlackBoard
 *
 * In addition, exports collected data at the end of each round
 */
public class BlackBoardController : MonoBehaviour {

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


        // Ambiently change the BlackBoard by resolving the skill tree
        switch(move.moveName)
        {
            case "Evade":
                break;
            
            default:
                break;
        }
    }

    void OnHit(HitBox strokeHitBox, MoveInfo move, CharacterInfo hitter)
    {
        // Calculate passive effects

        // Record the amount of damage done
    }
}
