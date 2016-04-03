using UnityEngine;
using System.Collections;

public class SaveTree : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public bool SaveCurrentTree()
    {
        Debug.Log("In SaveCurrentTree()");
        Debug.Log("Attempting to find TreeEdit object");
        GameObject reference = GameObject.Find("TreeEdit");
        if (reference == null) return false;

        Debug.Log("Attempting to find TreeEdit.TreeEditor");
        TreeEditor treeRef = reference.GetComponent<TreeEditor>();
        if (treeRef == null) return false;

        Debug.Log("Attempting to print tree");
        if (!treeRef.PrintTree())
        {
            Debug.Log("Could not print tree");
            return false;
        }
        return true;
    }

    public SkillTree BuildTree(NodeControl root)
    {
        /*node = new SkillTree(name of skill);
        foreach child
            if child: buildTree(object containing child)
            else set to empty(call new SkillTreeInfo() no arguments)

        SkillTree node = new SkillTree();
        NodeControl[] userNodes = GameObject.FindObjectsOfType<NodeControl>();
        for (int i = 0; i < userNodes.Length; i++)
        {
            if (userNodes[i].parent != "")
            {
                this.BuildTree(userNodes[i]);
            }
            else
            {
                new SkillTree();
            }
        }
        */

        return null;
    }
}
