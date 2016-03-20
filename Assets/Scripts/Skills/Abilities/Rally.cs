using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Rally : Strip {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    /* Battle effects
     */
    // Changes the amount of favor
    public void PassiveEffect(float deltaRally, string player)
    {
        Dictionary<string, string> properties = bb.GetProperties(player);

        float rally = float.Parse(properties[id]),
              newRally = rally + deltaRally;

        // Write valid amounts only
        if (newRally >= 0 && newRally <= 100)
            bb.UpdateProperty(player, id, newRally.ToString());
        else if (newRally < 0)
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
