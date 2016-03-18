using UnityEngine;
using System.Collections;

public class HostGameScreen : UFEScreen{
	public virtual void GoToNetworkGameScreen(){
		UFE.StartNetworkGameScreen();
	}

	public virtual void GoToConnectionLostScreen(){
		UFE.StartConnectionLostScreen();
	}

	public virtual void StartHostGame() {
		UFE.HostGame();
	}
}
