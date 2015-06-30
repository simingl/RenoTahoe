﻿using UnityEngine;
using System.Collections;

public class MapItem : MonoBehaviour {
	public GameObject drone_map;

	private WorldObject worldObj;
	private GameObject mapBounds;

	private const int MINIMAP_LAYER = 8;
	// Use this for initialization
	void Start () {
		worldObj = this.GetComponent<WorldObject> ();
		if (worldObj is Drone) {
			Drone drone = (Drone)worldObj;
			mapBounds = GameObject.Instantiate (drone_map);
			this.SetLayerRecursively (mapBounds, MINIMAP_LAYER);

			mapBounds.transform.parent = transform;
			mapBounds.transform.localScale = Vector3.one*15;
			mapBounds.transform.rotation = transform.rotation;
			
			Vector3 forwardaxis = transform.TransformDirection(Vector3.forward);
			Vector3 leftaxis = transform.TransformDirection(Vector3.left);
			
			mapBounds.transform.RotateAround(mapBounds.transform.position, forwardaxis, 90);
			mapBounds.transform.RotateAround(mapBounds.transform.position, leftaxis, 90);
			
			mapBounds.transform.localPosition = new Vector3(0,50,0);
			
			Transform childtr = mapBounds.transform.GetChild(0);
			MeshRenderer rend_mapBounds = childtr.gameObject.GetComponent<MeshRenderer>();
			
			rend_mapBounds.material.color = drone.color;

		} else if (worldObj is NPC) {
			mapBounds = GameObject.CreatePrimitive (PrimitiveType.Cube);
			mapBounds.layer = MINIMAP_LAYER;
			mapBounds.GetComponent<Collider> ().enabled = false;

			mapBounds.transform.parent = transform;
			mapBounds.transform.localScale = Vector3.one*5;
			mapBounds.transform.rotation = transform.rotation;
			
			Vector3 forwardaxis = transform.TransformDirection(Vector3.forward);
			Vector3 leftaxis = transform.TransformDirection(Vector3.left);
			
			mapBounds.transform.RotateAround(mapBounds.transform.position, forwardaxis, 90);
			mapBounds.transform.RotateAround(mapBounds.transform.position, leftaxis, 90);
			
			mapBounds.transform.localPosition = new Vector3(0,0,0);
			mapBounds.GetComponent<Renderer>().material.color = Color.red;
		}
		mapBounds.name = "MiniMapIcon";
	}

	void Awake(){

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
