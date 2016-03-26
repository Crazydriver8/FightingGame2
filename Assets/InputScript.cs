using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InputScript : MonoBehaviour {
    public string placeholder = "";

    private string initText = null;
    private InputField userEntry = null;

	// Use this for initialization
	void Start () {
    }
	
	// Update is called once per frame
	void Update () {
        if (userEntry == null)
        {
            userEntry = this.GetComponent<InputField>();
        }

        if (userEntry != null)
        {
            if (initText == null)
            {
                initText = userEntry.text;
            }
            else
            {
                if ((userEntry.text == null || userEntry.text == "" || userEntry.text == placeholder))
                {
                    string currName = GameObject.Find("Name").GetComponent<NameHolder>().username;
                    userEntry.text = (currName == null || currName == "" ? initText : currName);
                }
                else if (userEntry.text != initText)
                {
                    GameObject.Find("Name").GetComponent<NameHolder>().SetName(userEntry.text);
                }
            }
        }
        else
        {
            Debug.Log("no field");
        }
    }

    public string getName()
    {
        if (userEntry != null)
        {
            return userEntry.text;
        }
        return "";
    }
}
