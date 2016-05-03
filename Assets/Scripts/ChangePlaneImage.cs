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
            changeTextureFromResources();
		}
        if (Input.GetKeyDown(KeyCode.X))
        {
           StartCoroutine( changeTextureFromServer());
        }
	}

    void changeTextureFromResources()
    {
		Texture _texture;

		_texture = Resources.Load(imageName) as Texture;
		P.GetComponent<Renderer>().material.mainTexture = _texture;

	}

    IEnumerator changeTextureFromServer()
    {
        Texture2D _texture = new Texture2D(4, 4, TextureFormat.DXT1, false);

        //_texture = Resources.Load(imageName) as Texture;
        
        print("Load image in = " + imageName);
        
        while (true)
        {
            // Start a download of the given URL
            var www = new WWW(imageName);

            // wait until the download is done
            yield return www;

            // assign the downloaded image to the main texture of the object
            www.LoadImageIntoTexture(_texture);
            P.GetComponent<Renderer>().material.mainTexture = _texture;
            break;
        }

    }
}
