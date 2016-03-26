using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using RTS;
using UnityStandardAssets.ImageEffects;

public class Player : MonoBehaviour {
	public string username;
	public bool human = true;
	public HUD hud;

	public List <WorldObject> selectedObjects;

	public AudioManager audioManager;
	public ChangePOV changePOV;
	public SceneManager sceneManager;
	public StationManager stationManager;
	public ConfigManager configManager;

	// Use this for initialization
	void Start () {
		hud = GetComponentInChildren< HUD >();
		selectedObjects = new List<WorldObject> ();
		audioManager = this.GetComponent<AudioManager> ();
		changePOV = this.GetComponent<ChangePOV> ();
		sceneManager = GameObject.FindObjectOfType<SceneManager> ();
		stationManager = GameObject.FindObjectOfType<StationManager> ();

		configManager = ConfigManager.getInstance();
	}

	public void addSelectedObject(WorldObject obj){
		if (!selectedObjects.Contains (obj)) {
			selectedObjects.Add (obj);
			this.audioManager.playUnitSelectSound();
		}

		if (obj == this.selectedObjects [0] && obj is Drone) {
			Drone drone = (Drone)obj;
			drone.showPIPCameraFront();
		}
	}

	public void setSelectedObject(WorldObject obj){

		this.cleanSelectedObject ();
		this.addSelectedObject (obj);
	}

	public void toggleSelectObject(WorldObject obj){
		if (selectedObjects.Contains (obj)) {
			this.removeSelectedObject (obj);
		} else {
			this.addSelectedObject(obj);
		}
	}
	public void removeSelectedObject(WorldObject obj){
		selectedObjects.Remove (obj);
		if (obj is Drone) {
			((Drone)obj).Unselect();
		}
	}

	public void cleanSelectedObject(){
		foreach(WorldObject selectedObj in selectedObjects){
			if(selectedObj is Drone){
				((Drone)selectedObj).Unselect();
			}
		}
		selectedObjects.Clear ();
	}

	public List<WorldObject> getSelectedObjects(){
		return new List<WorldObject>( selectedObjects);
	}

	public Vector3 getOffsetFromCenterOfSelectedObjects(Vector3 objPos){
		Vector3 center = this.getCenterOfSelectedObjects ();
		if (center == ResourceManager.InvalidPosition)
			return ResourceManager.InvalidPosition;

		return (objPos - center);
	}

	private Vector3 getCenterOfSelectedObjects(){
		if (selectedObjects.Count <= 0)
			return ResourceManager.InvalidPosition;

		float sumX = 0f;
		float sumZ = 0f;
		foreach(WorldObject selectedObj in selectedObjects){
			sumX +=selectedObj.transform.position.x;
			sumZ +=selectedObj.transform.position.z;
		}
		int size = this.selectedObjects.Count;
		return new Vector3 (sumX / size, 0, sumZ / size);
	}

	public bool isSelected(WorldObject obj){
		return selectedObjects.Contains (obj);
	}

	public bool isPIPActive(){
		return this.changePOV.activeCamera == null && this.getSelectedObjects ().Count > 0;
	}
}