using UnityEngine;
using System.Collections;
using UnityEditor;
using SimpleJSON;
using System.IO;

public class MainMenuScreen : UFEScreen {
	public virtual void Quit(){
		UFE.Quit();
	}

	public virtual void GoToBluetoothPlayScreen(){
		UFE.StartBluetoothGameScreen();
	}

	public virtual void GoToStoryModeScreen(){
		UFE.StartStoryMode();
	}

	public virtual void GoToVersusModeScreen(){
        Debug.Log("Versus Button");
        InputScript input = GameObject.Find("InputField").GetComponent<InputScript>();
        if(input.getName() != "" && input.getName() != "Name Plz") {
            Debug.Log("Name found: " + input.getName());
            UFE.StartVersusModeScreen();
        } else
        {
            Debug.Log("No name found");
            InputScript temp = GameObject.Find("InputField").GetComponent<InputScript>();
            temp.flickerName();
        }
	}

    public virtual void TestJSONFile()
    {
        DecisionTreeAI test = new DecisionTreeAI();
        string path = EditorUtility.OpenFilePanel("Get JSON", "", "json");
        Debug.Log("Path is : " + path);
        //path = path.Replace(".json", "");
        //Debug.Log(path);
        if (path.Length != 0)
        {
            //WWW w = new WWW("file:///" + path);
            TextAsset targetFile = Resources.Load<TextAsset>(path);
            string jsonInput;
            using (StreamReader r = new StreamReader(path)) {
                jsonInput = r.ReadToEnd();        
            }
            if (jsonInput == "" || jsonInput == null)
            {
                Debug.Log("No json found");
            }
            else
            {
                Debug.Log("JSON found : " + jsonInput);
                test.PopulateMoves(jsonInput);
                test.BestMove();
            }
            //Debug.Log(targetFile.text);
            
        }
        
    }

	public virtual void GoToTrainingModeScreen(){
		UFE.StartTrainingMode();
	}

	public virtual void GoToNetworkPlayScreen(){
		UFE.StartNetworkGameScreen();
	}

	public virtual void GoToOptionsScreen(){
		UFE.StartOptionsScreen();
	}

	public virtual void GoToCreditsScreen(){
		UFE.StartCreditsScreen();
	}

    public virtual void GoToSkillTreeScreen()
    {
        //Debug.Log("Skill Tree Screen Clicked");
        Debug.Log("Skill Tree Button");
        InputScript input = GameObject.Find("InputField").GetComponent<InputScript>();
        if (input.getName() != "" && input.getName() != "Name Plz")
        {
            Debug.Log("Name found: " + input.getName());
            UFE.StartVersusModeScreen();
        }
        else
        {
            Debug.Log("No name found");
            InputScript temp = GameObject.Find("InputField").GetComponent<InputScript>();
            temp.flickerName();
        }
        UFE.StartSkillTreeScreen();
    }
}
