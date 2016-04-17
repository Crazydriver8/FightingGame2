using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MenuScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Z)) {
			//Application.LoadLevel ("Arena");
            //SceneManager.LoadScene("Arena");
		}
		if (Input.GetKeyDown (KeyCode.X)) {
			//Application.LoadLevel ("SkillEditor");
            //SceneManager.LoadScene("SkillEditor");
		}
		if (Input.GetKeyDown (KeyCode.A)) {
            //Application.LoadLevel ("SkillTree");
            //SceneManager.LoadScene("SkillTree");
		}
		if (Input.GetKeyDown (KeyCode.Escape)) {
			Application.Quit ();
		}
	}
}
