using UnityEngine;
using System.Collections;
using RTS;

public class WorldObject : MonoBehaviour {
	public WorldObjectType type;

	public string objectName;
	public int cost, hitPoints, maxHitPoints;

	protected Player player;
	protected string[] actions = {};
	public bool currentlySelected = false;

	protected Bounds selectionBounds;

	protected Rect playingArea = new Rect(0.0f, 0.0f, 0.0f, 0.0f);

	protected virtual void Awake() {
		selectionBounds = ResourceManager.InvalidBounds;
		CalculateBounds();
	}
	
	protected virtual void Start () {
		player = transform.root.GetComponentInChildren< Player >();
		this.playingArea = player.hud.GetPlayingArea ();
	}
	
	protected virtual void Update () {


	}
	
	protected virtual void OnGUI() {
		if (currentlySelected && Camera.main) {
			DrawSelection ();
		}
	}

	public void SetSelection(bool selected) {
		currentlySelected = selected;
	}

	public string[] GetActions() {
		return actions;
	}
	
	public virtual void PerformAction(string actionToPerform) {
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
		controller.cleanSelectedObject();
		controller.addSelectedObject(worldObject);
	}
	private void DrawSelection() {
		GUI.skin = ResourceManager.SelectBoxSkin;
		Rect selectBox = WorkManager.CalculateSelectionBox(selectionBounds, playingArea);
		GUI.BeginGroup(playingArea);
		DrawSelectionBox(selectBox);
		GUI.EndGroup();


	}

	public void CalculateBounds() {
		selectionBounds = new Bounds(transform.position, Vector3.zero);

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
