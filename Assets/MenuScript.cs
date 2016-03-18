using UnityEngine;
using System.Collections;

public class MenuScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Z)) {
			Application.LoadLevel ("Arena");
		}
		if (Input.GetKeyDown (KeyCode.X)) {
			Application.LoadLevel ("SkillEditor");
		}
		if (Input.GetKeyDown (KeyCode.A)) {
			Application.LoadLevel ("SkillTree");
		}
		if (Input.GetKeyDown (KeyCode.Escape)) {
			Application.Quit ();
		}
	}
}
