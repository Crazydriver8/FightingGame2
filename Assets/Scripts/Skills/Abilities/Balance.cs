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
    // Changes numerical values using a delta or by setting it
    public void PassiveEffect(float deltaBalance, string player, string index = "", bool set = false)
    {
        Dictionary<string, string> properties = bb.GetProperties(player);

        float balance = float.Parse(properties[(index == "" ? id : index)]),
              newBalance = (set ? balance + deltaBalance : deltaBalance);

        // Write valid amounts only
        if (newBalance >= 0 && newBalance <= 100)
            bb.UpdateProperty(player, id, newBalance.ToString());
        else if (newBalance < 0)
            bb.UpdateProperty(player, id, "0");
        else
            bb.UpdateProperty(player, id, "100");
    }

    // Checks preconditions, given the minimum required properties
    public bool VerifyActiveEffect(Dictionary<string, string> atLeast, Dictionary<string, string> atMost, Dictionary<string, string> match, string key)
    {
        return (atLeast == null || bb.IsAtLeast(key, atLeast)) && (atMost == null || bb.IsAtMost(key, atMost)) && (match == null || bb.IsMatch(key, match));
    }
}
