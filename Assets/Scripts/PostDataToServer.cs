﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/* Synchronizes writing to server
 */
public static class PostDataToServer {
    public static List<WWW> postQueueP1 = new List<WWW>();
    public static List<WWW> postQueueP2 = new List<WWW>();

    public static IEnumerator PostData(bool p1 = true)
    {
        while (true)
        {
            if (p1)
            {
                // Attempt to POST the first thing in the queue
                if (postQueueP1.Count > 0)
                {
                    yield return postQueueP1[0];

                    // Check for errors
                    if (postQueueP1[0].error != null)
                        Debug.Log("There was a logging error: " + postQueueP1[0].error);
                    
                    //Debug.Log(postQueueP1[0].text);

                    // Remove the first element
                    postQueueP1.RemoveAt(0);
                }
                else // Nothing to write; standby
                    yield return null;
            }
            else
            {
                // Attempt to POST the first thing in the queue
                if (postQueueP2.Count > 0)
                {
                    yield return postQueueP2[0];

                    // Check for errors
                    if (postQueueP2[0].error != null)
                        Debug.Log("There was a logging error: " + postQueueP2[0].error);

                    //Debug.Log(postQueueP2[0].text);

                    // Remove the first element
                    postQueueP2.RemoveAt(0);
                }
                else // Nothing to write; standby
                    yield return null;
            }
        }
    }


    /* Skill trees */
    public static IEnumerator PostSkillTree(string playerName, string json)
    {
        Debug.Log("Attempting to post skill tree to server");
        WWW write_to = new WWW(Constants.postTreeUrl + "playerName=" + playerName + "&json=" + WWW.EscapeURL(json));
        yield return write_to;

        if (write_to.error != null)
            Debug.Log("There was a logging error: " + write_to.error);

        Debug.Log(write_to.text);
    }
}
