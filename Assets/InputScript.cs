using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class InputScript : MonoBehaviour {
    public string placeholder = "";

    private string initText = null;
    private InputField userEntry = null;
    private Image versusButton = null;

	// Use this for initialization
	void Start () {
        versusButton = GameObject.Find("Button_VersusMode").GetComponent<Image>();
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
                versusButton.color = Color.gray;
            }
            else
            {
                if ((userEntry.text == null || userEntry.text == "" || userEntry.text == placeholder))
                {
                    string currName = GameObject.Find("Name").GetComponent<NameHolder>().username;
                    userEntry.text = (currName == null || currName == "" ? initText : currName);
                    versusButton.color = Color.gray;
                }
                else if (userEntry.text != initText)
                {
                    GameObject.Find("Name").GetComponent<NameHolder>().SetName(userEntry.text);
                    //Debug.Log("Different text entered");
                    versusButton.color = Color.white;
                    //Debug.Log(versusButton.color.ToString());
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

    public void flickerName()
    {
        float shakeAmount = 30;
        float shareOffset = 0;
        bool bShake = true;
        Debug.Log("Attempting to shake");
        Vector3 initPos = this.transform.position;
        this.transform.position = new Vector3(-26, Mathf.Lerp(0,10,0.5f), 0);
        this.transform.position = initPos;
        
    }
}
