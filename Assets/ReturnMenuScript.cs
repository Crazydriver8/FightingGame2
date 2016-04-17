using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ReturnMenuScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Q)) {
			//Application.LoadLevel ("Menu");
            SceneManager.LoadScene("Menu");
		}
		if (Input.GetKeyDown (KeyCode.Escape)) {
			Application.Quit ();
		}
	}
}
