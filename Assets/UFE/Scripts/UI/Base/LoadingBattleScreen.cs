using UnityEngine;
using System.Collections;

public class LoadingBattleScreen : UFEScreen {
	#region public instance methods
	public virtual void StartBattle(){
		UFE.StartGame(UFE.config.gameGUI.gameFadeDuration);
	}
	#endregion
}
