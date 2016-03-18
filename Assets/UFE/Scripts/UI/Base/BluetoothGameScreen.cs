using UnityEngine;
using System.Collections;
using System;
using System.Net;

public class BluetoothGameScreen : UFEScreen{
	public virtual void GoToMainMenu(){
		UFE.StartMainMenuScreen();
	}

	public virtual void HostGame() {
		UFE.HostBluetoothGame();
	}

	public virtual void JoinGame() {
		UFE.JoinBluetoothGame();
	}
}
