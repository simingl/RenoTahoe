using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using RTS;

public class Unit : WorldObject {
	public Image batteryBarImage;
	public Canvas canvas;
	private Image battery;

	protected bool moving, rotating;
	public float moveSpeed, rotateSpeed;

	private Vector3 destination;
	private Quaternion targetRotation;

	private LineRenderer lineRaycast;
	private GameObject lineMoveContainer;
	private LineRenderer lineMove;

	private Transform destinationMark;

	public float startBattery = 100;
	public float currentBattery = 100;
	public float batteryUsage = 0.1f;

	private bool selectedByClick = false;

	public Unit(){
		type = WorldObjectType.Unit;
	}



	protected override void Awake() {
		base.Awake();

		//Create a battery bar from the prefab
		battery = (Image)GameObject.Instantiate(batteryBarImage, transform.position, transform.localRotation);
		battery.transform.SetParent (canvas.transform);
		battery.transform.localScale = Vector3.one;
		battery.enabled = false;
	}

	protected override void Start () {
		base.Start();

		//setup the destination mark
		destinationMark = this.transform.FindChild("DestinationMark");
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

		this.CalculateBattery ();



		if (Input.GetMouseButton (0)) {
			if(!selectedByClick){
				Vector3 camPos = Camera.main.WorldToScreenPoint(transform.position);
				camPos.y = CameraManagement.InvertMouseY(camPos.y);
				//this.currentlySelected = HUD.selection.Contains(camPos);
				if(HUD.selection.Contains(camPos)){
					this.player.addSelectedObject(this);
				}
			}
		}
	}
	
	protected override void OnGUI() {
		base.OnGUI();
		battery.enabled = currentlySelected;
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
				float y = transform.position.y;
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

			destinationMark.gameObject.SetActive(true);
			destinationMark.position = targetGroundHit;

		} else {
			lineMove.enabled = false;
			destinationMark.gameObject.SetActive(false);

		}
	}

	private void drawBatteryBar(Rect rect){
		Vector3 pos = Camera.main.WorldToScreenPoint (transform.position);
		battery.transform.position = new Vector3(pos.x,pos.y+rect.height/2,0);
	}

	private void CalculateBattery(){
		if (this.currentBattery > 0) {
			this.currentBattery -= Time.deltaTime * this.batteryUsage;
		}
		this.battery.rectTransform.sizeDelta=new Vector2(this.currentBattery, 10);
	}

	private void OnMouseDown(){
		selectedByClick = true;

		if (!this.currentlySelected) {
			player.addSelectedObject(this);
		}
		 
	}
	
	private void OnMouseUp(){
		if (selectedByClick) {
			if(!this.currentlySelected){
				player.addSelectedObject(this);
			}
		}
		selectedByClick = false;
	}
}