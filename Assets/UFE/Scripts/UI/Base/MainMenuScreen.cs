﻿using UnityEngine;
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
		UFE.StartVersusModeScreen();
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
        Debug.Log("Skill Tree Screen Clicked");
        UFE.StartSkillTreeScreen();
    }
}
