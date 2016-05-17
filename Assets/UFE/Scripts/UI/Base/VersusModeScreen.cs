using UnityEngine;
using System.Collections;

public class VersusModeScreen : UFEScreen{
	public virtual void SelectPlayerVersusPlayer(){
		UFE.StartPlayerVersusPlayer();
	}

	public virtual void SelectPlayerVersusCpu(){
        Debug.Log("Player vs CPU");
        NameHolder temp = GameObject.Find("Name").GetComponent<NameHolder>();
        temp.setGameMode("playerVsFuzzy");
		UFE.StartPlayerVersusCpu();
	}

    public virtual void SelectPlayerVsSelfAI()
    {
        Debug.Log("Player vs Self AI");
        NameHolder temp = GameObject.Find("Name").GetComponent<NameHolder>();
        temp.setGameMode("playerVsDecision");
        UFE.StartPlayerVersusCpu();
    }

	public virtual void SelectCpuVersusCpu(){
        Debug.Log("Decision vs Fuzzy");
        NameHolder temp = GameObject.Find("Name").GetComponent<NameHolder>();
        temp.setGameMode("fuzzyVsDecision");
        UFE.StartCpuVersusCpu();
	}

    public virtual void SelectSelfAIvsSelf()
    {
        Debug.Log("Decision vs Decision");
        NameHolder temp = GameObject.Find("Name").GetComponent<NameHolder>();
        temp.setGameMode("decisionVsDecision");
        UFE.StartCpuVersusCpu();
    }

	public virtual void GoToMainMenu(){
		UFE.StartMainMenuScreen();
	}
}
