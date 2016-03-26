using UnityEngine;
using System.Collections;

public class NodeControl : MonoBehaviour {
    public string abilityName = "";
    public string parent = "";
    public string[] children;
    public int nodeNum = 0;
    public Vector3 initPos;
    public Vector3 currPos;
    private bool placed;
    private bool resetting;
    private GameObject[] lines = new GameObject[3] { null, null, null };
    // Use this for initialization
    void Start () {
        initPos = this.transform.position;
        currPos = this.transform.position;
        resetting = false;
        placed = false;
        children = new string[3];
        for(int i = 0; i < 3; i++) 
        {
            children[i] = "";
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    public void OnBeginDrag()
    {
        currPos = this.transform.position;
    }
    public void OnDrag() {
        
        this.transform.position = Input.mousePosition;
    }
    public void OnEndDrag()
    {
        Debug.Log(children.Length);
        if (placed && children.Length > 0)
        {
            this.transform.position = currPos;
        }
        Debug.Log(this.transform.position);
        //first check for if close to placed node
        Debug.Log("Checking for placed nodes");
        if (GetPlacedNode())
        {
            return;
        }
        //if that fails, check for root node
        Debug.Log("Checking for root node");
        if (GetRootNode())
        {
            return;
        }
        if (!resetting) {
            Debug.Log("reset");
            this.transform.position = currPos;
            return;
        }
        this.transform.position = initPos;
        this.tag = "Node";

    }
    private bool GetPlacedNode()
    {
        GameObject[] objectsInScene = GameObject.FindGameObjectsWithTag("PlacedNode");
        if (objectsInScene != null && objectsInScene.Length > 0)
        {
            for (int i = 0; i < objectsInScene.Length; i++)
            {
                if (WithinRange(objectsInScene[i]) && objectsInScene[i].name != this.name)
                {
                    Debug.Log("Success with parent: " + objectsInScene[i].name);
                    bool temp = objectsInScene[i].GetComponent<NodeControl>().SetChild(this.name);
                    if (temp)
                    {
                        nodeNum = objectsInScene.Length + 1;
                        placed = true;
                        this.tag = "PlacedNode";
                        parent = objectsInScene[i].name;
                        //StartCoroutine(DrawLine(this.transform.position, objectsInScene[i].transform.position, Color.red, 0));
                        return true;
                    } else
                    {
                        Debug.Log("Too many children");
                    }
                }
                else
                {
                    //Debug.Log("fail");
                    //return false;
                }
            }
        }
        return false;
    }

    private bool GetRootNode()
    {
        GameObject[] basesInScene = GameObject.FindGameObjectsWithTag("Base");
        if (basesInScene != null && basesInScene.Length > 0)
        {
            for (int i = 0; i < basesInScene.Length; i++)
            {
                if (WithinRange(basesInScene[i]) && basesInScene[i].name != this.name)
                {
                    Debug.Log("Success with parent: " + basesInScene[i].name);
                    nodeNum = 1;
                    placed = true;
                    this.tag = "PlacedNode";
                    parent = basesInScene[i].name;
                    //StartCoroutine(DrawLine(this.transform.position, basesInScene[i].transform.position, Color.red, 0));
                    return true;
                }
                else
                {
                    //Debug.Log("fail");
                    //return false;
                }
            }
        }
        return false;
    }

    private bool WithinRange(GameObject obj)
    {
        float dist = Vector3.Distance(this.transform.position, obj.transform.position);
        Debug.Log(dist);
        if (dist < 80)
        {
            return true;
        }
        if (dist > 200)
        {
            resetting = true;
            return false;
        } else
        {
            resetting = false;
        }
        return false;
    }

    public bool SetChild(string childName)
    {
        Debug.Log("Checking children for "+ this.name);
        for(int i = 0; i < this.children.Length; i++)
        {
            if (this.children[i] == null || children[i] == "")
            {
                this.children[i] = childName;
                return true;
            }
        }
        Debug.Log("All spaces filled");
        return false;
    }

    public string[] GetChildren()
    {
        return children;
    }

    IEnumerator DrawLine(Vector3 start, Vector3 end, Color color, int position)
    {
        ClearLines();
        Debug.Log("drawing");
        lines[position] = new GameObject();
        lines[position].transform.position = start;
        lines[position].AddComponent<LineRenderer>();
        LineRenderer lr = lines[position].GetComponent<LineRenderer>();
        lr.SetColors(color, color);
        lr.SetWidth(0.1f, 0.1f);
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        yield return null;
    }
    void ClearLines()
    {
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i] != null)
            {
                GameObject.Destroy(lines[i]);
                lines[i] = null;
            }
        }
    }
}
