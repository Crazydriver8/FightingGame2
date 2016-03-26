using UnityEngine;
using System.Collections;

public class NameHolder : MonoBehaviour {
    public string username;

    public void SetName(string outName)
    {
        username = outName;
    }
    void update()
    {
        Debug.Log(username);
    }
}
