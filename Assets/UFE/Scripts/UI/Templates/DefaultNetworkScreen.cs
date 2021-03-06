﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DefaultNetworkScreen : NetworkGameScreen {
	#region public instance fields
	public AudioClip onLoadSound;
	public AudioClip music;
	public AudioClip selectSound;
	public AudioClip cancelSound;
	public AudioClip moveCursorSound;
	public float delayBeforePlayingMusic = 0.1f;
	public Text myInfoText;
	#endregion

	#region public override methods
	public override void DoFixedUpdate(){
		base.DoFixedUpdate();
		this.DefaultNavigationSystem(this.moveCursorSound, this.selectSound, this.cancelSound, this.GoToMainMenu);
	}
	
	public override void OnShow (){
		base.OnShow ();
		this.HighlightOption(this.FindFirstSelectable());
		
		if (this.music != null){
			UFE.DelayLocalAction(delegate(){UFE.PlayMusic(this.music);}, this.delayBeforePlayingMusic);
		}

		if (this.onLoadSound != null){
			UFE.DelayLocalAction(delegate(){UFE.PlaySound(this.onLoadSound);}, this.delayBeforePlayingMusic);
		}
		
		if (myInfoText != null) {
			myInfoText.text = "IP: " + GetIp() + "\nPort: " + UFE.config.networkOptions.port;
		}
	}
	#endregion
}
