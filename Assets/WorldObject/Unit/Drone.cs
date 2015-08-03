using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using System.Collections;
using System.Collections.Generic;
using RTS;

public class Drone : WorldObject {
	private const int PIP_DEPTH_ACTIVE = 2;
	private const int PIP_DEPTH_DEACTIVE = -1;
	public Color color;  

	protected bool moving, rotating;
	public float moveSpeed, rotateSpeed;

	public float startBattery = 100;
	public float currentBattery = 100;
	public float batteryUsage = 0.1f;

	public int sensingRange = 100;

	private Vector3 destination;
	private Quaternion targetRotation;
	
	private LineRenderer lineRaycast;
	private GameObject lineMoveContainer;
	private LineRenderer lineMove;
	
	private Transform destinationMark;
	private Image battery;
	private Canvas canvas;

	public Slider batterySliderfabs;
	private Slider batterySlider;

	private Stack<Cellphone> cellphones;
	private Stack<WaterBottle> waters;
	private int MAX_CELL = 8;
	private int MAX_WATER = 5;

	private Projector projector;
	private Camera camera_1st_view, camera_3rd_view, camera_hover_view;

	private Rigidbody rigidbody;



	public Drone(){
		type = WorldObjectType.Unit;
		cellphones = new Stack<Cellphone>();
		waters = new Stack<WaterBottle>();
	}

	protected override void Awake() {
		base.Awake();


		rigidbody = this.GetComponent<Rigidbody> ();
		//rigidBody.Sleep ();

		this.canvas = GameObject.FindObjectOfType<Canvas> ();
		//Initialize to random color
		color = new Color(Random.Range(0.0f,1.0f),Random.Range(0.0f,1.0f),Random.Range(0.0f,1.0f));  

		//Create a battery bar from the prefab
		batterySlider = (Slider)GameObject.Instantiate (batterySliderfabs, new Vector3(-10000f, -10000f, -10000f), transform.localRotation);
		batterySlider.transform.SetParent (canvas.transform);
		batterySlider.transform.localScale = Vector3.one;
		batterySlider.gameObject.SetActive(false);

		//setup the destination mark
		destinationMark = this.transform.FindChild("DestinationMark");

		this.camera_1st_view = (Camera)(this.transform.FindChild ("camera_1st_view").gameObject).GetComponent<Camera>();
		this.camera_3rd_view = (Camera)(this.transform.FindChild ("camera_3rd_view").gameObject).GetComponent<Camera>();
		this.camera_hover_view = (Camera)(this.transform.FindChild ("camera_hover_view").gameObject).GetComponent<Camera>();

		this.camera_1st_view.depth = PIP_DEPTH_DEACTIVE;
		this.camera_3rd_view.depth = PIP_DEPTH_DEACTIVE;
		this.camera_hover_view.depth = PIP_DEPTH_DEACTIVE;

		this.SetPIPCameraActive (false);
	}

	protected override void Start () {
		base.Start();

		//find the top mesh and render it
		transform.FindChild ("mesh").FindChild ("group_top").GetComponent<Renderer>().material.color = this.color;


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

		if (Input.GetMouseButtonUp (0) && !EventSystem.current.IsPointerOverGameObject () && HUD.selection.width * HUD.selection.height > 10) {
			Vector3 camPos = Camera.main.WorldToScreenPoint(transform.position);
			camPos.y = Screen.height - camPos.y;
			//camPos.y = CameraManagement.InvertMouseY(camPos.y);
			if( HUD.selection.Contains(camPos) ){
				this.player.addSelectedObject(this);
			}
			else {
				this.player.removeSelectedObject(this);
			}
		}

		this.drawRaycastLine ();

		if(rotating) TurnToTarget();
		else if(moving) MakeMove();

		this.CalculateBattery ();
	}



	protected override void OnGUI() {
		base.OnGUI();
		batterySlider.value = this.currentBattery;
		batterySlider.gameObject.SetActive (player.isSelected(this));

		if (base.isSelected() && player.GetComponent<ChangePOV> ().activeCamera == null) {
			batterySlider.gameObject.SetActive (true);
		} else {
			batterySlider.gameObject.SetActive (false);
			this.camera_1st_view.depth = PIP_DEPTH_DEACTIVE;
			this.camera_3rd_view.depth = PIP_DEPTH_DEACTIVE;
			this.camera_hover_view.depth = PIP_DEPTH_DEACTIVE;
		}
	}

