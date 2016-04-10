using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;


public class NodeControl : MonoBehaviour {
    // What the node refers to
    public string abilityName = "";

    // What the node is connected to
    public int parent = -1;
    public NodeControl[] connections = new NodeControl[4] { null, null, null, null };

    // Where the node is on the screen
    public Vector3 initPos;
    public Vector3 currPos;

    //public int currDepth = 1;

    public GameObject linePrefab = null;
    private GameObject instLine = null;

    private bool baseChild;

    private BlackBoardController bbc = null;
    public bool isInmotion = false;

	// Use this for initialization
	void Start () {
        initPos = transform.position;
        currPos = transform.position;
        bbc = GameObject.Find("Skills").GetComponent<BlackBoardController>();
	}
	
	// Update is called once per frame
	void Update () {
        
	}

    public bool CheckConstraints()
    {

        if (bbc.numSkill + 1 <= 3)
        {
            return true;
        }
        else
        {
            Debug.Log("Too many skills");
            return false;
        }
    }


    /* Events */
    public void OnBeginDrag()
    {
        //maintain current position in case of reset
        currPos = this.transform.position;
    }
    public void OnDrag()
    {
        isInmotion = true;
        //display image moving with mouse
        this.transform.position = Input.mousePosition;
    }
    public void OnEndDrag()
    {
        isInmotion = false;
        // Grab all nearby for this.depth() - 1, then this.depth()
        List<NodeControl> leavesOnVertical;
        List<NodeControl> leavesOnHorizontal;

        if (!CheckConstraints())
        {
            Debug.Log("Did not add skill");
            if (!CheckChildren())
            {
                this.transform.position = initPos;
            }
            else
            {
                this.transform.position = currPos;
            }
            return;
        }

        int depth = TreeEditor.S.GetDepthOf(this);
        //Debug.Log(TreeEditor.S.GetDepthOf(this));

        // Check to insert above... on success, quit out to prevent execution of anything else
        if(TreeEditor.S.leaves.TryGetValue(depth - 1, out leavesOnVertical) && leavesOnVertical.Count > 0 && this.parent == -1)
        {
            //Debug.Log("Found leaves on vertical");
            NodeControl vertNode = GetNearestNode(leavesOnVertical);
            if (!vertNode.CheckChildren((int)Constants.Branch.DOWN))
            {
                vertNode.SetChild(this);
                drawLine(vertNode, this);
                bbc.AddSkill(this.abilityName);
                return;
            }
        }

        // Check to insert left or right... on success, quit out to prevent execution of anything else
        if (TreeEditor.S.leaves.TryGetValue(depth, out leavesOnHorizontal) && leavesOnHorizontal.Count > 0 && this.parent == -1)
        {
            //Debug.Log("Found leaves on horizontal");
            NodeControl horizNode = GetNearestNode(leavesOnHorizontal);
            switch (this.CheckLeft(horizNode))
            {
                case true:
                    if (!horizNode.CheckChildren((int)Constants.Branch.LEFT))
                    {
                        horizNode.SetChild(this, (int)Constants.Branch.LEFT);
                        drawLine(horizNode, this);
                        bbc.AddSkill(this.abilityName);
                    }

                    return;

                default:
                    if (!horizNode.CheckChildren((int)Constants.Branch.RIGHT) && this.parent == -1)
                    {
                        horizNode.SetChild(this, (int)Constants.Branch.RIGHT);
                        drawLine(horizNode, this);
                        bbc.AddSkill(this.abilityName);
                    }

                    return;
            }
        }

        // On default, kick back to original position
        Debug.Log("No leaves found");
        if (!CheckChildren())
        {
            this.transform.position = initPos;
        }
        else
        {
            this.transform.position = currPos;
        }

    }


    /* Utilities */

