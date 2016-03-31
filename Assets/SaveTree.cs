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

    public void SaveCurrentTree()
    {
        //have access to ability name, parent, children[], curr depth

        GameObject rootObj = GameObject.Find("RootNode");
        NodeControl root = rootObj.GetComponent<NodeControl>();

        // If the root node has children save the tree
        if (!root.CheckChildren())
        {
            BuildTree(root);
        }
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
