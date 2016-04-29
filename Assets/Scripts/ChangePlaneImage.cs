using UnityEngine;
using System.Collections;

public class ChangePlaneImage : MonoBehaviour {

	public GameObject P;
	public string imageName;
	Material m;


	// Use this for initialization
	void Start () {
		m = P.GetComponent<Material> ();
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.C)) {
			changeTexture ();
		}
	}

	void changeTexture(){
		Texture _texture;

		_texture = Resources.Load(imageName) as Texture;
		P.GetComponent<Renderer>().material.mainTexture = _texture;

	}
}
