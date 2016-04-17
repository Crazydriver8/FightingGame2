using UnityEngine;
using UnityEngine.UI;
using System;
using System.Reflection;

public class SkillTreeScreen : UFEScreen
{
    //private bool saveSkills = false;
    public GameObject captionPrefab;
    //private GameObject instCaption = null;

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
        //Debug.Log("Attempting to save");
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
                //Debug.Log("Ability Name: " + tempNC.abilityName);
                //Debug.Log("Parent: " + tempNC.parent);
                if (tempNC.parent > -1 && tempNC.connections[tempNC.parent].abilityName == "Root")
                {
                    reference = tempNC;
                    break;
                }
            }
        }

        if (reference == null)
            Debug.Log("Could not get NodeControl");
        
        if (reference == null)
        {
            Debug.Log("Could not output");
            return;
        }
        //else Debug.Log("Found Node");

        BlackBoardController bbc = GameObject.Find("Skills").GetComponent<BlackBoardController>();
        if (!bbc.SaveSkills())
        {
            Debug.Log("Could not save skills");
        }
        else
        {
            //Debug.Log("Save success");
        }

        Debug.Log(bbc.savedSkillList.Count + " Skill(s) Saved");
        string output = "";
        int i = 0;
        foreach(string s in bbc.savedSkillList)
        {
            if (bbc.savedSkillList.Count < (i + 1))
            {
                output += (s + " ");
            } else
            {
                output += (s + ", ");
            }
        }
        //Debug.Log(output);
        //Debug.Log(reference.ToString());

        bbc.DisplaySavedSkills(output);
        NameHolder store = GameObject.Find("Name").GetComponent<NameHolder>();
        Debug.Log(store.username + " has a skilltree = " + reference.ToString());
        StartCoroutine(PostDataToServer.PostSkillTree(store.username, reference.ToString()));
        //SkillTreeStructure newTree = new SkillTreeStructure().FromJSON("{\"name\" : \"Aggression\",\"left\" : {\"name\" : \"Power\",\"left\" : \"\",\"right\" : \"\",\"down\" : {\"name\" : \"Safety\",\"left\" : \"\",\"right\" : \"\",\"down\" : \"\"}},\"right\" : \"\",\"down\" : \"\"}");
        store.skillTree = new SkillTreeStructure().FromJSON(reference.ToString());
    }
}