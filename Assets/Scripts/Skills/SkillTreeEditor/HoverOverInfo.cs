using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class HoverOverInfo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool hovering = false;
    public GameObject captionPrefab;
    private GameObject instCaption = null;
    //private Transform tempTrans = null;
    //private Text capTitle = null;
    //private Text capSum = null;
    private string abilityName = "";

    public void OnPointerEnter(PointerEventData ped)
    {
        //Debug.Log("Hovering over" + gameObject.name);
        if (!gameObject.GetComponent<NodeControl>().isInmotion)
        {
            InstCaption();
            hovering = true;
        } else
        {
            DeleteCaption();
            hovering = false;
        }
        
    }

    public void OnPointerExit(PointerEventData ped)
    {
        //Debug.Log("Leaving " + gameObject.name);
        hovering = false;
        
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
        if (!hovering)
        {
            DeleteCaption();
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

    public void InstCaption()
    {
        Canvas canvasRef = Canvas.FindObjectOfType<Canvas>();
        instCaption = GameObject.Instantiate(captionPrefab);

        instCaption.transform.SetParent(canvasRef.transform, false);
        instCaption.transform.position = gameObject.transform.position + Vector3.up;

        Text[] captionContents = instCaption.GetComponentsInChildren<Text>();
        Text captionTitle = captionContents[0];
        captionTitle.text = this.abilityName;

        Text captionText = captionContents[1];
        string cap = "";

        GameObject skillNodes = GameObject.Find("SkillTreeNodes");
        if (skillNodes != null)
        {
            switch (this.abilityName)
            {
                case "Ghost":
                    Ghost g = skillNodes.GetComponent<Ghost>();
                    cap = g.skillDescription;
                    break;
                case "Applause":
                    Applause a = skillNodes.GetComponent<Applause>();
                    cap = a.skillDescription;
                    break;
                case "Coach":
                    Coach c = skillNodes.GetComponent<Coach>();
                    cap = c.skillDescription;
                    break;
                case "Surprise":
                    Surprise s = skillNodes.GetComponent<Surprise>();
                    cap = s.skillDescription;
                    break;
                case "Golf Clap":
                    GolfClap gc = skillNodes.GetComponent<GolfClap>();
                    cap = gc.skillDescription;
                    break;
            }
        }
        captionText.text = cap;
    }

    public void DeleteCaption()
    {
        if (instCaption != null)
        {
            Destroy(instCaption, 0f);
        }
    }
}