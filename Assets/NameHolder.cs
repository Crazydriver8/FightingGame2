using UnityEngine;
using System.Collections;

public class NameHolder : MonoBehaviour {
    public string username;
    public bool diagnosticMode = false;
    private bool roundStarted = false;
    public SkillTreeStructure skillTree;
    public string gameMode;
    

    void Start()
    {
        skillTree = new SkillTreeStructure();
    }

    public void SetName(string outName)
    {
        username = outName;
    }
    public void SetDiagnosticMode(bool mode)
    {
        diagnosticMode = mode;
    }
    void update()
    {
        Debug.Log(username);
    }

    public bool ExistsTree()
    {
        if (skillTree.IsNull())
        {
            // Attempt to retrieve from server
            WWW getTree = new WWW(Constants.getTreeUrl + "playerName=" + this.username);

            while (!getTree.isDone) { }

            if (getTree.error != null)
            {
                Debug.Log("There was a GET error: " + getTree.error);
                return false;
            }
            else if (getTree.text == null || getTree.text == "")
            {
                Debug.Log("There was a GET error: No data");
                return false;
            }
            else
            {
                skillTree = new SkillTreeStructure().FromJSON(getTree.text);
                //Debug.Log("Built tree from: " + getTree.text);
                return true;
            }
        }
        else
            return true;
    }

    public bool getRoundStarted()
    {
        return roundStarted;
    }

    public void setRoundStarted(bool roundUp)
    {
        this.roundStarted = roundUp;
    }

    public void setGameMode(string gm)
    {
        this.gameMode = gm;
    }
}
