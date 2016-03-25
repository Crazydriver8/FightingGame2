using UnityEngine;
using System.Collections;

public class TreeEditor : MonoBehaviour {

    // Dragging and dropping skill tree nodes
    private Vector3 screenPoint;
    private Vector3 offset;
    public string baseTag = "Base";
    public string nodeTag = "Node";

    // Objects past this point (y > line) are "in the tree" (and y < line is "not in the tree")
    public int skillBankLine = -5;

    // Make a line to show that there is a link
    // Otherwise, reset the object to where it started
    private Vector3 startPos;
    public float linkRange = 2.0f;
    private GameObject[] lines = new GameObject[3] { null, null, null };


    // Use this for initialization
    void Start () {
        startPos = transform.position;
	}
	
	// Update is called once per frame
	void Update () {

    }

    public void OnDrag() { transform.position = Input.mousePosition; }

    /* Override mouse clicks to move skill tree nodes when they are clicked on
     */
    void OnMouseDown()
    {
        screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
        offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
    }

    void OnMouseDrag()
    {
        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);

        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint) + offset;
        transform.position = curPosition;
    }

    void OnMouseUp()
    {
        // Reset if a line wasn't drawn and the object is not in the bank
        if (!(CanDrawLine(Vector2.up) || CanDrawLine(Vector2.left) || CanDrawLine(Vector2.right)) && transform.position.y >= skillBankLine)
        {
            // Clear out lines
            ClearLines();

            transform.position = startPos;
        }
    }


    /* Draw lines between skill tree nodes to indicate links
     */
    bool CanDrawLine(Vector2 direction)
    {
        // Find a target in range
        RaycastHit hit;
        
        if (Physics.Raycast(transform.position, direction, out hit, maxDistance: linkRange))
        {
            if (direction == Vector2.up && (hit.transform.gameObject.tag == baseTag || hit.transform.gameObject.tag == nodeTag))
            {
                StartCoroutine(DrawLine(transform.position, hit.transform.position, Color.red, 0));
                return true;
            }
            else if(direction == Vector2.left && hit.transform.gameObject.tag == nodeTag)
            {
                // Firing a ray to the left and hitting means that this is a right child
                StartCoroutine(DrawLine(transform.position, hit.transform.position, Color.red, 2));
                return true;
            }
            else if (direction == Vector2.right && hit.transform.gameObject.tag == nodeTag)
            {
                // Firing a ray to the right and hitting means that this is a left child
                StartCoroutine(DrawLine(transform.position, hit.transform.position, Color.red, 1));
                return true;
            }
        }

        return false;
    }

    IEnumerator DrawLine(Vector3 start, Vector3 end, Color color, int position)
    {
        ClearLines();

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
