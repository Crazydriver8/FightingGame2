using UnityEngine;
using System.Collections;

public class UFEScreen : MonoBehaviour{

    public bool canvasPreview = true;
    //public bool enableUFEInput = false;
	public GameObject firstSelectableGameObject = null;
	public bool wrapInput = true;

	public virtual void DoFixedUpdate(){}

	public virtual bool IsVisible(){
		return this.gameObject.activeInHierarchy;
	}

	public virtual void OnHide(){}
	public virtual void OnShow(){
        //UFE.PauseGame(!enableUFEInput);
    }
	public virtual void SelectOption(int option, int player){}
}
