using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/* Adds some bonus damage based on how good you are at hitting your abilities
 * 
 * Using a favor ability removes all of your favor
 * Max favor: 100
 */
public class Favor : Strip {
    // Called when the STRIP is created
    void Awake()
    {
        base.Init();
    }

    
    /* Action hooks
     * 
     * The general action hook is meant to ambiently change the state of the blackboard only
     * Specific action hooks are added for each 
     */


    
    /* Preconditions
     */
    // Check who has more favor
    bool HasMoreFavor(string playerFavor, string opponentFavor)
    {
        int pf = -1,
            of = -1;
        return int.TryParse(playerFavor, out pf) && int.TryParse(opponentFavor, out of) && pf > of;
    }

    // Check if you've hit the favor cap
    bool FavorCap(string currFavor)
    {
        int f = -1;
        return int.TryParse(currFavor, out f) && f == Constants.MAX_FAVOR;
    }

    // Check to see if favor is on cooldown
    bool FavorOnCooldown(string until)
    {
        float u = 0.0f;
        return float.TryParse(until, out u) && u > Time.time;
    }
}