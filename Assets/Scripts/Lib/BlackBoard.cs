using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/* Emulates a tuple space using a dictionary
 */
public class BlackBoard : MonoBehaviour
{
	// Main dictionary that stores tuples as a name indexed to a List<string> of properties
	private Dictionary<string, Dictionary<string, string>> flags = new Dictionary<string, Dictionary<string, string>>();
	
	// A list of SmartObjects that can interact with each other
	private Dictionary<string, GameObject> objects = new Dictionary<string, GameObject>();
	
	
	/* Initialization
	 */
	// Adds a SmartObject or a key to a dictionary of objects, and adds all properties that the BlackBoard can manage to the dictionary of flags
	public string Register(GameObject smartObj, Dictionary<string, string> properties, string key = null)
	{
        // Can't do anything if no object or key was given
        if (smartObj == null && key == null)
            return null;
        else
        {
            // Generate a new key if none is provided or if the key provided is already in the dictionary
            if (key != null && flags.ContainsKey(key) == false && objects.ContainsKey(key) == false)
            {
                objects.Add(key, smartObj);
                flags.Add(key, properties);

                return key;
            }
            else
            {
                string newKey = smartObj.GetHashCode().ToString();

                // The only thing that can cause a failure here is if the hash has already been added (no object can be added more than once)
                if (flags.ContainsKey(newKey) == false && objects.ContainsKey(newKey) == false)
                {
                    objects.Add(newKey, smartObj);
                    flags.Add(newKey, properties);

                    return smartObj.GetHashCode().ToString();
                }
                else
                    return null;
            }
        }
	}
    public string Register(string key, Dictionary<string, string> properties)
    {
        // Can't do anything if no object or key was given
        if (key == null)
            return null;
        else
        {
            // Generate a new key if none is provided or if the key provided is already in the dictionary
            if (key != null && flags.ContainsKey(key) == false && objects.ContainsKey(key) == false)
            {
                // Note that no object is added if no key is provided
                flags.Add(key, properties);
                return key;
            }
        }

        // Note that nothing happens if the key already exists in the dictionary
        return null;
    }


    /* Base functionality
	 */
    // Checks that a key exists in the dictionary
    public bool Exists(string key)
	{
        Dictionary<string, string> test;
		return flags.TryGetValue(key, out test);
	}

    // Gets properties by key
    public Dictionary<string, string> GetProperties(string key)
    {
        Dictionary<string, string> properties;
        if (flags.TryGetValue(key, out properties))
        {
            return properties;
        }

        return null;
    }

    // Get value by two keys
    public string GetValue(string key, string index)
    {
        Dictionary<string, string> properties;
        if (flags.TryGetValue(key, out properties))
        {
            string value;
            if (properties.TryGetValue(key, out value))
            {
                return value;
            }
        }

        return null;
    }

    // Update a property by key, index, and value
    public bool UpdateProperty(string key, string index, string value)
    {
        Dictionary<string, string> properties;
        if (flags.TryGetValue(key, out properties))
        {
            string oldProperty;
            if(properties.TryGetValue(index, out oldProperty))
            {
                properties[index] = value;

                DumpBlackBoard(key);
                return true;
            }
        }

        return false;
    }

    // Update a set of properties
    public bool UpdateProperties(string key, Dictionary<string, string> newProperties)
    {
        Dictionary<string, string> properties;
        Dictionary<string, string> oldProperties = new Dictionary<string, string>();
        if (flags.TryGetValue(key, out properties))
        {
            foreach (KeyValuePair<string, string> newProperty in newProperties)
            {
                string oldValue;
                if (properties.TryGetValue(newProperty.Key, out oldValue))
                {
                    oldProperties.Add(newProperty.Key, oldValue);
                    properties[newProperty.Key] = newProperty.Value;
                }
                else
                {
                    // Revert; it is already known that these properties exist
                    foreach(KeyValuePair<string, string> oldProperty in oldProperties)
                    {
                        properties[oldProperty.Key] = oldProperty.Value;
                    }

                    return false;
                }
            }

            DumpBlackBoard(key);
            return true;
        }

        return false;
    }

