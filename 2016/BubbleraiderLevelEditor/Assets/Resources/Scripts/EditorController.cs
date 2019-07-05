using UnityEngine;
using System.Collections;

public class EditorController : MonoBehaviour {

    private uiController userInterface;

	// Use this for initialization
	void Start () {
        userInterface = new uiController();
        userInterface.init();
	}
	
	// Update is called once per frame
	void Update () {
        userInterface.tick();
	}
}
