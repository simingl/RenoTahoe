using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RTS;

public class SceneManager : MonoBehaviour {
	private const int MAX_DRONE = 16;

	public GameObject droneModel;
	public GameObject helicopterModel;
	public GameObject humanModel;
	public GameObject carModel;

	private List<WorldObject> allHelicopters = new List<WorldObject> ();
	private List<WorldObject> allPeople = new List<WorldObject> ();
	private List<WorldObject> allCars = new List<WorldObject> ();

	private Player player;
	private ConfigManager configManager;
	private int sceneDroneCount = 1;
	private int sceneHelicopterCount = 1;
	private int scenePeopleCount = 1;
	private int sceneCarCount = 1;

	void Awake(){
		player = GameObject.FindGameObjectWithTag ("Player").GetComponent<Player>();
		configManager = ConfigManager.getInstance ();


	}

	void Start(){
		Random.seed = 1;
		InitialScene ();
	}

	void InitialScene (){
		initialDroneSpawnLocation ();
		initialHelicopterSpawnLocation ();
		initialPeopleSpawnLocation ();
		initialCarSpawnLocation ();
	}

	private void initialDroneSpawnLocation(){
		sceneDroneCount = configManager.getSceneDroneCount ();
		GameObject droneSpawnLocation = GameObject.FindGameObjectWithTag("DroneSpawnLocation");
		int row = Mathf.RoundToInt(Mathf.Sqrt(sceneDroneCount));
		int col = Mathf.CeilToInt((float)sceneDroneCount / row);
		Vector3 center = droneSpawnLocation.transform.position;
		center.y += 0.1f;
		float droneWidth = 2.5f;
		Vector3 start = new Vector3(center.x - row/2 * droneWidth, center.y, center.z + col/2* droneWidth);
		for (int i=0; i< row; i++) {
			for(int j=0;j< col;j++){
				if(this.getAllDrones ().Length == sceneDroneCount) return;
				Vector3 pos = start;
				pos.x = start.x + j*droneWidth;
				pos.z = start.z - i*droneWidth;
				this.CreateDrone(pos);
			}
		}
	}

	private void initialHelicopterSpawnLocation(){
		Transform sceneHelicopterContainer = gameObject.transform.FindChild ("Helicopters");
		this.sceneHelicopterCount = configManager.getSceneHelicopterCount ();
		GameObject[] heliSpawnLocation = GameObject.FindGameObjectsWithTag("HelicopterSpawnLocation");
		int spawnIndex = Random.Range (0, heliSpawnLocation.Length - 1);;
		for (int i=0; i< sceneHelicopterCount; i++) {
			if(spawnIndex >= heliSpawnLocation.Length) spawnIndex = 0;
			GameObject heligo = (GameObject)Instantiate (helicopterModel, heliSpawnLocation[spawnIndex++].transform.position, new Quaternion(0,0,0,1));
			heligo.SetActive(true);
			heligo.transform.parent = sceneHelicopterContainer;
			WorldObject heli = heligo.GetComponent<WorldObject>();
			this.allHelicopters.Add(heli);
		}
	}

	private void initialPeopleSpawnLocation(){
		Transform scenePeopleContainer = gameObject.transform.FindChild ("People");
		this.scenePeopleCount = configManager.getScenePeopleCount ();
		GameObject[] peopleSpawnLocations = GameObject.FindGameObjectsWithTag("Waypoint");
		int spawnIndex = Random.Range (0, peopleSpawnLocations.Length - 1);;
		for (int i=0; i< scenePeopleCount; i++) {
			if(spawnIndex >= peopleSpawnLocations.Length) spawnIndex = 0;
			GameObject peoplego = (GameObject)Instantiate (humanModel, peopleSpawnLocations[spawnIndex++].transform.position, new Quaternion(0,0,0,1));
			peoplego.SetActive(true);
			peoplego.transform.parent = scenePeopleContainer;
			WorldObject people = peoplego.GetComponent<WorldObject>();
			this.allPeople.Add(people);
		}
	}

	private void initialCarSpawnLocation(){
		Transform sceneCarContainer = gameObject.transform.FindChild ("Cars");
		this.sceneCarCount = configManager.getSceneCarCount ();
		GameObject[] carSpawnLocations = GameObject.FindGameObjectsWithTag("Waypoint");
		int spawnIndex = Random.Range (0, carSpawnLocations.Length - 1);;
		for (int i=0; i< sceneCarCount; i++) {
			if(spawnIndex >= carSpawnLocations.Length) spawnIndex = 0;
			GameObject cargo = (GameObject)Instantiate (carModel, carSpawnLocations[spawnIndex++].transform.position, new Quaternion(0,0,0,1));
			cargo.SetActive(true);
			cargo.transform.parent = sceneCarContainer;
			WorldObject car = cargo.GetComponent<WorldObject>();
			this.allCars.Add(car);
		}
	}

	public List<WorldObject> getWorldObjects(Vector3 position, float range){
		List<WorldObject> result = new List<WorldObject> ();
		foreach (WorldObject obj in allHelicopters) {
			if(Vector3.Distance(position, obj.gameObject.transform.position) <= range){
				result.Add(obj);
			}
		}
		return result;
	}
	public WorldObject getWorldObject(Vector3 position, float range){
		WorldObject minObj = null;
		float minDist = 100000;
		foreach (WorldObject obj in allHelicopters) {
			float dist = Vector3.Distance(position, obj.gameObject.transform.position);
			if(dist <= range && dist < minDist){
				minDist = dist;
				minObj = obj;
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

		GameObject newdrone = (GameObject)Instantiate (droneModel, position, new Quaternion(0,0,0,1));
		newdrone.transform.parent = this.player.transform;
		newdrone.name = "Drone-" + drones.Length;
		newdrone.SetActive (true);
	}
}
