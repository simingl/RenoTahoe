using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Player : MonoBehaviour {
	public string username;
	public bool human = true;
	public HUD hud;

	public List <WorldObject> selectedObjects;

	// Use this for initialization
	void Start () {
		hud = GetComponentInChildren< HUD >();
		selectedObjects = new List<WorldObject> ();
	}


	public void addSelectedObject(WorldObject obj){
		obj.SetSelection(true);
		if(!selectedObjects.Contains(obj))
			selectedObjects.Add (obj);
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