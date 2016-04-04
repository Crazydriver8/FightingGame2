using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UserNameGet : MonoBehaviour {
    private Text userEntry = null;
    private string initText = "No user entered";
    // Use this for initialization
    void Start () {
        if (userEntry == null)
        {
            userEntry = this.GetComponent<Text>();
        }
        string currName = GameObject.Find("Name").GetComponent<NameHolder>().username;
        //Debug.Log("Setting skills for " + currName);
        userEntry.text = (currName == null || currName == "" ? initText : currName);
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
