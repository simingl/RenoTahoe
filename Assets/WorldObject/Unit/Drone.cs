using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using System.Collections;
using System.Collections.Generic;
using RTS;

public class Drone : WorldObject {
	public GameObject routeline;
	public Texture cameraIcon;
	public const int PIP_DEPTH_ACTIVE = 2;
	public const int PIP_DEPTH_DEACTIVE = -1;
	public Color color;  

	public float moveSpeed, rotateSpeed;

	public float currentBattery = 100;
	public float batteryUsage = 0.1f;

	public int sensingRange = 100;

	private Queue<Vector3> destination;
	private Dictionary<Vector3, GameObject> routelines;
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
	private Camera camera_front, camera_down;

	private Rigidbody rigidbody;

	private StationCharger charger;

	private GameObject fire;

	public Drone(){
		scoreValue = 500;
		type = WorldObjectType.Unit;
		cellphones = new Stack<Cellphone>();
		waters = new Stack<WaterBottle>();
		this.destination = new Queue<Vector3>();
		routelines = new Dictionary<Vector3, GameObject>();
	}

	protected override void Awake() {
		base.Awake();

		fire = transform.FindChild ("fire").gameObject;
		currentBattery = ResourceManager.DroneBatteryLife;
		rigidbody = this.GetComponent<Rigidbody> ();

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

		this.camera_front = (Camera)(this.transform.FindChild ("camera_1st_view").gameObject).GetComponent<Camera>();
		this.camera_down = (Camera)(this.transform.FindChild ("camera_hover_view").gameObject).GetComponent<Camera>();

		this.camera_front.depth = PIP_DEPTH_DEACTIVE;
		this.camera_down.depth = PIP_DEPTH_DEACTIVE;
	}

	protected override void Start () {
		base.Start();

		//find the top mesh and render it
		this.setColor (color);

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

		if (this.currentStatus == STATUS.DEAD)
			return;

		if (Input.GetMouseButtonUp (0) && !EventSystem.current.IsPointerOverGameObject () && HUD.selection.width * HUD.selection.height > 10) {
			Vector3 camPos = Camera.main.WorldToScreenPoint(transform.position);
			camPos.y = Screen.height - camPos.y;
			if( HUD.selection.Contains(camPos) ){
				this.player.addSelectedObject(this);
			}
			else {
				this.player.removeSelectedObject(this);
			}
		}

		this.drawRaycastLine ();
		this.drawRouteLine ();

		switch (this.currentTask) {
		case TASK.RECHARGING:
			this.Recharging();
			break;
		case TASK.ROUTE:
			this.StartMoveInPath();
			break;
		}

		switch (this.currentStatus) {
		case STATUS.TAKEOFF:
			this.TakeOffing();
			break;
		case STATUS.ROTATING:
			this.TurnToTarget ();
			break;
		case STATUS.MOVING:
			this.MakeMove();
			break;
		case STATUS.LANDING:
			this.Landing();
			break;
		}

		this.CalculateBattery ();
	}

