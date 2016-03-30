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

    public int currDepth = 1;

    public GameObject linePrefab = null;

    private GameObject instLine = null;

    private bool baseChild;

    private NodeControl parentRef = null;

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

        bool isLeft = false;

        //check if node has children
        if (upNode != null && lrNode != null)
        {
            // There can only be one... take the closer one
            if (Vector3.Distance(upNode.gameObject.transform.position, this.gameObject.transform.position) < Vector3.Distance(lrNode.gameObject.transform.position, this.gameObject.transform.position)) {
                if (upNode.children[(int)Constants.Branch.DOWN - 1] == "")
                {
                    // if the node found is set to down (upnode dependent), set lr to null
                    lrNode = null;
                }
                else
                {
                    // if the node found is not set to down (lrNode dependent), set up to null
                    upNode = null;
                }
            }
            else
            {
                //if operating on horizontal node, check if to the left of parent node
                isLeft = CheckLeft(this.transform.position, lrNode.transform.position);
                if (isLeft)
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
        // if upnode is found, attempt to set
        if (upNode != null)
        {
            Debug.Log("upnode set");

            // query for parent (already done)
            // Add child to the parent, which automatically sets the parent of the child
            upNode.SetChild(this);
            

            /*if (setMeAsChild(upNode))
            {
                currPos = this.transform.position;
                drawLine(upNode, this);
                Debug.Log("node set in position");
                return;
            } else
            {
                Debug.Log("Could not set node");
                if (CheckChildren())
                {
                    this.transform.position = currPos;
                }
                else
                {
                    this.transform.position = initPos;
                    deleteLine();
                    if (parentRef != null)
                    {
                        parentRef.unsetChild(this);
                    }
                    this.resetNodeAttributes();

                }
            }*/
        }
        //if lrNode is found, attempt to set
        else if (lrNode != null)
        {
            //Debug.Log("lrNode set");
            
            lrNode.SetChild(this, (int)(isLeft ? Constants.Branch.LEFT : Constants.Branch.RIGHT));

            /*if (setMeAsChild(lrNode))
            {
                currPos = this.transform.position;
                drawLine(lrNode, this);
                Debug.Log("node set in position");
            } else
            {
                Debug.Log("Could not set node");
                if (CheckChildren())
                {
                    this.transform.position = currPos;
                }
                else
                {
                    this.transform.position = initPos;
                    deleteLine();
                    if (parentRef != null)
                    {
                        parentRef.unsetChild(this);
                    }
                    this.resetNodeAttributes();

                }
            }*/
            return;
        }
        else
        {
            //if close to neither node, check if it has children
            //if there are children, reset to position in tree
            if (CheckChildren())
            {
                this.transform.position = currPos;
            }
            else
            {
                //if there are no children, reset to starting position in node bank
                this.transform.position = initPos;
                deleteLine();
                if (parentRef != null)
                {
                    parentRef.unsetChild(this);
                }
                this.resetNodeAttributes();

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
                //Debug.Log("has child");
                return true;
            }
        }

        return false;
    }

    // Returns number of children of NodeControl node
    public int NumChildren(NodeControl node)
    {
        int numChild = 0;
        for (int i = 0; i < node.children.Length; i++)
        {
            if (node.children[i] != "")
            {
                numChild++;
            }
        }
        Debug.Log(node.name + " has " + numChild + " children");
        return numChild;
    }

    // Check nearest nodes and return minimum distance
    public NodeControl GetNearestNode(bool up = true)
    {
        List<NodeControl> leavesOnDepth = null;

        NodeControl heldNode = null;
        //float minDist = float.MaxValue;
        float minDist = float.MaxValue;
        float maxDist = 100f;
        //Debug.Log("Trying " +(TreeEditor.S.leaves.TryGetValue(TreeEditor.S.GetDepthOf(this) - (up ? 1 : 0), out leavesOnDepth)));
        //Debug.Log("Depth is " + TreeEditor.S.GetDepthOf(this));
        if (checkDepth())
        {
            return null;
        }
        // if there are leaves on the depth
        if (TreeEditor.S.leaves.TryGetValue(TreeEditor.S.GetDepthOf(this) + (up ? 1 : 0), out leavesOnDepth))
        {
            //Debug.Log("found leaves on depth");
            baseChild = false;
            int i = 0;
            foreach (NodeControl NodeC in leavesOnDepth)
            {
                if (NodeC != this)
                {
                    //Debug.Log("Node " + i + " found");
                    float dist = Vector3.Distance(NodeC.transform.position, this.transform.position);
                    //Debug.Log("Node dist: " + dist);

                    //if it is within range, update heldnode to current acting node
                    if (dist < minDist)
                    {
                        minDist = dist;
                        heldNode = NodeC;
                    }

                    i++;
                }
            }
            //Debug.Log("Min distance is " + minDist);
            if (minDist > maxDist)
            {
                //Debug.Log("too far");
                return null;
            }
        } else
        {
            //no leaves on depth, check for base
            //Debug.Log("no leaves, check for base");
            NodeControl baseNode = GameObject.FindGameObjectWithTag(TreeEditor.S.baseTag).GetComponent<NodeControl>();
            
            if (baseNode != null)
            {
                float dist = Vector3.Distance(baseNode.transform.position, this.transform.position);
                //Debug.Log("Base node dist: " + dist);
                if (dist < minDist)
                {
                    heldNode = baseNode;
                    baseChild = true;
                } else
                {
                    baseChild = false;
                }
            }
        }

        return heldNode;
    }

    // Returns true if left 
    public bool CheckLeft(Vector3 thisObj, Vector3 thatObj)
    {
        if (thisObj.x < thatObj.x) {
            return true;
        }
        return false;
    }

    // Sets passed in NodeControl node as child
    public bool SetChild(NodeControl child, int direction = (int)Constants.Branch.DOWN)
    {

        int depth;

        switch (direction)
        {
            case (int)Constants.Branch.DOWN:
                // Get the parent's depth
                depth = TreeEditor.S.GetDepthOf(this);

                // Check if there is already a DOWN child
                if (this.children[(int)Constants.Branch.DOWN] == null || this.children[(int)Constants.Branch.DOWN] == "")
                {
                    this.children[(int)Constants.Branch.DOWN] = child.abilityName;
                    //this.parent = (int)Constants.Branch.UP;
                    //child.SetParent(this, Constants.Branch.UP);

                    // New leaves

                }
                else
                    return false;

                return true;


            case (int)Constants.Branch.LEFT:

            case (int)Constants.Branch.RIGHT:

            default:
                return false;
        }

        for (int i = 0; i < children.Length; i++)
        {
            if (children[i] == null || children[i] == "")
            {
                children[i] = child.abilityName;
                
                return true;
            }
        }
        return false;
    }

    // Sets self as child of NodeControl node
    public bool setMeAsChild(NodeControl node)
    {
        //check if no tree made
        /*Debug.Log("Setting node");
        if (baseChild)
        {
            Debug.Log("Creating new depth");
            if (CheckChildren(currDepth + 1))
            {
                Debug.Log("Elements above");
            } else
            {
                Debug.Log("no elements above");
                
            }


            if (node.name == "RootNode" && NumChildren(node) < 1)
            {
                List<NodeControl> temp = new List<NodeControl>(3);
                temp.Add(this);
                TreeEditor.S.leaves.Add(TreeEditor.S.GetDepthOf(this), temp);
                currDepth = TreeEditor.S.GetDepthOf(this);
                parent = node.name;
                parentRef = node;
                node.setChild(this);
                return true;
            } 
            if(node.name != "RootNode" && NumChildren(node) < 3)
            {
                List<NodeControl> temp = new List<NodeControl>(3);
                temp.Add(this);
                TreeEditor.S.leaves.Add(TreeEditor.S.GetDepthOf(this), temp);
                currDepth = TreeEditor.S.GetDepthOf(this);
                parent = node.name;
                parentRef = node;
                node.setChild(this);
                return true;
            }
        }
        else
        {
            if (TreeEditor.S.leaves == null)
            {
                Debug.Log("Cannot find in dictionary");
            }
            else
            {
                //check if too many children, if not add node
                Debug.Log("Leaves exists with length " + TreeEditor.S.leaves.Count);
                if (NumChildren(node) < 3 && !(TreeEditor.S.leaves[TreeEditor.S.GetDepthOf(this)].Contains(this)) && node.name != "RootNode") { 
                    TreeEditor.S.leaves[TreeEditor.S.GetDepthOf(this)].Add(this);
                    node.setChild(this);
                    currDepth = TreeEditor.S.GetDepthOf(this);
                    parent = node.name;
                    parentRef = node;
                    return true;
                }
            }
        }*/
        return false;
    }

    // Removes NodeControl removeNode from children array
    public bool unsetChild(NodeControl removeNode)
    {
        for (int i = 0; i < children.Length; i++)
        {
            if (children[i] == removeNode.name)
            {
                children[i] = "";
                Debug.Log("Removing reference to " + removeNode.name);
                
                return true; 
            }
        }
        return false;
    }

    // Returns true if depth is valid
    public bool checkDepth()
    {
        if (TreeEditor.S.GetDepthOf(this) >= 0) {
            return true;
        }
        return false;
    }

    // Resets attributes and removes references in tree
    public void resetNodeAttributes()
    {
        //reset depth to initial (1)
        this.currDepth = 1;
        if (parentRef != null)
        {
            if (TreeEditor.S.leaves.ContainsKey(TreeEditor.S.GetDepthOf(parentRef)))
            {
                if (TreeEditor.S.leaves[TreeEditor.S.GetDepthOf(parentRef)].Contains(this))
                {
                    parentRef.unsetChild(this);
                }
                if (TreeEditor.S.leaves[TreeEditor.S.GetDepthOf(this)].Contains(this))
                {
                    Debug.Log("Found child at depth");
                    TreeEditor.S.leaves[TreeEditor.S.GetDepthOf(this)].Remove(this);
                }
            }
        }
        this.parent = "";
    }

    // Draws line from parent to child
    public void drawLine(NodeControl parent, NodeControl child)
    {
        float lineWidth = 5f;
        Canvas canvasRef = Canvas.FindObjectOfType<Canvas>();

        deleteLine();

        instLine = Instantiate(linePrefab);
        instLine.transform.SetParent(canvasRef.transform, false);
        RectTransform test = instLine.GetComponent<RectTransform>();
        if (test != null)
        {
            Vector3 differenceVector = child.transform.position - parent.transform.position;
            test.sizeDelta = new Vector2(differenceVector.magnitude * 2, lineWidth);
            test.pivot = new Vector2(0, 0.5f);
            test.position = parent.transform.position;
            float angle = Mathf.Atan2(differenceVector.y, differenceVector.x) * Mathf.Rad2Deg;
            test.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    // Removes existing line associated with this node
    public void deleteLine()
    {
        if (instLine != null)
        {
            Destroy(instLine, 0f);
        }
    }

    // This node can take no more children
    public bool IsFull()
    {
        return this.children[(int)Constants.Branch.DOWN] != null && this.children[(int)Constants.Branch.DOWN] != "" && this.children[(int)Constants.Branch.LEFT] != null && this.children[(int)Constants.Branch.LEFT] != "" && this.children[(int)Constants.Branch.RIGHT] != null && this.children[(int)Constants.Branch.RIGHT] != "";
    }
}
