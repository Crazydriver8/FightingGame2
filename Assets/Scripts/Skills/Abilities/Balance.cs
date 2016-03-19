using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/* Balance will either encourage varied moves or repeated moves
 * 
 * Starts out at 50
 *  Lower values (>= 0) -> Defensive
 *  Higher values (<= 100) -> Offensive
 */
public class Balance : Strip {
    /* Fields */

    
    // Use this for initialization
    void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
	    
	}


    /* Battle effects
     */
    // Changes the balance value
    public void PassiveEffect(float deltaBalance, string player)
    {
        Dictionary<string, string> properties = bb.GetProperties(player);

        float balance = float.Parse(properties[id]),
              newBalance = balance + deltaBalance;

        // Write valid amounts only
        if (balance >= 0 && balance <= 100)
            bb.UpdateProperty(player, id, balance.ToString());
    }

    public void ActiveEffect(SkillTreeNode ability)
    {
        // Check preconditions
        
        // Change BlackBoard
    }
}