	protected override void OnGUI() {
		base.OnGUI();

		//reset the width of the battery bar
		Rect selectBox = WorkManager.CalculateSelectionBox(selectionBounds, playingArea);
		float width_ratio = selectBox.width/50f;   //50 is the width of the slider defined in prefabs
		batterySlider.transform.localScale = new Vector3(width_ratio+0.1f, 1,1);
		batterySlider.value = this.currentBattery;
		batterySlider.gameObject.SetActive (player.isSelected(this));

		if (base.isSelected() && player.GetComponent<ChangePOV> ().activeCamera == null) {
			if(batterySlider.gameObject.active == false){
				batterySlider.gameObject.SetActive (true);
			}
		} else {
			batterySlider.gameObject.SetActive (false);
		}

		if (this.camera_front.depth == PIP_DEPTH_ACTIVE || this.camera_down.depth == PIP_DEPTH_ACTIVE) {
			DrawCameraIcon ();
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
				Vector3 dest = new Vector3(x, y, z);
				//this.addWayPoint(dest);
				StartMove(dest);
			}
		}
	}

    public Camera getCameraFront() {
        return this.camera_front;
    }
	public Camera getCameraDown() {
		return this.camera_down;
	}
	public void StartMove() {
		Vector3 dest = this.destination.Peek ();
		dest.y = transform.position.y;

		targetRotation = Quaternion.LookRotation (dest - transform.position);
		if (this.currentStatus == STATUS.LANDED || this.currentStatus == STATUS.CHARGING) {
			this.currentStatus = STATUS.TAKEOFF;
		} else {
			//this.destination.y = transform.position.y;
			this.currentStatus = STATUS.ROTATING;
		}
	}

	public void StartMove(Vector3 d) {
		this.clearDestination ();
		this.destination.Enqueue (d);
		Vector3 dest = this.destination.Peek();
		dest.y = transform.position.y;
		
		targetRotation = Quaternion.LookRotation (dest - transform.position);
		if (this.currentStatus == STATUS.LANDED || this.currentStatus == STATUS.CHARGING) {
			this.currentStatus = STATUS.TAKEOFF;
		} else {
			//this.destination.y = transform.position.y;
			this.currentStatus = STATUS.ROTATING;
		}
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
			this.currentStatus = STATUS.MOVING;
		}
		CalculateBounds();
	}

	private void MakeMove() {
		Vector3 dest = this.destination.Peek ();
		dest.y = transform.position.y;

		transform.position = Vector3.MoveTowards(transform.position, dest, Time.deltaTime * moveSpeed);

		if (this.IsArrivedIn2D(dest)) {
			Vector3 d = this.destination.Dequeue();
			if(this.routelines.ContainsKey(d)){
				Object.Destroy(this.routelines[d]);
				this.routelines.Remove(d);
			}
			this.currentStatus = STATUS.IDLE;
		}
		CalculateBounds();
	}

	public void StopMove(){
		base.StopMove ();
		targetRotation = transform.rotation;
		this.clearDestination ();
		this.currentStatus = STATUS.IDLE;
	}

	private void drawRaycastLine(){
		Vector3 groundHit = new Vector3 (transform.position.x, 0, transform.position.z);
		lineRaycast.SetPosition (0, transform.position);
		lineRaycast.SetPosition (1, groundHit);

		if (this.currentStatus == STATUS.ROTATING || this.currentStatus== STATUS.MOVING) {
			lineMove.enabled = true;
			Vector3 targetGroundHit = new Vector3 (destination.Peek().x, 0.01f, destination.Peek().z);
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

	private void drawRouteLine(){
		if (this.destination.Count > 0) {
			Vector3[] waypoints = this.destination.ToArray();

			if(!this.routelines.ContainsKey(waypoints[0])){
				GameObject line = this.drawLine(transform.position, waypoints[0]);
				this.routelines[waypoints[0]] = line;				
			}

			for(int i=1;i<waypoints.Length;i++){
				if(!this.routelines.ContainsKey(waypoints[i])){
					GameObject line = this.drawLine(waypoints[i-1], waypoints[i]);
					this.routelines[waypoints[i]] = line;	
				}
			}
		}
	}

	private GameObject drawLine(Vector3 org, Vector3 dst){
		GameObject line = (GameObject)Instantiate (routeline, org, new Quaternion(0,0,0,1));
		LineRenderer lr = line.GetComponent<LineRenderer> ();
		lr.SetPosition (0, org);
		lr.SetPosition (1, dst);
		return line;
	}
	private void drawBatteryBar(Rect rect){
		Vector3 pos = Camera.main.WorldToScreenPoint (transform.position);
		batterySlider.transform.position = new Vector3 (pos.x,pos.y+rect.height/2,0);
	}

	private void CalculateBattery(){
		//Recharging battery
		if (this.currentStatus == STATUS.CHARGING && this.currentBattery < ResourceManager.DroneBatteryLife) {
			float chargingSpeed = (float)ResourceManager.DroneBatteryLife/(float)ResourceManager.DroneBatteryCharging;
			this.currentBattery += Time.deltaTime * chargingSpeed;
			return;
		}


		//Battery usage
		if (this.currentStatus != STATUS.CHARGING && this.currentStatus != STATUS.LANDED && this.currentBattery > 0) {
			this.currentBattery -= Time.deltaTime;
		} else if (this.currentBattery <= 0) {
			if(this.currentStatus != STATUS.LANDING){
				this.destination.Clear();
				this.Dieing();
			}
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

	public void showPIPCameraFront(){
		this.camera_front.rect = ResourceManager.getInstance ().getPIPCameraPosition();
		this.camera_front.depth = PIP_DEPTH_ACTIVE;
		this.camera_down.depth = PIP_DEPTH_DEACTIVE;
	}

	public void togglePIPCamera(){
		float tmp = this.camera_front.depth;
		this.camera_front.depth = this.camera_down.depth;
		this.camera_down.depth= tmp;
	}

	public void Unselect(){
		if (this.camera_front.rect == ResourceManager.getInstance ().getPIPCameraPosition ()) {
			this.camera_front.depth = PIP_DEPTH_DEACTIVE;
		}
		if (this.camera_down.rect == ResourceManager.getInstance ().getPIPCameraPosition ()) {
			this.camera_down.depth = PIP_DEPTH_DEACTIVE;
		}

	}

	private void DrawCameraIcon(){
		GUI.skin = ResourceManager.SelectBoxSkin;
		Rect selectBox = WorkManager.CalculateSelectionBox(selectionBounds, playingArea);
		GUI.BeginGroup(playingArea);
		Rect cameraBox = new Rect (selectBox.x+selectBox.width-5, selectBox.y, 15, 15);
		GUI.DrawTexture(cameraBox, cameraIcon);
		GUI.EndGroup();
	}

	public void Land(){
		this.currentStatus = STATUS.LANDING;
	}

	public void TakeOff(){
		if (this.currentBattery > 0) {
			this.currentStatus = STATUS.TAKEOFF;
		}
	}

	private void Landing(){
		Vector3 low = this.FindHitPoint (this.camera_front, this.transform);
		if (this.rigidbody.transform.position.y >= low.y + 0.5) {
			Vector3 newpos = this.rigidbody.transform.position;
			newpos.y -= Time.deltaTime;
			this.rigidbody.transform.position = newpos;
		} else if (this.currentBattery <= 0 || this.currentStatus == STATUS.DEAD) {
			this.Dieing();
		} else {
			currentStatus = STATUS.LANDED;
		}
	}

	private void TakeOffing(){
		Vector3 low = this.FindHitPoint (this.camera_front, this.transform);
		if (this.rigidbody.transform.position.y <= low.y + 3.5) {
			Vector3 newpos = this.rigidbody.transform.position;
			newpos.y += Time.deltaTime;
			this.rigidbody.transform.position = newpos;
		} else if (this.destination.Count > 0 && !IsArrivedIn2D(this.destination.Peek())) {
			this.StartMove();
		} else if (this.destination.Count == 0) {
			currentStatus = STATUS.IDLE;
		}
	}

	private void Recharging(){
		if (this.destination.Count == 0 || IsArrivedIn2D (this.destination.Peek())) {
			if(this.currentStatus == STATUS.IDLE){
				this.currentStatus = STATUS.LANDING;
			} else if(this.currentStatus == STATUS.LANDED){
				this.currentStatus = STATUS.CHARGING;
				this.currentTask = TASK.NULL;
			}
		}
	}

	private void StartMoveInPath(){
		if (this.destination.Count > 0 && this.currentStatus != STATUS.ROTATING && this.currentStatus != STATUS.MOVING) {
			StartMove ();
		} else if(this.destination.Count == 0){
			this.currentTask = TASK.NULL;
		}
	}

	private void Crashing(){
		this.fire.SetActive (true);
		this.rigidbody.useGravity = true;
		this.rigidbody.velocity = Vector3.zero;
		this.Dieing ();
		ScoreManager.score -= scoreValue;
	}

	public void Recharge(){
		StationCharger sc = this.player.stationManager.getNearestAvailabeCharger (transform.position);
		if (sc != null) {
			this.charger = sc;
			this.destination.Enqueue(sc.transform.position);
			this.currentTask = TASK.RECHARGING;
			this.currentStatus = STATUS.TAKEOFF;
			sc.Occupy(this);
		} else {
			Debug.Log("No availble charger.");
		}
	}

	private bool IsArrivedIn2D(Vector3 pos){
		if(Mathf.Abs(pos.x - transform.position.x) < 0.01f && Mathf.Abs(pos.z - transform.position.z) < 0.01f){
			return true;
		}
		return false;
	}

	public void OnCollisionEnter(Collision collisionInfo){
		GameObject go = collisionInfo.gameObject;
		Helicopter heli = go.GetComponent<Helicopter> ();

		if (heli != null && !this.isDead()) {
			Crashing();
		}
	} 

	public void Dieing(){
		this.currentStatus = STATUS.DEAD;
		this.setColor (Color.gray);
		this._isSelectable = false;
		this.player.removeSelectedObject (this);
	}

	public void setColor(Color col){
		this.color = col;
		//find the top mesh and render it
		transform.FindChild ("mesh").FindChild ("group_top").GetComponent<Renderer>().material.color = this.color;
	}

	public void addWayPoint(Vector3 point){
		this.destination.Enqueue (point);
	}

	private void clearDestination(){
		while (this.destination.Count >0) {
			Vector3 dest = this.destination.Dequeue();
			Object.Destroy(this.routelines[dest]);

		}
	}
}
