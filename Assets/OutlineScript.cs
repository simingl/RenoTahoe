using UnityEngine;
using System.Collections;

public class OutlineScript : MonoBehaviour {
	public Shader shader1;
    public Shader shader2;
	public float outlineSize = 0.01f;
	public Color innerColor = Color.yellow;
	public Color outlineColor = Color.black;
	private bool alreadyNear = false;
	
	// Use this for initialization
	void Start () {
		shader1 = Shader.Find("Diffuse");
    	shader2 = Shader.Find("Outlined/Silhouetted Diffuse");
	}
	
	// Update is called once per frame
	void Update () {
		if (true) {
			if (true) {
				alreadyNear = true;
				GetComponent<Renderer>().material.shader = shader2;

				GetComponent<Renderer>().material.SetFloat("_Outline", outlineSize);
				GetComponent<Renderer>().material.SetColor("_Color", innerColor);
				GetComponent<Renderer>().material.SetColor("_OutlineColor", outlineColor);

			}
		} else {
			alreadyNear = false;
			GetComponent<Renderer>().material.shader = shader1;
		}
	}
}
