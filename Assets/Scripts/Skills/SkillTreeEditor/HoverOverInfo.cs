using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class HoverOverInfo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool hovering = false;
    public GameObject captionPrefab;
    private GameObject instCaption = null;
    private Transform tempTrans = null;
    private Text capTitle = null;
    private Text capSum = null;
    private string abilityName = "";

    public void OnPointerEnter(PointerEventData ped)
    {
        Canvas canvasRef = Canvas.FindObjectOfType<Canvas>();
        //Debug.Log("Hovering over" + gameObject.name);
        hovering = true;

        instCaption = GameObject.Instantiate(captionPrefab);

        instCaption.transform.SetParent(canvasRef.transform, false);
        instCaption.transform.position = gameObject.transform.position + Vector3.up;

        capTitle = instCaption.GetComponentInChildren<Text>();
        capTitle.text = abilityName;

    }

    public void OnPointerExit(PointerEventData ped)
    {
        //Debug.Log("Leaving " + gameObject.name);
        hovering = false;
        if (instCaption != null)
        {
            Destroy(instCaption, 0f);
        }
    }

    void Update()
    {
        if (hovering && Input.GetMouseButtonDown(1))
        {
            Debug.Log("Attempting to delete "+ gameObject.name);
            NodeControl nodeRef = gameObject.GetComponent<NodeControl>();
            if (nodeRef == null) return;
            nodeRef.ResetToOrigin();
            Destroy(instCaption, 0f);
        }
    }

    void Start()
    {
        NodeControl temp = gameObject.GetComponent<NodeControl>();
        if (temp != null)
        {
            abilityName = temp.abilityName;
        } else
        {
            Debug.Log("Could not find ability name");
        }
    }
}