	public override void MouseClick(GameObject hitObject, Vector3 hitPoint, Player controller) {
		base.MouseClick(hitObject, hitPoint, controller);
		//only handle input if owned by a human player and currently selected
		if(player && player.human && base.isSelected() && Input.GetMouseButton(1)) {
			if(hitPoint != ResourceManager.InvalidPosition) {
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

	public void StopMove(){
		base.StopMove ();
		targetRotation = transform.rotation;
		destination = transform.position;
		rotating = false;
		moving = false;
	}

	private void drawRaycastLine(){
		Vector3 groundHit = new Vector3 (transform.position.x, 0, transform.position.z);
		lineRaycast.SetPosition (0, transform.position);
		lineRaycast.SetPosition (1, groundHit);

		if (rotating || moving) {
			lineMove.enabled = true;
			Vector3 targetGroundHit = new Vector3 (destination.x, 0.01f, destination.z);
			lineMove.SetPosition (0, transform.position);
			lineMove.SetPosition (1, targetGroundHit);

			destinationMark.gameObject.SetActive(true);
			destinationMark.position = targetGroundHit;
			destinationMark.transform.parent = null;
		} else {
			lineMove.enabled = false;
			destinationMark.gameObject.SetActive(false);
			destinationMark.transform.parent = transform;
		}
	}

	private void drawBatteryBar(Rect rect){
		Vector3 pos = Camera.main.WorldToScreenPoint (transform.position);
		batterySlider.transform.position = new Vector3 (pos.x,pos.y+rect.height/2,0);
	}

	private void CalculateBattery(){
		if (this.currentBattery > 0) {
			this.currentBattery -= Time.deltaTime * this.batteryUsage;
		}
	}

	public void DropCell(){
		if (this.cellphones.Count == 0)
			return;

		Cellphone cell = this.cellphones.Pop ();
		if (cell != null) {
			cell.parent = null;
			cell.transform.SetParent(null);
			cell.transform.GetComponent<Rigidbody>().useGravity = true;
			cell.gameObject.GetComponent<Collider>().enabled = true;
		}
	}

	public void LoadCell(){
		if (this.cellphones.Count >= MAX_CELL)
			return;

		Cellphone cell =  this.player.sceneManager.getFreeCellphone (this.transform.position, this.sensingRange);
		if (cell!=null) {
			this.cellphones.Push (cell);
			cell.parent = this;
			cell.gameObject.transform.SetParent(this.gameObject.transform);
			cell.gameObject.transform.localRotation = new Quaternion(0,0,0,1);

			cell.gameObject.transform.localPosition = new Vector3(12f,0f,-10f+2f*this.cellphones.Count);

			Rigidbody rb = cell.gameObject.GetComponent<Rigidbody>();

			rb.useGravity = false;
			rb.velocity = Vector3.zero;
			rb.freezeRotation = true;
			cell.gameObject.GetComponent<Collider>().enabled = false;
		}
	}

	public int GetCellCount(){
		return cellphones.Count;
	}

	public void DropWater(){
		if (this.waters.Count == 0)
			return;
		
		WaterBottle water = this.waters.Pop ();
		if (water != null) {
			water.parent = null;
			water.transform.SetParent(null);
			water.transform.GetComponent<Rigidbody>().useGravity = true;
			water.gameObject.GetComponent<Collider>().enabled = true;
		}
	}
	
	public void LoadWater(){
		if (this.waters.Count >= MAX_WATER)
			return;
		
		WaterBottle water =  this.player.sceneManager.getFreeWaterBottle (this.transform.position, this.sensingRange);
		if (water!=null) {
			this.waters.Push (water);
			water.parent = this;
			water.gameObject.transform.SetParent(this.gameObject.transform);
			water.gameObject.transform.localRotation = new Quaternion(0,0,0,1);
			
			water.gameObject.transform.localPosition = new Vector3(-12f,0f,-10f+3f*this.waters.Count);
			
			Rigidbody rb = water.gameObject.GetComponent<Rigidbody>();
			
			rb.useGravity = false;
			rb.velocity = Vector3.zero;
			rb.freezeRotation = true;
			water.gameObject.GetComponent<Collider>().enabled = false;
		}
	}
	
	public int GetWaterCount(){
		return waters.Count;
	}

	public void SetPIPCameraActive(bool isActive){
		this.camera_1st_view.enabled = isActive;
		this.camera_3rd_view.enabled = isActive;
		this.camera_hover_view.enabled = isActive;
	}

	public void showPIP(int i){
		if (i == 0) {
			this.camera_1st_view.depth = PIP_DEPTH_ACTIVE;
			this.camera_3rd_view.depth = PIP_DEPTH_DEACTIVE;
			this.camera_hover_view.depth = PIP_DEPTH_DEACTIVE;
		} else if (i == 1) {
			this.camera_1st_view.depth = PIP_DEPTH_DEACTIVE;
			this.camera_3rd_view.depth = PIP_DEPTH_ACTIVE;
			this.camera_hover_view.depth = PIP_DEPTH_DEACTIVE;

		} else if (i == 2) {
			this.camera_1st_view.depth = PIP_DEPTH_DEACTIVE;
			this.camera_3rd_view.depth = PIP_DEPTH_DEACTIVE;
			this.camera_hover_view.depth = PIP_DEPTH_ACTIVE;

		}
	}


}
