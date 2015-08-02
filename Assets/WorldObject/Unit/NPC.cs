using UnityEngine;
using System.Collections;

public class NPC : WorldObject {

	private Renderer rend;
	public Texture red;
	public Texture green;

	public GameObject marker;
	//private GameObject mark;

	protected virtual void Start () {
		base.Start ();
		marker = transform.Find ("NPCMarker").gameObject;
		//rend = transform.FindChild("EthanBody").GetComponent<Renderer>();
		//rend.material.SetTexture (0,red);
		/*
		mark = GameObject.CreatePrimitive (PrimitiveType.Cube);
		mark.layer = gameObject.layer;
		mark.GetComponent<Collider> ().enabled = false;
		
		mark.transform.parent = transform;
		mark.transform.localScale = Vector3.one * 0.2f;
		mark.transform.localPosition = new Vector3 (0, 1.8f, 0);
		mark.transform.rotation = gameObject.transform.rotation;
		mark.GetComponent<Renderer> ().material.color = Color.red;
	}
	public void setColor(Color color){
		//rend.material.SetTexture (0,green);
		mark.GetComponent<Renderer> ().material.color = color;
	}

	public void setRed(){
		//rend.material.SetTexture (0,red);;
	}
	*/

}
	public void setMarker ()
	{
		marker.SetActive (true);
	}
	
	public void disableMarker ()
	{
		marker.SetActive (false);
	}
}
