using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class NodeControl : MonoBehaviour {
    // What the node refers to
    public string abilityName = "";

    // What the node is connected to
    public string parent = "";
    public string[] children = new string[3] { "", "", "" };

    // Where the node is on the screen
    public Vector3 initPos;
    public Vector3 currPos;

    private bool baseChild;

	// Use this for initialization
	void Start () {
        initPos = transform.position;
        currPos = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    /* Events */
    public void OnBeginDrag()
    {
        //maintain current position in case of reset
        currPos = this.transform.position;
    }
    public void OnDrag()
    {
        //display image moving with mouse
        this.transform.position = Input.mousePosition;
    }
    public void OnEndDrag()
    {
        NodeControl upNode = GetNearestNode(),
                    lrNode = GetNearestNode(false);

        //check if node has children
        if (upNode != null && lrNode != null)
        {
            // There can only be one... take the closer one
            if (Vector3.Distance(upNode.gameObject.transform.position, this.gameObject.transform.position) < Vector3.Distance(lrNode.gameObject.transform.position, this.gameObject.transform.position)) {
                if (upNode.children[(int)Constants.Branch.DOWN - 1] == "")
                {
                    lrNode = null;
                }
                else
                {
                    upNode = null;
                }
            }
            else
            {
                if (CheckLeft(this.transform.position, lrNode.transform.position))
                {
                    if (lrNode.children[(int)Constants.Branch.LEFT - 1] == "")
                    {
                        upNode = null;
                    }
                    else
                    {
                        lrNode = null;
                    }
                }
                else
                {
                    if (lrNode.children[(int)Constants.Branch.RIGHT - 1] == "")
                    {
                        upNode = null;
                    }
                    else
                    {
                        lrNode = null;
                    }
                }
            }
        }

        if (upNode != null)
        {

            currPos = this.transform.position;
            setChild(upNode);
            return;
        }
        else if (lrNode != null)
        {
            currPos = this.transform.position;
            setChild(lrNode);
            return;
        }
        else
        {
            if (CheckChildren())
            {
                this.transform.position = currPos;

            }
            else
            {
                this.transform.position = initPos;
            }
            return;
        }
        return;
    }


    /* Utilities */

    // True if there are children
    public bool CheckChildren(int index = -1)
    {
        // Check for a specific child
        if (index > -1 && index < children.Length)
        {
            return children[index] != "";
        }

        // Check if there are any children
        for (int i = 0; i < this.children.Length; i++)
        {
            if (children[i] != "")
            {
                Debug.Log("has child");
                return true;
            }
        }

        return false;
    }

    public NodeControl GetNearestNode(bool up = true)
    {
        List<NodeControl> leavesOnDepth = null;

        NodeControl heldNode = null;
        //float minDist = float.MaxValue;
        float minDist = float.MaxValue;
        float maxDist = 100f;
        Debug.Log("Trying " +(TreeEditor.S.leaves.TryGetValue(TreeEditor.S.GetDepthOf(this) - (up ? 1 : 0), out leavesOnDepth)));
        //Debug.Log("Depth is " + TreeEditor.S.GetDepthOf(this));
        // if there are leaves on the depth
        if (TreeEditor.S.leaves.TryGetValue(TreeEditor.S.GetDepthOf(this) - (up ? 1 : 0), out leavesOnDepth))
        {
            Debug.Log("found leaves on depth");
            int i = 0;
            foreach (NodeControl NodeC in leavesOnDepth)
            {
                Debug.Log("Node " + i + " found");
                float dist = Vector3.Distance(NodeC.transform.position, this.transform.position);
                Debug.Log("Node dist: " + dist);
                
                //if it is within range, update
                if (dist < minDist)
                {
                    minDist = dist;
                    heldNode = NodeC;
                }
                i++;
            }
        } else
        {
            //no leaves on depth, check for base
            NodeControl baseNode = GameObject.FindGameObjectWithTag(TreeEditor.S.baseTag).GetComponent<NodeControl>();
            if (baseNode != null)
            {
                float dist = Vector3.Distance(baseNode.transform.position, this.transform.position);
                //Debug.Log("Base node dist: " + dist);
                if (dist < minDist)
                {
                    heldNode = baseNode;
                    baseChild = true;
                }
            }
        }
        return heldNode;
    }

    public bool CheckLeft(Vector3 thisObj, Vector3 thatObj)
    {
        if (thisObj.x < thatObj.x) {
            return true;
        }
        return false;
    }

    public bool setChild(NodeControl node)
    {
        //check if no tree made
        Debug.Log("Setting node");
        if (baseChild)
        {
            Debug.Log("Creating new depth");
            List<NodeControl> temp = new List<NodeControl>(3);
            temp.Add(node);
            TreeEditor.S.leaves.Add(TreeEditor.S.GetDepthOf(this), temp);
            
        }
        else
        {
            //check if too many children, if not add node
            for (int i = 0; i < 3; i++)
            {
                if (TreeEditor.S.leaves[i] == null)
                {
                    TreeEditor.S.leaves[i].Add(node);
                    return true;
                }
            }
        }
        return false;
    }
}
