using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Player : MonoBehaviour {
	public string username;
	public bool human = true;
	public HUD hud;

	public List <WorldObject> selectedObjects;

	public AudioManager audioManager;
	public ChangePOV changePOV;
	public SceneManager sceneManager;

	// Use this for initialization
	void Start () {
		hud = GetComponentInChildren< HUD >();
		selectedObjects = new List<WorldObject> ();
		audioManager = this.GetComponent<AudioManager> ();
		changePOV = this.GetComponent<ChangePOV> ();
		sceneManager = GameObject.FindObjectOfType<SceneManager> ();
	}

	public void addSelectedObject(WorldObject obj){
		if (!selectedObjects.Contains (obj)) {
			selectedObjects.Add (obj);
			this.audioManager.playUnitSelectSound();
		}

		if (obj == this.selectedObjects [0] && obj is Drone) {
			Drone drone = (Drone)obj;
			drone.SetPIPCameraActive(true);
			drone.showPIP(0);
		}
	}

	public void setSelectedObject(WorldObject obj){
		foreach(WorldObject selectedObj in selectedObjects){
			if(selectedObj is Drone){
				((Drone)selectedObj).SetPIPCameraActive(false);
			}
		}
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
			((Drone)obj).SetPIPCameraActive(false);
		}
	}

	public void cleanSelectedObject(){
		selectedObjects.Clear ();
	}

	public List<WorldObject> getSelectedObjects(){
		return new List<WorldObject>( selectedObjects);
	}

	public bool isSelected(WorldObject obj){
		return selectedObjects.Contains (obj);
	}

	public bool isPIPActive(){
		return this.changePOV.activeCamera == null && this.getSelectedObjects ().Count > 0;
	}
}