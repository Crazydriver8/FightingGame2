using UnityEngine;
using System.Collections;
using System;

public class KeyLogger : MonoBehaviour
{
    // A list of keys to listen for
    private enum Direction { UD, LR };
    public string[] movementKeys = new string[2] { "Vertical", "Horizontal" };
    public string[] movementNames = new string[4] { "MoveUp", "MoveDown", "MoveLeft", "MoveRight" };
    public string[] attackKeys = new string[4] { "Basic", "Strong", "Evade", "Grab"};

    // Database information
    private string secretKey = "TheCakeIsALie"; // Because Unity3D is bad and doesn't support environment variables
    private string addLogUrl = "fighting-game-675.herokuapp.com/datalog/keylogger.php?";

    // The keypress data that will be stored in the database
    private struct KeyData
    {
        string matchId;
        string time;
        string keyPress;
        string playerName;

        public KeyData(int matchId, float time, string keyPress, string playerName)
        {
            this.matchId = matchId.ToString("X");
            this.time = time.ToString();
            this.keyPress = keyPress;
            this.playerName = playerName;
        }

        // Get MD5 hash of data
        public string Md5Sum(string key)
        {
            string strToEncrypt = matchId + time.ToString() + keyPress + playerName + key;

            System.Text.UTF8Encoding ue = new System.Text.UTF8Encoding();
            byte[] bytes = ue.GetBytes(strToEncrypt);

            // encrypt bytes
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
            return "matchId=" + matchId + "&time=" + time + "&keyPress=" + keyPress + "&playerName=" + playerName;
        }
    }


	// Use this for initialization
	void Start () {
        // Start logging
        StartCoroutine(GetMovements());
        StartCoroutine(GetAttacks());
	}

    // Update is called once per frame
    void Update () {
	    
	}


    /* Co-routines
     */
    // Detect movement
    private IEnumerator GetMovements()
    {
        while (true)
        {
            for (int i = 0; i < movementKeys.Length; i++)
            {
                float move = Input.GetAxis(movementKeys[i]);

                switch (i)
                {
                    case (int)Direction.UD:
                        if (move != 0)
                            print((move > 0 ? movementNames[0] : movementNames[1]));
                        break;

                    case (int)Direction.LR:
                        if (move != 0)
                            print((move > 0 ? movementNames[2] : movementNames[3]));
                        break;

                    default: break;
                }
            }

            yield return null;
        }
    }

    // Detect attacks
    private IEnumerator GetAttacks()
    {
        while(true)
        {
            for (int i = 0; i < attackKeys.Length; i++)
            {
                if (Input.GetAxis(attackKeys[i]) > 0)
                {
                    print(attackKeys[i]);
                }
            }

            yield return null;
        }
    }

    // Upload key logging data to the server
    private IEnumerator PostKeyLog(int matchId, float time, string keyPress, string playerName)
    {
        // Gather the KeyData and convert to a string
        KeyData data = new KeyData(matchId, time, keyPress, playerName);
        string hash = data.Md5Sum(secretKey);

        string post_url = addLogUrl + data.AsUrlParams() + "&hash=" + hash;

        // Post the URL to the site and create a download object to get the result.
        WWW hs_post = new WWW(post_url);
        yield return hs_post; // Wait until the download is done

        if (hs_post.error != null)
            print("There was an error posting the key press: " + hs_post.error);
    }
}
