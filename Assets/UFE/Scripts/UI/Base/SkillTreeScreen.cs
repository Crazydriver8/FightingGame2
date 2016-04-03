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
        GameObject[] temp = GameObject.FindGameObjectsWithTag("Node");
        NodeControl reference = null;

        if (temp == null)
        {
            Debug.Log("Could not find nodes");
            return;
        }
        else
        {
            foreach(GameObject node in temp)
            {
                NodeControl tempNC = node.GetComponent<NodeControl>();
                Debug.Log("Ability Name: " + tempNC.abilityName);
                Debug.Log("Parent: " + tempNC.parent);
                if (tempNC.parent > -1 && tempNC.connections[tempNC.parent].abilityName == "Root")
                {
                    reference = tempNC;
                    break;
                }
            }
        }

        if (reference == null)
            Debug.Log("Fuk");
        
        if (reference == null)
        {
            Debug.Log("Could not output");
            return;
        }
        else Debug.Log("Found Node");

        Debug.Log(reference.ToString());
        Debug.Log("Save success");
    }
}