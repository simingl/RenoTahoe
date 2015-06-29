using UnityEngine;
using System.Collections;
using RTS;

public class WorldObject : MonoBehaviour {
	public WorldObjectType type;

	public string objectName;

	protected Player player;
	protected string[] actions = {};

	protected Bounds selectionBounds;

	protected Rect playingArea = new Rect(0.0f, 0.0f, 0.0f, 0.0f);

	protected virtual void Awake() {
		selectionBounds = ResourceManager.InvalidBounds;
		CalculateBounds();
	}
	
	protected virtual void Start () {
		player = GameObject.FindGameObjectWithTag ("Player").GetComponent<Player>();
//		player = transform.root.GetComponentInChildren< Player >();
		this.playingArea = player.hud.GetPlayingArea ();
	}
	
	protected virtual void Update () {


	}
	
	protected virtual void OnGUI() {
		if (this.isSelected() && this.player.changePOV.activeCamera == null) {
			DrawSelection ();
		}
	}

	public string[] GetActions() {
		return actions;
	}
	
	public virtual void PerformAction(string actionToPerform) {
	}

	public virtual void MouseClick(GameObject hitObject, Vector3 hitPoint, Player controller) {
		//only handle input if currently selected
		if(this.isSelected() && hitObject && hitObject.name != "Ground") {
			WorldObject worldObject = hitObject.GetComponent< WorldObject >();
			//clicked on another selectable object
			if(worldObject) ChangeSelection(worldObject, controller);
		}
	}

	private void ChangeSelection(WorldObject worldObject, Player controller) {
		controller.cleanSelectedObject();
		controller.addSelectedObject(worldObject);
	}
	public void DrawSelection() {
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

	public bool isSelected(){
		return player.isSelected (this);
	}
}
