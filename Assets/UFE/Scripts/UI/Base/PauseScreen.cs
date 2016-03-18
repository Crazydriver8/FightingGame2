using UnityEngine;
using System.Collections;

public class PauseScreen : UFEScreen {
	public virtual void GoToMainMenu(){
        UFE.PauseGame(false);
		UFE.StartMainMenuScreen();
	}

	public virtual void ResumeGame(){
		UFE.PauseGame(false);
	}
}
