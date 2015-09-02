using UnityEngine;
using System.Collections;
using RTS;

public class Vehicle : WorldObject {
	private GameObject mark;

	override protected void Start () {
		base.Start ();
		
		this._isSelectable = false;

		this.scoreValue = 200;

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
	
	public void Mark(){
		mark.GetComponent<Renderer> ().material.color = Color.green;
		SetLayerRecursively (gameObject, gameObject.layer, ResourceManager.LayerEntitiesCommon);
		ScoreManager.score += this.scoreValue;
	}


}
