using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class DragPanel : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{

    private Vector2 pointerOffset;
    private RectTransform canvasRectTransform;
    private RectTransform panelRectTransform;

    //Basic tags for base and general nodes
    public string baseTag = "Base";
    public string nodeTag = "Node";
    //Past this point is 'in tree'
    public int skillBankLine = -80;

    //Draw line utilities
    private Vector2 startPos;
    public float linkRange = 2.0f;
    private GameObject[] lines = new GameObject[2]
    {
        null,
        null
    };

    void Awake()
    {
        /*Canvas canvas = GetComponentInParent<Canvas>();*/
        Canvas canvas = Canvas.FindObjectOfType<Canvas>();
        
        if (canvas != null)
        {
            Debug.Log("Canvas is not null\nGetting Canvas Transform");
            canvasRectTransform = canvas.transform as RectTransform;
            Debug.Log("Getting Panel Transform");
            panelRectTransform = transform as RectTransform;
            startPos = panelRectTransform.localPosition;
        } else
        {
            Debug.Log("Canvas is Null");
        }
    }

    public void OnPointerDown(PointerEventData data)
    {
        //Debug.Log("Click");
        //Debug.Log(panelRectTransform.name);
        panelRectTransform.SetAsLastSibling();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(panelRectTransform, data.position, data.pressEventCamera, out pointerOffset);
    }

    public void OnDrag(PointerEventData data)
    {
        //Debug.Log("Dragging "+panelRectTransform.name);
        if (panelRectTransform == null)
            return;

        Vector2 pointerPostion = ClampToWindow(data);

        Vector2 localPointerPosition;
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRectTransform, pointerPostion, data.pressEventCamera, out localPointerPosition
        ))
        {
            panelRectTransform.localPosition = localPointerPosition - pointerOffset;
        }
    }

    public void OnPointerUp(PointerEventData data)
    {
        Debug.Log("Pointer up");
        //Reset if a line wasn't drawn
        if (!(CanDrawLine(Vector2.up) || CanDrawLine(Vector2.left) || CanDrawLine(Vector2.right)) && transform.position.y >= skillBankLine)
        {
            ClearLines();
            Debug.Log("Reset to start");
            panelRectTransform.localPosition = startPos;
        }
    }

    Vector2 ClampToWindow(PointerEventData data)
    {
        Vector2 rawPointerPosition = data.position;

        Vector3[] canvasCorners = new Vector3[4];
        canvasRectTransform.GetWorldCorners(canvasCorners);

        float clampedX = Mathf.Clamp(rawPointerPosition.x, canvasCorners[0].x, canvasCorners[2].x);
        float clampedY = Mathf.Clamp(rawPointerPosition.y, canvasCorners[0].y, canvasCorners[2].y);

        Vector2 newPointerPosition = new Vector2(clampedX, clampedY);
        //Vector2 newPointerPosition = rawPointerPosition;
        return newPointerPosition;
    }

    bool CanDrawLine(Vector2 direction)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, direction, out hit, maxDistance: linkRange))
        {
            if (direction == Vector2.up && (hit.transform.gameObject.tag == baseTag || hit.transform.gameObject.tag == nodeTag))
            {
                StartCoroutine(DrawLine(transform.position, hit.transform.position, Color.red, 0));
                return true;
            }
            else if (direction == Vector2.left && hit.transform.gameObject.tag == nodeTag)
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