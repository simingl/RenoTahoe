﻿using UnityEngine;
using System.Collections;
using RTS;

public class WorldObject : MonoBehaviour {
	public WorldObjectType type;

	public string objectName;
	public int cost, hitPoints, maxHitPoints;

	protected Player player;
	protected string[] actions = {};
	protected bool currentlySelected = false;

	protected Bounds selectionBounds;

	protected Rect playingArea = new Rect(0.0f, 0.0f, 0.0f, 0.0f);

	protected virtual void Awake() {
		selectionBounds = ResourceManager.InvalidBounds;
		CalculateBounds();
	}
	
	protected virtual void Start () {
		player = transform.root.GetComponentInChildren< Player >();
	}
	
	protected virtual void Update () {


	}
	
	protected virtual void OnGUI() {
		if(currentlySelected) DrawSelection();
	}

	public void SetSelection(bool selected, Rect playingArea) {
		currentlySelected = selected;
		if(selected) this.playingArea = playingArea;
	}

	public string[] GetActions() {
		return actions;
	}
	
	public virtual void PerformAction(string actionToPerform) {
		//it is up to children with specific actions to determine what to do with each of those actions
	}

	public virtual void MouseClick(GameObject hitObject, Vector3 hitPoint, Player controller) {
		//only handle input if currently selected
		if(currentlySelected && hitObject && hitObject.name != "Ground") {
			WorldObject worldObject = hitObject.GetComponent< WorldObject >();
			//clicked on another selectable object
			if(worldObject) ChangeSelection(worldObject, controller);
		}
	}

	private void ChangeSelection(WorldObject worldObject, Player controller) {
		//this should be called by the following line, but there is an outside chance it will not
		SetSelection(false, playingArea);
		if(controller.SelectedObject) controller.SelectedObject.SetSelection(false, playingArea);
		controller.SelectedObject = worldObject;
		worldObject.SetSelection(true, controller.hud.GetPlayingArea());
	}
	private void DrawSelection() {
		GUI.skin = ResourceManager.SelectBoxSkin;
		Rect selectBox = WorkManager.CalculateSelectionBox(selectionBounds, playingArea);
		//Draw the selection box around the currently selected object, within the bounds of the playing area
		GUI.BeginGroup(playingArea);
		DrawSelectionBox(selectBox);
		GUI.EndGroup();
	}

	public void CalculateBounds() {
		selectionBounds = new Bounds(transform.position, Vector3.zero);
		Renderer x = GetComponent<MeshRenderer> ();

		foreach(Renderer r in GetComponentsInChildren< Renderer >()) {
			if(r.GetType().Name == "MeshRenderer" && r.ToString().IndexOf("group_top")>=0){
				selectionBounds.Encapsulate(r.bounds);
			}
		}
	}

	protected virtual void DrawSelectionBox(Rect selectBox) {
		GUI.Box(selectBox, "");
	}

	public virtual void SetHoverState(GameObject hoverObject) {
		//only handle input if owned by a human player and currently selected
		if(player && player.human && currentlySelected) {
			if(hoverObject.name != "Ground") player.hud.SetCursorState(CursorState.Select);
		}
	}


}
