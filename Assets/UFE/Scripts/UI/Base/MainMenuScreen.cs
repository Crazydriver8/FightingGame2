using UnityEngine;
using System.Collections;

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
