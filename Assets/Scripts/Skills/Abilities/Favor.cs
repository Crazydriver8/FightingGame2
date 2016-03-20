using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/* Favor represents whether or not a player is a favorite to win (value between 0 and 100)
 * This value increases as the player does well
 */
public class Favor : Strip {
    /* Fields */

    
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


    /* Battle effects
     */
    // Changes the amount of favor
    public void PassiveEffect(float deltaFavor, string player)
    {
        Dictionary<string, string> properties = bb.GetProperties(player);

        float favor = float.Parse(properties[id]),
              newFavor = favor + deltaFavor;

        // Write valid amounts only
        if (newFavor >= 0 && newFavor <= 100)
            bb.UpdateProperty(player, id, newFavor.ToString());
        else if (newFavor < 0)
            bb.UpdateProperty(player, id, "0");
        else
            bb.UpdateProperty(player, id, "100");
    }
    
    // Checks preconditions
    public void VerifyActiveEffect()
    {

    }
}
