using UnityEngine;
using System.Collections;

public class FindIndicator : MonoBehaviour {

    public Camera mainCamera;
    RaycastHit hit;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5F, 0.5F, 0));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
            print("I'm looking at " + hit.transform.name);
        else
            print("I'm looking at nothing!");
	}
}
