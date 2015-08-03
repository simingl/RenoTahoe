using UnityEngine;
using System.Collections;

public class Vehicle : WorldObject {
	private GameObject mark;

	protected virtual void Start () {
		base.Start ();
		
		this._isSelectable = false;

		mark = GameObject.CreatePrimitive (PrimitiveType.Cube);
		mark.layer = gameObject.layer;
		mark.GetComponent<Collider> ().enabled = false;
		
		mark.transform.parent = transform;
		mark.transform.localScale = Vector3.one * 3f;
		mark.transform.localPosition = new Vector3 (0, 5f, 0);
		mark.transform.rotation = gameObject.transform.rotation;
		mark.GetComponent<Renderer> ().material.color = Color.red;
	}
	public void setColor(Color color){
		mark.GetComponent<Renderer> ().material.color = color;
	}
	
	public void setRed(){

	}
}
