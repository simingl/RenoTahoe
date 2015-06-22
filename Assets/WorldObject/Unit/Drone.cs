﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using RTS;

public class Drone : WorldObject {
	public Color color;
	public Image batteryBarImage;


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

	public Drone(){
		type = WorldObjectType.Unit;
		cellphones = new Stack<Cellphone>();
		waters = new Stack<WaterBottle>();
	}

	protected override void Awake() {
		base.Awake();

		this.canvas = GameObject.FindObjectOfType<Canvas> ();

		//Create a battery bar from the prefab
		batterySlider = (Slider)GameObject.Instantiate (batterySliderfabs, new Vector3(-10000f, -10000f, -10000f), transform.localRotation);
		batterySlider.transform.SetParent (canvas.transform);
		batterySlider.transform.localScale = Vector3.one;
		batterySlider.gameObject.SetActive(false);

	}

	protected override void Start () {
		base.Start();
		//Initialize to random color
		color = new Color(Random.Range(0.0f,1.0f),Random.Range(0.0f,1.0f),Random.Range(0.0f,1.0f));  
		//find the top mesh and render it
		transform.FindChild ("mesh").FindChild ("group_top").GetComponent<Renderer>().material.color = this.color;

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

		if (Input.GetMouseButtonUp (0) && player.hud.MouseInBounds()) {
			Vector3 camPos = Camera.main.WorldToScreenPoint(transform.position);
			camPos.y = CameraManagement.InvertMouseY(camPos.y);
			if(HUD.selection.Contains(camPos)){
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
		this.batterySlider.value = this.currentBattery;

		batterySlider.gameObject.SetActive (currentlySelected);
		ChangePOV pov = player.GetComponent<ChangePOV> ();

		if (currentlySelected && player.GetComponent<ChangePOV> ().activeCamera == null) {
			batterySlider.gameObject.SetActive (true);
		} else {
			batterySlider.gameObject.SetActive (false);
		}
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
}