using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/* Favor represents whether or not a player is a favorite to win (value between 0 and 100)
 * 
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
    }
    
    public void ActiveEffect(SkillTreeNode ability)
    {
        // Check preconditions

        // Change BlackBoard
    }
}
