using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class DefaultSkillTreeScreen : SkillTreeScreen{
    #region public instance fields
    public AudioClip onLoadSound;
    public AudioClip music;
    public AudioClip selectSound;
    public AudioClip cancelSound;
    public AudioClip moveCursorSound;
    public bool stopPreviousSoundEffectsOnLoad = false;
    public float delayBeforePlayingMusic = 0.1f;

    public Button buttonNetwork;
    #endregion
    #region public override methods
    public override void DoFixedUpdate()
    {
        base.DoFixedUpdate();
        this.DefaultNavigationSystem(this.selectSound, this.moveCursorSound, this.cancelSound, null);
    }

    public override void OnShow()
    {
        base.OnShow();
        this.HighlightOption(this.FindFirstSelectable());

        if (this.music != null)
        {
            UFE.DelayLocalAction(delegate () { UFE.PlayMusic(this.music); }, this.delayBeforePlayingMusic);
        }

        if (this.stopPreviousSoundEffectsOnLoad)
        {
            UFE.StopSounds();
        }

        if (this.onLoadSound != null)
        {
            UFE.DelayLocalAction(delegate () { UFE.PlaySound(this.onLoadSound); }, this.delayBeforePlayingMusic);
        }
    }
    #endregion
}
