using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using RTS;

public class Unit : WorldObject {
	public Image batteryBarImage;

	protected bool moving, rotating;
	public float moveSpeed, rotateSpeed;

	private Vector3 destination;
	private Quaternion targetRotation;

	private LineRenderer lineRaycast;
	private GameObject lineMoveContainer;
	private LineRenderer lineMove;

	private GameObject destinationMark;

	/*** Game Engine methods, all can be overridden by subclass ***/

	public Unit(){
		type = WorldObjectType.Unit;
	}

	protected override void Awake() {
		base.Awake();
	}
	
	protected override void Start () {
		base.Start();

		destinationMark = GameObject.FindGameObjectWithTag ("DestinationMark");
		//destinationMark.SetActive (false);
		//setup the line from object to the ground
		lineRaycast = this.GetComponent<LineRenderer> ();
		lineRaycast.useWorldSpace = true;

		//setup the line from object to the destination
		lineMoveContainer = new GameObject ("LineMoveTo");
		lineMoveContainer.AddComponent<LineRenderer> ();
		lineMove = lineMoveContainer.GetComponent<LineRenderer> ();
		lineMove.useWorldSpace = true;
		lineMove.materials = lineRaycast.materials;
		lineMove.SetColors (Color.green, Color.green);
		lineMove.SetWidth (0.3f,0.3f);
	}
	
	protected override void Update () {
		base.Update();

		this.drawRaycastLine ();

		if(rotating) TurnToTarget();
		else if(moving) MakeMove();


	}
	
	protected override void OnGUI() {
		base.OnGUI();
		Debug.DrawLine(Vector3.zero, new Vector3(0f, 50f, 5f), Color.red);
	}

	public override void SetHoverState(GameObject hoverObject) {
		base.SetHoverState(hoverObject);
		//only handle input if owned by a human player and currently selected
		if(player && player.human && currentlySelected) {
			if(hoverObject.name == "Ground") player.hud.SetCursorState(CursorState.Move);
		}
	}

	public override void MouseClick(GameObject hitObject, Vector3 hitPoint, Player controller) {
		base.MouseClick(hitObject, hitPoint, controller);
		//only handle input if owned by a human player and currently selected
		if(player && player.human && currentlySelected && Input.GetMouseButton(1)) {
			if(hitObject.name == "Ground" && hitPoint != ResourceManager.InvalidPosition) {
				float x = hitPoint.x;
				//makes sure that the unit stays on top of the surface it is on
				float y = hitPoint.y + player.SelectedObject.transform.position.y;
				float z = hitPoint.z;
				Vector3 destination = new Vector3(x, y, z);
				StartMove(destination);
			}
		}
	}

	public void StartMove(Vector3 destination) {
		this.destination = destination;
		targetRotation = Quaternion.LookRotation (destination - transform.position);
		rotating = true;
		moving = false;
	}

	protected override void DrawSelectionBox(Rect rect){
		base.DrawSelectionBox (rect);
		this.drawBatteryBar (rect);
	}
	private void TurnToTarget() {
		transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotateSpeed);
		//sometimes it gets stuck exactly 180 degrees out in the calculation and does nothing, this check fixes that
		Quaternion inverseTargetRotation = new Quaternion(-targetRotation.x, -targetRotation.y, -targetRotation.z, -targetRotation.w);
		if(transform.rotation == targetRotation || transform.rotation == inverseTargetRotation) {
			rotating = false;
			moving = true;
		}
		CalculateBounds();
	}

	private void MakeMove() {
		transform.position = Vector3.MoveTowards(transform.position, destination, Time.deltaTime * moveSpeed);
		if(transform.position == destination) moving = false;
		CalculateBounds();
	}

	private void drawRaycastLine(){
		Vector3 groundHit = new Vector3 (transform.position.x, 0, transform.position.z);
		lineRaycast.SetPosition (0, transform.position);
		lineRaycast.SetPosition (1, groundHit);

		if (rotating || moving) {
			lineMove.enabled = true;
			Vector3 targetGroundHit = new Vector3 (destination.x, 1, destination.z);
			lineMove.SetPosition (0, transform.position);
			lineMove.SetPosition (1, targetGroundHit);

			destinationMark.SetActive(true);
			destinationMark.transform.position = targetGroundHit;
		} else {
			lineMove.enabled = false;
			destinationMark.SetActive(false);

		}
	}

	private void drawBatteryBar(Rect rect){
		Vector3 pos = Camera.main.WorldToScreenPoint (transform.position);
		batteryBarImage.transform.position = new Vector3(pos.x,pos.y+rect.height/2,0);
	}
}