    // Checks that a key maps to the given values
    // Specifically, goes through a dictionary of tuples and checks if either some of them (not strict) or all of them (strict) are there
    public bool IsMatch(string key, Dictionary<string, string> values, bool strict = false)
	{
        Dictionary<string, string> test;
		if (flags.TryGetValue(key, out test))
		{
            // When strict is enabled, fail to match if sizes are different
            if (test.Count != values.Count && strict == true)
                return false;

            // Check that the values match
            foreach (KeyValuePair<string, string> tuple in values)
            {
                if (test[tuple.Key] != tuple.Value)
                    return false;
            }

            return true;
		}
		
		return false;
	}

    // For a set of properties, checks that all other properties are AT LEAST the passed-in values (float only)
    public bool IsAtLeast(string key, Dictionary<string, string> values, bool strict = false)
    {
        Dictionary<string, string> test;
        if (flags.TryGetValue(key, out test))
        {
            // When strict is enabled, fail to match if sizes are different
            if (test.Count != values.Count && strict)
                return false;

            // Check that the values are at least what is given
            foreach (KeyValuePair<string, string> tuple in values)
            {
                if (!(float.Parse(test[tuple.Key]) >= float.Parse(tuple.Value)))
                    return false;
            }

            return true;
        }

        return false;
    }

    // For a set of properties, checks that all other properties are AT MOST the passed-in values (float only)
    public bool IsAtMost(string key, Dictionary<string, string> values, bool strict = false)
    {
        Dictionary<string, string> test;
        if (flags.TryGetValue(key, out test))
        {
            // When strict is enabled, fail to match if sizes are different
            if (test.Count != values.Count && strict)
                return false;

            // Check that the values are at most what is given
            foreach (KeyValuePair<string, string> tuple in values)
            {
                if (!(float.Parse(test[tuple.Key]) <= float.Parse(tuple.Value)))
                    return false;
            }

            return true;
        }

        return false;
    }

    // Add a single property at a given point in the dictionary
    public bool AddProperty(string key, KeyValuePair<string, string> property)
	{
        Dictionary<string, string> properties;
        if (flags.TryGetValue(key, out properties))
        {
            if (!properties.ContainsKey(property.Key))
            {
                properties.Add(property.Key, property.Value);
            }

            return false;
        }

        return false;
    }

    // Remove a property
    public bool RemoveProperty(string key, string index)
    {
        Dictionary<string, string> properties;
        if (flags.TryGetValue(key, out properties))
        {
            properties.Remove(index);
            return true;
        }

        return false;
    }
	
	// Remove an object from the dictionary by removing its key from the dictionary
	public bool RemoveObject(string key)
	{
		return flags.Remove(key) || objects.Remove(key);
	}

    // Clears out the BlackBoard
    public void ClearBlackBoard()
    {
        flags = new Dictionary<string, Dictionary<string, string>>();
        objects = new Dictionary<string, GameObject>();
    }


