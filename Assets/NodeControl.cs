using UnityEngine;
using System.Collections;

public class NodeControl : MonoBehaviour {
    public string abilityName = "";
    public string parent = "";
    public string[] children;
    public int nodeNum = 0;
    public Vector3 initPos;
	// Use this for initialization
	void Start () {
        initPos = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    public void OnBeginDrag()
    {
        initPos = transform.position;
    }
    public void OnDrag() {
        
        transform.position = Input.mousePosition;
    }
    public void OnEndDrag()
    {
        Debug.Log(transform.position);
        GameObject Parent = null;
        GameObject[] Parents = GameObject.FindGameObjectsWithTag("Base");
        for(int i = 0; i < Parents.Length; i++)
        {
            Debug.Log(Parents[i].transform.position);
            Parent = Parents[i];
        }
        if (Parent != null)
        {
            double xDiff = transform.position.x - Parent.transform.position.x;
            double yDiff = transform.position.y - Parent.transform.position.y;
            Debug.Log("X difference: " + xDiff + ", Y difference: " + yDiff);
            if(yDiff > 0)
            {
                transform.position = initPos;
            }
        } else
        {
            Debug.Log("Parent not found");
        }
    }
}