    // True if there are children
    public bool CheckChildren(int index = -1)
    {
        // Check for a specific child
        if (index > -1 && index < connections.Length)
        {
            return connections[index] != null;
        }

        // Check if there are any children
        for (int i = 0; i < this.connections.Length; i++)
        {
            if (connections[i] != null && i != this.parent)
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
        for (int i = 0; i < node.connections.Length; i++)
        {
            if (node.connections[i] != null)
            {
                numChild++;
            }
        }
        //Debug.Log(node.name + " has " + numChild + " children");
        return numChild;
    }

    // Check nearest nodes and return minimum distance
    public NodeControl GetNearestNode(List<NodeControl> neighbors)
    {
        NodeControl heldNode = null;
        float minDist = float.MaxValue;
        foreach (NodeControl NodeC in neighbors)
        {
            if (NodeC != this)
            {
                float dist = Vector3.Distance(NodeC.transform.position, this.transform.position);
                //Debug.Log("Node " + NodeC.abilityName + "is dist: " + dist);

                //if it is within range, update heldnode to current acting node
                if (dist < minDist)
                {
                    minDist = dist;
                    heldNode = NodeC;
                }
            }
        }
        return heldNode;
    }

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
        if (TreeEditor.S.leaves.TryGetValue(TreeEditor.S.GetDepthOf(this) - (up ? 1 : 0), out leavesOnDepth))
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
    public bool CheckLeft(NodeControl thatObj)
    {
        if (this.transform.position.x < thatObj.transform.position.x) {
            return true;
        }
        return false;
    }

    // Sets passed in NodeControl node as child of this node
    public bool SetChild(NodeControl child, int direction = (int)Constants.Branch.DOWN)
    {

        int depth;
        // Get the parent's depth
        depth = TreeEditor.S.GetDepthOf(this);

        switch (direction)
        {
            //sets child as down child
            case (int)Constants.Branch.DOWN:

                // Check if there is already a DOWN child
                if (this.connections[(int)Constants.Branch.DOWN] == null || this.connections[(int)Constants.Branch.DOWN] == null)
                {
                    //Debug.Log("Setting down child");
                    this.connections[(int)Constants.Branch.DOWN] = child;
                    child.SetParent(this, (int)Constants.Branch.UP);

                    // New leaves get added to the dictionary (aka necronomicon)
                    TreeEditor.S.AddLeaf(depth + 1, child);
                    
                }
                else
                    return false;

                return true;

            //sets child as left child
            case (int)Constants.Branch.LEFT:
                if (this.connections[(int)Constants.Branch.LEFT] == null || this.connections[(int)Constants.Branch.LEFT])
                {
                    //Debug.Log("Setting left child");
                    this.connections[(int)Constants.Branch.LEFT] = child;
                    child.SetParent(this, (int)Constants.Branch.RIGHT);
                    TreeEditor.S.AddLeaf(depth, child);
                }
                else
                    return false;

                return true;

            //sets child as right child
            case (int)Constants.Branch.RIGHT:
                if (this.connections[(int)Constants.Branch.RIGHT] == null || this.connections[(int)Constants.Branch.RIGHT] == null)
                {
                    //Debug.Log("Setting right child");
                    this.connections[(int)Constants.Branch.RIGHT] = child;
                    child.SetParent(this, (int)Constants.Branch.LEFT);
                    TreeEditor.S.AddLeaf(depth, child);
                }
                else
                    return false;
                return true;

            default:
                return false;
        }
    }

    // Makes target node the parent of this node
    public bool SetParent(NodeControl parent, int direction)
    {
        if (this.connections[direction] == null)
        {
            this.parent = direction;
            this.connections[direction] = parent;
            return true;
        }

        return false;
    }

    // Gets rid of a child at an index
    public bool RemoveChild(int direction)
    {
        if (this.connections[direction] != null)
        {
            this.connections[direction].UnsetParent();
            this.connections[direction] = null;
            //this.parent = -1;

            return true;
        }

        return false;
    }

    // Gets rid of a parent
    public bool UnsetParent()
    {
        if (this.parent != -1 && this.connections[this.parent] != null)
        {
            this.connections[this.parent] = null;
            this.parent = -1;

            return true;
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

    // Draws line from parent to child
    public void drawLine(NodeControl parent, NodeControl child)
    {
        float lineWidth = 5f;
        //Canvas canvasRef = Canvas.FindObjectOfType<Canvas>();
        GameObject backRef = GameObject.Find("Background");

        deleteLine();

        instLine = Instantiate(linePrefab);
        instLine.transform.SetParent(backRef.transform, false);
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

    // This node can take no more childrenconnections
    public bool IsFull()
    {
        return this.connections[(int)Constants.Branch.DOWN] != null && this.connections[(int)Constants.Branch.LEFT] != null && this.connections[(int)Constants.Branch.RIGHT] != null;
    }

    /*TAKEN FROM SKILLTREE.CS*/
    public bool IsNull()
    {
        return abilityName == "" || abilityName == null;
    }

    public override string ToString()
    {
        string str = "{";

        // Name
        str += "\"name\" : \"" + this.abilityName + "\",";

        // Children (in resolution order)
        // Keep an empty string for no children
        if (this.connections != null)
        {

            str += "\"left\" : ";
            if (!(this.connections[(int)Constants.Branch.LEFT] == null) && this.parent != (int)Constants.Branch.LEFT) {
                //Debug.Log("Left node exists");
                str += connections[(int)Constants.Branch.LEFT].ToString() + ",";
            }
            else
            {
                str += "\"\",";
            }

            str += "\"right\" : ";
            if (!(this.connections[(int)Constants.Branch.RIGHT] == null) && this.parent != (int)Constants.Branch.RIGHT)
            {
                //Debug.Log("Right node exists");
                str += connections[(int)Constants.Branch.RIGHT].ToString() + ",";
            }
            else
            {
                str += "\"\",";
            }

            str += "\"down\" : ";
            if (!(this.connections[(int)Constants.Branch.DOWN] == null) && this.parent != (int)Constants.Branch.DOWN) {
                //Debug.Log("Down node exists");
                str += connections[(int)Constants.Branch.DOWN].ToString();
            }
            else
            {
                str += "\"\"";
            }

        }

        return str + "}";
    }

    //returns associated node to node bank
    public void ResetToOrigin()
    {
        NodeControl tempParent = null;
        // If there are no children, can remove node
        if (!CheckChildren())
        {
            if (this.parent != -1 && this.connections[this.parent] != null)
            {
                // Check and remove references to the soon-deleted node
                if (this.parent == ((int)Constants.Branch.UP))
                {
                    //Debug.Log("Removing Down Child");
                    tempParent = this.connections[this.parent];
                    tempParent.RemoveChild((int)Constants.Branch.DOWN);                    

                } else if (this.parent == ((int)Constants.Branch.LEFT))
                {
                    //Debug.Log("Removing Right Child");
                    tempParent = this.connections[this.parent];
                    tempParent.RemoveChild((int)Constants.Branch.RIGHT);

                } else if (this.parent == ((int)Constants.Branch.RIGHT))
                {
                    //Debug.Log("Removing Left Child");
                    tempParent = this.connections[this.parent];
                    tempParent.RemoveChild((int)Constants.Branch.LEFT);
                } else
                {
                    //Debug.Log("Removing Up Child (Shouldn't happen)");
                    tempParent = this.connections[this.parent];
                    tempParent.RemoveChild((int)Constants.Branch.UP);
                }
            }

            //Debug.Log("Remove parent reference and reset to skill bank");
            
            this.UnsetParent();
            this.deleteLine();
            this.transform.position = initPos;

        } else
        { 
            Debug.Log("Could not reset due to children");
        }
    }

    
}
