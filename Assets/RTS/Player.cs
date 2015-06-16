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
		obj.SetSelection(true);
		if (!selectedObjects.Contains (obj)) {
			selectedObjects.Add (obj);
			this.audioManager.playUnitSelectSound();
		}
	}

	public void setSelectedObject(WorldObject obj){
		this.cleanSelectedObject ();

		obj.SetSelection(true);
		if (!selectedObjects.Contains (obj)) {
			selectedObjects.Add (obj);
			this.audioManager.playUnitSelectSound();
		}
	}

	public void toggleSelectObject(WorldObject obj){
		if (selectedObjects.Contains (obj)) {
			this.removeSelectedObject (obj);
		} else {
			this.addSelectedObject(obj);
		}
	}
	public void removeSelectedObject(WorldObject obj){
		obj.SetSelection(false);
		selectedObjects.Remove (obj);
	}

	public void cleanSelectedObject(){
		foreach (WorldObject obj in selectedObjects) {
			obj.SetSelection(false);
		}
		selectedObjects.Clear ();
	}

	public List<WorldObject> getSelectedObjects(){
		return selectedObjects;
	}

	public bool isSelected(WorldObject obj){
		return selectedObjects.Contains (obj);
	}

	// Update is called once per frame
	void Update () {
		
	}
}