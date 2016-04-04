using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class CaptionScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private bool hovering = false;
    public void OnPointerEnter(PointerEventData ped)
    {
        //Debug.Log("Hovering over" + gameObject.name);
        hovering = true;
    }

    public void OnPointerExit(PointerEventData ped)
    {
        //Debug.Log("Leaving " + gameObject.name);
        hovering = false;

    }

    public bool CaptionHoverOver()
    {
        return hovering;
    }
}
