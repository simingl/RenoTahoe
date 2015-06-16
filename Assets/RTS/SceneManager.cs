﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SceneManager : MonoBehaviour {
	public GameObject tree;
	public GameObject fire;
	public GameObject cellphone;

	private List<WorldObject> allEntities = new List<WorldObject> ();

	private int number = 3;

	private Vector3[] treePoints;
	private Vector3[] firePoints;
	private Vector3[] cellPoints;

	private int width = 10, height = 10;

	void Start(){
		Random.seed = 1;

		treePoints = new Vector3[number];
		firePoints = new Vector3[number];
		cellPoints = new Vector3[number];

		for (int i= 0;i<number;i++) {
			//treePoints[i] = generateRandomPosition(width, height);
			firePoints[i] = generateRandomPosition(width, height);
			cellPoints[i] = generateRandomPosition(width, height);
		}

		InitialScene ();
	}

	void InitialScene (){
		for (int i = 0; i< number;i++) {
			//Instantiate (tree, treePoints[i], new Quaternion(0,0,0,0));
			//GameObject newfire = (GameObject)Instantiate (fire, firePoints[i], new Quaternion(1,1,0,1));
			GameObject newcellgo = (GameObject)Instantiate (cellphone, cellPoints[i], new Quaternion(0,0,0,1));
			Renderer r = newcellgo.GetComponent<Renderer>();
			r.material.color = Color.red;
			WorldObject newcell = newcellgo.GetComponent<WorldObject>();
			this.allEntities.Add(newcell);
		}
	}

	private Vector3 generateRandomPosition(int w, int h){
		float scale = 1.5f;
		float x = Random.Range(-1*width, height);
		float z = Random.Range(-1*width, height);

		return new Vector3 (x,1,z);
	}

	public List<WorldObject> getWorldObjects(Vector3 position, float range){
		List<WorldObject> result = new List<WorldObject> ();
		foreach (WorldObject obj in allEntities) {
			if(Vector3.Distance(position, obj.gameObject.transform.position) <= range){
				result.Add(obj);
			}
		}
		return result;
	}
	public WorldObject getWorldObject(Vector3 position, float range){
		WorldObject minObj = null;
		float minDist = 100000;
		foreach (WorldObject obj in allEntities) {
			float dist = Vector3.Distance(position, obj.gameObject.transform.position);
			if(dist <= range && dist < minDist){
				minDist = dist;
				minObj = obj;
			}
		}
		return minObj;
	}

	public Cellphone getFreeCellphone(Vector3 position, float range){
		Cellphone minObj = null;
		float minDist = 100000;
		foreach (WorldObject obj in allEntities) {
			if(obj is Cellphone){
				Cellphone cell = (Cellphone)obj;
				if(cell.parent != null) continue;

				float dist = Vector3.Distance(position, obj.gameObject.transform.position);
				if(dist <= range && dist < minDist){
					minDist = dist;
					minObj = (Cellphone)obj;
				}
			}
		}
		return minObj;
	}

	public Drone[] getAllDrones(){
		Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
		return player.GetComponentsInChildren<Drone> ();
	}
}