    // Output the blackboard as a string
    public IEnumerator BlackBoardLog(string player)
    {
        bool ai = false;
        //Debug.Log("Logging for " + player);
        
        // Check if player is AI
        if (player == Constants.p1Key)
        {
            UFEController p1Control = UFE.GetPlayer1Controller();
            if (p1Control.isCPU)
            {
                //Debug.Log(player + " is cpu");
                ai = true;
            }
        }
        if (player == Constants.p2Key)
        {
            UFEController p2Control = UFE.GetPlayer2Controller();
            if (p2Control.isCPU)
            {
                //Debug.Log(player + " is cpu");
                ai = true;
            }
        }
        //player = (UFE.iscpu ? player + "_AI" : player);

        //Debug.Log(player + " is an ai: " + ai);

        // Record data for this player
        // Append _AI to all AI players
        KeyData data;
        if (ai)
        {
            data = new KeyData(UFE.GetTimer(), "BlackBoard Update", (flags[player][Constants.playerName] == "" ? player + "_AI" : flags[player][Constants.playerName] + "_AI"), BlackBoardToString());
        }
        else {
            data = new KeyData(UFE.GetTimer(), "BlackBoard Update", (flags[player][Constants.playerName] == "" ? player : flags[player][Constants.playerName]), BlackBoardToString());
        }

        string write_to = Constants.addLogUrl + data.AsUrlParams() + "&hash=" + data.Md5Sum(Constants.notSoSecretKey);
        //Debug.Log("Write to: " + write_to);
        // Enqueue for POSTing to server

        if (player == Constants.p1Key)
            PostDataToServer.postQueueP1.Add(new WWW(write_to));
        else if (player == Constants.p2Key)
            PostDataToServer.postQueueP2.Add(new WWW(write_to));
        else
            Debug.Log("Error: 3 Players");
        yield return null;
    }
    public string DumpBlackBoard(string player = null)
    {
        //Debug.Log(player);
        if (player == null)
        {
            if (UFE.GetLocalPlayer() == 1)
                StartCoroutine(BlackBoardLog(Constants.p1Key));
            else
                StartCoroutine(BlackBoardLog(Constants.p2Key));
        }
        else
        {
            //Debug.Log(player + " is using the correct thing");
            StartCoroutine(BlackBoardLog(player));
        }

        // Also output the string that was dumped
        return BlackBoardToString();
    }
    public string BlackBoardToString()
    {
        string bb = "{\n";
		
        foreach(KeyValuePair<string, Dictionary<string, string>> f in flags)
        {
            bb += "\t\"" + f.Key + "\" : {\n";
            foreach (KeyValuePair<string, string> tuple in f.Value)
            {
                bb += "\t\t\"" + tuple.Key + "\" : \"" + tuple.Value + "\",\n";
            }
			
			bb = bb.Substring(0, bb.Length - 2); // Remove trailing comma
            bb += "\n\t},\n";
        }
		
		bb = bb.Substring(0, bb.Length - 2); // Remove trailing comma
        bb += "\n}";
		
        return bb;
    }
}


public struct KeyData
{
    string time;
    string keyPress;
    string playerName;
    string blackBoard;

    public KeyData(float time, string keyPress, string playerName, string blackBoard)
    {
        this.time = time.ToString();
        this.keyPress = keyPress;
        this.playerName = playerName;
        this.blackBoard = (blackBoard == null ? "" : blackBoard);
    }

    // Get MD5 hash of data
    public string Md5Sum(string key)
    {
        string strToEncrypt = time.ToString() + keyPress + playerName + key;

        System.Text.UTF8Encoding ue = new System.Text.UTF8Encoding();
        byte[] bytes = ue.GetBytes(strToEncrypt);

        // Encrypt bytes
        System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
        byte[] hashBytes = md5.ComputeHash(bytes);

        // Convert the encrypted bytes back to a string (base 16)
        string hashString = "";

        for (int i = 0; i < hashBytes.Length; i++)
        {
            hashString += System.Convert.ToString(hashBytes[i], 16).PadLeft(2, '0');
        }

        return hashString.PadLeft(32, '0');
    }

    // Output data in URL parameter form
    public string AsUrlParams()
    {
        if (playerName != "Player2") {
            return "time=" + time + "&keyPress=" + keyPress + "&playerName=" + playerName + "&bbState=" + WWW.EscapeURL(blackBoard);
        } else
        {
            return "time=" + time + "&keyPress=" + keyPress + "&playerName=" + playerName + "_AI" + "&bbState=" + WWW.EscapeURL(blackBoard);
        }
    }
}