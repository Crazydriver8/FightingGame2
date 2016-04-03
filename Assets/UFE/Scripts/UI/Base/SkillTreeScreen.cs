using UnityEngine;
using System;
using System.Reflection;

public class SkillTreeScreen : UFEScreen
{
    public virtual void Quit()
    {
        UFE.Quit();
    }

    public virtual void GoToStoryModeScreen()
    {
        UFE.StartStoryMode();
    }

    public virtual void GoToVersusModeScreen()
    {
        UFE.StartVersusModeScreen();
    }

    public virtual void GoToTrainingModeScreen()
    {
        UFE.StartTrainingMode();
    }

    public virtual void GoToNetworkPlayScreen()
    {
        UFE.StartNetworkGameScreen();
    }

    public virtual void GoToOptionsScreen()
    {
        UFE.StartOptionsScreen();
    }

    public virtual void GoToMainMenuScreen()
    {
        NodeControl[] temp = GameObject.FindObjectsOfType<NodeControl>();
        for (int i = 0; i < temp.Length; i++)
        {
            temp[i].deleteLine();
        }
        UFE.StartMainMenuScreen();

    }

    public virtual void GoToCreditsScreen()
    {
        UFE.StartCreditsScreen();
    }

    public virtual void GoToSkillTreeScreen()
    {
        UFE.StartSkillTreeScreen();
    }

    public virtual void SaveSkillTree()
    {
        Debug.Log("Attempting to save");
        GameObject temp = GameObject.Find("TreeEdit");
        
        if (temp == null)
        {
            Debug.Log("Could not find TreeEdit");
            return;
        }

        TreeEditor reference = temp.GetComponent<TreeEditor>();
        if (reference == null)
        {
            Debug.Log("Could not output");
            return;
        }
        
        if (!reference.PrintTree())
        {
            Debug.Log("Could not print tree");
            return;
        }
        Debug.Log("Save success");
    }
}