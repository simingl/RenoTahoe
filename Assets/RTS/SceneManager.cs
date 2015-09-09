using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SceneManager : MonoBehaviour {
	private const int MAX_DRONE = 16;

	public GameObject ground;
	public GameObject tree;
	public GameObject fire;
	//public GameObject cellphone;
	//public GameObject water;
	public GameObject drone;
	public GameObject helicopter;

	private List<WorldObject> allEntities = new List<WorldObject> ();

	private int number = 5;

	private Vector3[] treePoints;
	private Vector3[] firePoints;
	private Vector3[] cellPoints;
	private Vector3[] heliPoints;

	private int width = 80, height = 80;
	private Transform container;

	private Player player;
	void Awake(){
		player = GameObject.FindGameObjectWithTag ("Player").GetComponent<Player>();
		container = gameObject.transform.FindChild ("Helicopters");
	}

	void Start(){
		Random.seed = 1;

		treePoints = new Vector3[number];
		firePoints = new Vector3[number];
		cellPoints = new Vector3[number];
		heliPoints = new Vector3[number];

		for (int i= 0;i<number;i++) {
			treePoints[i] = generateRandomPosition();
			firePoints[i] = generateRandomPosition();
			cellPoints[i] = generateRandomPosition();
			heliPoints[i]= generateRandomPosition();
		}
		InitialScene ();
	}

	void InitialScene (){
		return;
		for (int i = 0; i< number;i++) {
			//Instantiate (tree, treePoints[i], new Quaternion(0,0,0,0));
			//GameObject newfire = (GameObject)Instantiate (fire, firePoints[i], new Quaternion(1,1,0,1));
			GameObject heligo = (GameObject)Instantiate (helicopter, heliPoints[i], new Quaternion(0,0,0,1));
			heligo.transform.parent = container;
			WorldObject heli = heligo.GetComponent<WorldObject>();
			this.allEntities.Add(heli);
		}
	}


	public Vector3 generateRandomPosition(){
		float x = Random.Range(-1*width, height);
		float z = Random.Range(-1*width, height);

		return new Vector3 (x,4,z);
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

	public WaterBottle getFreeWaterBottle(Vector3 position, float range){
		WaterBottle minObj = null;
		float minDist = 100000;
		foreach (WorldObject obj in allEntities) {
			if(obj is WaterBottle){
				WaterBottle cell = (WaterBottle)obj;
				if(cell.parent != null) continue;
				
				float dist = Vector3.Distance(position, obj.gameObject.transform.position);
				if(dist <= range && dist < minDist){
					minDist = dist;
					minObj = (WaterBottle)obj;
				}
			}
		}
		return minObj;
	}

	public Drone[] getAllDrones(){
		Player player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
		return player.GetComponentsInChildren<Drone> ();
	}

	public void CreateDrone(Vector3 position){
		Drone[] drones = this.getAllDrones ();
		if (drones.Length >= MAX_DRONE) {
			return;
		}

		GameObject newdrone = (GameObject)Instantiate (drone, position, new Quaternion(0,0,0,1));
		newdrone.transform.parent = this.player.transform;
		newdrone.name = "Drone-" + drones.Length;
	}
}
