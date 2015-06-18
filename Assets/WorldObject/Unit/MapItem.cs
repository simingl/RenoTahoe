﻿using UnityEngine;
using System.Collections;

public class MapItem : MonoBehaviour {
	public GameObject drone_map;

	Renderer rend;
	// Use this for initialization
	void Start () {
		rend = this.GetComponent<Renderer> ();

		GameObject mapBounds = GameObject.Instantiate (drone_map);

		//GameObject mapBounds = GameObject.CreatePrimitive (PrimitiveType.Cube);
		mapBounds.name = "MapBounds";

		this.SetLayerRecursively (mapBounds, 8);
		//mapBounds.layer = 8;
		//mapBounds.GetComponent<Collider> ().enabled = false;
		mapBounds.transform.parent = transform;
		mapBounds.transform.localScale = Vector3.one*20;
		mapBounds.transform.rotation = transform.rotation;
		mapBounds.transform.RotateAround(mapBounds.transform.position, Vector3.forward, 90);
		mapBounds.transform.RotateAround(mapBounds.transform.position, Vector3.left, 90);
		//mapBounds.transform.localRotation = new Quaternion (0,0,0,1);
		mapBounds.transform.localPosition = new Vector3(0,0,0);
		Renderer rend_mapBounds = mapBounds.GetComponent<Renderer>();
		//rend_mapBounds.material.color = rend.material.color;
	}

	private void SetLayerRecursively(GameObject obj,  int newLayer )
	{
		obj.layer = newLayer;
		
		foreach (Transform child in obj.transform )
		{
			SetLayerRecursively( child.gameObject, newLayer );
		}
	}
}
