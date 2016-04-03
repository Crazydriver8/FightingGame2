using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class TreeEditor : MonoBehaviour {
    public static TreeEditor S;

    // Dragging and dropping skill tree nodes
    private Vector3 screenPoint;
    private Vector3 offset;
    public string baseTag = "Base";
    public string nodeTag = "Node";

    // Objects past this point (y > line) are "in the tree" (and y < line is "not in the tree")
    public int skillBankLine = -5;

    // This is how far apart the depths are
    public float depthSpacing = -150.0f;

    // The "root" node; a node that can't be moved or otherwise edited
    public NodeControl baseNode;

    // Here are all the skills; save a reference to their controller
    //public Dictionary<string, NodeControl> skills =  new Dictionary<string, NodeControl> ();

    // It is currently possible to add children to these nodes
    // Keep them grouped by depth
    // Calculate depth using y (vertical) coordinate
    public Dictionary<int, List<NodeControl>> leaves = new Dictionary<int, List<NodeControl>>();


    // Use this to initialize
    void Start()
    {
        // Find the "root" and add it to the dictionary
        baseNode = GameObject.FindGameObjectWithTag(baseTag).GetComponent<NodeControl>();
        leaves.Add(0, new List<NodeControl>() { baseNode });

        // Get all nodes by tag
        foreach(GameObject node in GameObject.FindGameObjectsWithTag(nodeTag))
        {
            NodeControl nodeControl = node.GetComponent<NodeControl>();
            //skills[nodeControl.abilityName] = nodeControl;
        }

        // Static reference to editor
        S = this;
    }


    /* Utilities
     */
    // Calculate the depth
    public int GetDepthOf(NodeControl node)
    {
        //Debug.Log("Finding depth of " + node.name);

        Debug.Log("Node: " + node.GetComponent<RectTransform>().anchoredPosition.y);
        Debug.Log("Base: " + baseNode.GetComponent<RectTransform>().anchoredPosition.y);

        float diff = Mathf.CeilToInt((node.GetComponent<RectTransform>().anchoredPosition.y - baseNode.GetComponent<RectTransform>().anchoredPosition.y));
        //diff = Mathf.Abs(diff);
        Debug.Log("Distance to base: " + diff + ", DepthSpacing: " + depthSpacing);
        
        diff = diff / depthSpacing;
        //Debug.Log("Depth: " + Mathf.RoundToInt(diff));

        return Mathf.CeilToInt(diff);
    }

    // Add a leaf or a bunch of leaves
    public bool AddLeaf(int leafDepth, List<NodeControl> nodeList)
    {
        leaves.Add(leafDepth, nodeList);
        return true;
    }
    public bool AddLeaf(int leafDepth, NodeControl node)
    {
        if (leaves.ContainsKey(leafDepth))
        {
            leaves[leafDepth].Add(node);
        }
        else
        {
            leaves[leafDepth] = new List<NodeControl>() { node };
        }
        Debug.Log("ADDING LEAF: " + leaves[leafDepth][0].abilityName + " @ DEPTH: " + leafDepth);
        return true;
    }

    // This node no longer needs to be on the frontier
    public bool Completed(NodeControl node)
    {
        if (node.IsFull())
        {
            return this.leaves[GetDepthOf(node)].Remove(node);
        }

        return false;
    }

    public bool PrintTree()
    {
        if (leaves == null)
        {
            Debug.Log("Leaves not found");
            return false;
        }
        Debug.Log("Printing tree");
        // Go through each depth
        string temp = "";
        foreach (KeyValuePair<int, List<NodeControl>> kvp in leaves)
        {
            Debug.Log("DEPTH IS " + kvp.Key);
            List<NodeControl> tempNodeList = kvp.Value;
            foreach(NodeControl node in tempNodeList)
            {
                string ability = node.abilityName;
                if (ability == "" || ability == null) ability = "nothing";
                Debug.Log("node is " + ability);
                temp += string.Format("Depth = {0}, Ability = {1}, ", kvp.Key, ability);
            }
        }
        Debug.Log(temp);

        return true;
    }
}
