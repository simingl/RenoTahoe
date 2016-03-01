using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using System.Collections;
using System.Collections.Generic;
using RTS;
using UnityStandardAssets.ImageEffects;

//test for pull

public class Drone : WorldObject {
    public Texture cameraIcon;
    public const int PIP_DEPTH_ACTIVE = 2;
    public const int PIP_DEPTH_DEACTIVE = -1;
    public Color color;

    public float speed = 0f;
    private float minSpeed = 0f;
    private float maxSpeed = 2f;
    public float moveSpeed, rotateSpeed;
    private float acceleration = 0.3f;
    private float turnSpeed = 0.3f;

    public float currentBattery = 100;
    public float batteryUsage = 0.1f;

    public int sensingRange = 100;
   

    //	private Queue<Vector3> routePoints;
    private Queue<GameObject> routePointsQueue;
    public GameObject routePoints;
	private Dictionary<GameObject, GameObject> routelines;
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

    private Rigidbody rb;

    private StationCharger charger;

    private GameObject fire;

    private List<GameObject> routeLinePointsSelect;
    public GameObject emptyGameObject;

    public int droneNumber = 0;
    private Random randomNum = new Random();
    public TextMesh droneNumberText;

    private SceneManager mySceneManager;

    public Drone() {
        scoreValue = 500;
        type = WorldObjectType.Unit;
        cellphones = new Stack<Cellphone>();
        waters = new Stack<WaterBottle>();
        this.routePointsQueue = new Queue<GameObject>();
		routelines = new Dictionary<GameObject, GameObject>();

    }

    protected override void Awake() {
        base.Awake();
        fire = transform.FindChild("fire").gameObject;
        currentBattery = ResourceManager.DroneBatteryLife;
        rb = this.GetComponent<Rigidbody>();                
        this.canvas = GameObject.FindObjectOfType<Canvas>();
        //Initialize to random color

        //color = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));

        switch(this.droneNumber)
        {
            case 0:
                this.color = Color.yellow;
                break;
            case 1:
                this.color = Color.white;
                break;
            case 2:
                this.color = Color.blue;
                break;
            case 3:
                this.color = Color.red;
                break;
            case 4:
                this.color = Color.green;
                break;
            case 5:
                this.color = Color.gray;
                break;
            default:
                this.color = Color.cyan;
                break;                
        }
        
        //Create a battery bar from the prefab
        batterySlider = (Slider)GameObject.Instantiate(batterySliderfabs, new Vector3(-10000f, -10000f, -10000f), transform.localRotation);
        batterySlider.transform.SetParent(canvas.transform);
        batterySlider.transform.parent = emptyGameObject.transform;

        batterySlider.transform.localScale = Vector3.one;
        batterySlider.gameObject.SetActive(false);

        //setup the destination mark
        destinationMark = this.transform.FindChild("DestinationMark");

        this.camera_front = (Camera)(this.transform.FindChild("camera_1st_view").gameObject).GetComponent<Camera>();
        this.camera_down = (Camera)(this.transform.FindChild("camera_hover_view").gameObject).GetComponent<Camera>();
        this.camera_front.depth = PIP_DEPTH_DEACTIVE;
        this.camera_down.depth = PIP_DEPTH_DEACTIVE;
    }

    public void FrontCameraBlur()
    {
        //		this.camera_front.
    }

    protected override void Start() {
        base.Start();
        this.droneNumberText.text = "No. " + this.droneNumber;
        //hold routLinePoints prefab in an object;
        //		routLinePointsSelect = GameObject.FindGameObjectsWithTag("RouteLinePoint"); 	

        //find the top mesh and render it
        this.setColor(color);

        //setup the line from object to the ground
        lineRaycast = this.GetComponent<LineRenderer>();
        lineRaycast.useWorldSpace = true;

       

        this.malFunction ();
    }

    protected override void Update() {
        base.Update();
        //select an object------
        //		if (Input.GetMouseButtonDown(0))
        //		{
        //			RaycastHit hitInfo = new RaycastHit();
        ////			if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo) && 
        ////				hitInfo.transform.tag == "RouteLinePoint"
        ////				)
        //			if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo) && hitInfo.transform.tag == "RouteLinePoint")
        //			{
        //				print ("It's working");
        //			}
        //		}
        //-------------
        //for (int i = 0; i < ConfigManager.getInstance().getSceneDroneCount(); ++i)
        //{
        //    if (this.droneNumber == mySceneManager.getAllDrones()[i].droneNumber)
        //        ++this.droneNumber;
        //}
        //if (this.isSelected())
        //{
        //    //player.getSelectedObjects()[0].isSelected()
        //    //Debug.Log(this.transform.position);
        //    getDroneArea();
        //}
        if (this.currentStatus == STATUS.DEAD)
            return;

        if (Input.GetMouseButtonUp(0) && !EventSystem.current.IsPointerOverGameObject() && HUD.selection.width * HUD.selection.height > 10) {
            Vector3 camPos = Camera.main.WorldToScreenPoint(transform.position);
            camPos.y = Screen.height - camPos.y;
            if (HUD.selection.Contains(camPos)) {
                this.player.addSelectedObject(this);
            }
            else {
                this.player.removeSelectedObject(this);
            }
        }

        this.HandleKeyboardControl();
        //this.drawRaycastLine ();
        this.drawRouteLine();

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
            case STATUS.MOVING:
                this.MakeRotateMove();
                break;
            case STATUS.LANDING:
                this.Landing();
                break;
        }

        this.CalculateBattery();
    }

    public void blurEnable()
    {
        Component myScript = this.GetComponentInChildren<Drone>();
        Debug.Log(myScript.gameObject);
    }

    public void blurDisable()
    {
       
        if(this.isSelected() == true)
        {
            Debug.Log(this.name);
        }
    }


    public void testDroneNumberButton()
    {
        //Debug.Log(player.getSelectedObjects()[0]);
        //Debug.Log(ConfigManager.getInstance().getSceneDroneCount());
    }


    public int getDroneArea()
    {
        int droneCount = ConfigManager.getInstance().getSceneDroneCount();
        int rootOfDrone = (int)Mathf.Sqrt(droneCount);
        float gridSizeOfScene = 200 / rootOfDrone;
        
        int dronePostionOffset = 100;
        int result = 0;
        for (int i=0; i< rootOfDrone; ++i)
        {
            for (int j=0; j< rootOfDrone; ++j )
            {
                if(gridSizeOfScene * i<(this.transform.position.x+dronePostionOffset) && 
                   gridSizeOfScene * (i+1) > (this.transform.position.x+ dronePostionOffset) &&
                   gridSizeOfScene * j < (this.transform.position.z + dronePostionOffset) &&
                   gridSizeOfScene * (j + 1) > (this.transform.position.z + dronePostionOffset)
                   )
                {
                    result = i + (rootOfDrone -1-j)*rootOfDrone;
                }
            }
        }
        //Debug.Log(this.name + "is in: " + result);
        return result;
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
		Vector3 dest = this.routePointsQueue.Peek ().transform.position;
		dest.y = transform.position.y;
		targetRotation = Quaternion.LookRotation (dest - transform.position);
		if (this.currentStatus == STATUS.LANDED || this.currentStatus == STATUS.CHARGING) {
			this.currentStatus = STATUS.TAKEOFF;
		} else {
			this.currentStatus = STATUS.MOVING;
		}
	}

	private void SpawnRoutePoints(Vector3 d)
	{
		GameObject colon = (GameObject)Instantiate (routePoints, d, new Quaternion(0,0,0,1));
		colon.GetComponent<Renderer>().material.color = color;
		colon.layer = ResourceManager.LayerMainCamerea;
        colon.transform.parent = emptyGameObject.transform;
        this.routePointsQueue.Enqueue (colon);
	}

    public void writeDroneAreasToXML()
    {

    }

	public void StartMove(Vector3 d) {
		this.clearDestination ();
		SpawnRoutePoints(d);
//		this.routePointsQueue.Enqueue (d);
		Vector3 dest = this.routePointsQueue.Peek().transform.position;
		dest.y = transform.position.y;
		
		targetRotation = Quaternion.LookRotation (dest - transform.position);
		if (this.currentStatus == STATUS.LANDED || this.currentStatus == STATUS.CHARGING) {
			this.currentStatus = STATUS.TAKEOFF;
		} else {
			this.currentStatus = STATUS.MOVING;
		}
	}

	protected override void DrawSelectionBox(Rect rect){
		base.DrawSelectionBox (rect);
		this.drawBatteryBar (rect);
	}

	private void MakeRotateMove() {
		if (this.routePointsQueue.Count > 0) {
			Vector3 dest = this.routePointsQueue.Peek ().transform.position;
			dest.y = transform.position.y;

			float distance = Vector3.Distance (dest, transform.position);
			//heading
			Quaternion targetRotation = Quaternion.LookRotation (dest - transform.position);
			transform.rotation = Quaternion.RotateTowards (transform.rotation, targetRotation, this.turnSpeed * Time.timeScale);

			//speed
			if (distance / speed <= speed / acceleration) {
				speed -= acceleration * Time.deltaTime;
			} else {
				speed += acceleration * Time.deltaTime;
			}
			speed = Mathf.Clamp (speed, minSpeed, maxSpeed);
			transform.Translate (0, 0, speed * Time.deltaTime);

			if (this.IsArrivedIn2D (dest)) {
				speed = 0f;
                //                GameObject tmp = this.routePointsQueue.Peek();
                //                Vector3 d = tmp.transform.position ;
                //                this.routePointsQueue.Dequeue();
                //                Object.Destroy(tmp);
                ////				this.routePoints.Dequeue ();
                //				if (this.routelines.ContainsKey (d)) {
                //					Object.Destroy (this.routelines [d]);
                //					this.routelines.Remove (d);
                //				}

                GameObject ball = this.routePointsQueue.Dequeue();
                Object.Destroy(this.routelines[ball]);
                this.routelines.Remove(ball);
                Object.Destroy(ball);
                this.currentStatus = STATUS.IDLE;
			}

			CalculateBounds ();
		}
	}

	private void HandleKeyboardControl(){
		Vector3 leftaxis = transform.TransformDirection(Vector3.up);
		if (player.isSelected (this)) {
			transform.RotateAround (transform.position, leftaxis, Input.GetAxis ("Horizontal") * 0.5f);
			speed += Input.GetAxis ("Vertical") * acceleration * Time.deltaTime*10;
		}
		
		speed = Mathf.Clamp (speed, minSpeed, maxSpeed);

		//altitude
		float jump = Input.GetAxis("Jump");
		if (jump != 0) {
			this.currentStatus = STATUS.MOVING;

			Vector3 newPos = transform.up * jump * 0.01f;
			transform.position += newPos;
			
			newPos.x = transform.position.x;
			newPos.y = Mathf.Clamp(rb.transform.position.y, ResourceManager.MaxBottom,  ResourceManager.MaxTop);
			newPos.z = transform.position.z;
			transform.position = newPos;
		}

		//move forward
		if (this.routePointsQueue.Count <= 0 && speed > 0) {
			this.currentStatus = STATUS.MOVING;
			transform.Translate (0, 0, speed * Time.deltaTime);
			this.CalculateBounds ();
		}	
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
	}

    private void drawRouteLine()
    {
        if (this.routePointsQueue.Count > 0)
        {
            //			Vector3[] waypoints = this.routePoints.ToArray();
            GameObject[] waypoints = this.routePointsQueue.ToArray();

            if (!this.routelines.ContainsKey(waypoints[0]))
            {
                GameObject line = this.drawLine(transform.position, waypoints[0].transform.position);
                this.routelines[waypoints[0]] = line;
            }
            else {
                GameObject line = this.routelines[waypoints[0]];
                LineRenderer lr = line.GetComponent<LineRenderer>();
                lr.SetPosition(0, transform.position);
				lr.SetPosition(1, waypoints[0].transform.position);
            }

            for (int i = 1; i < waypoints.Length; i++)
            {
                if (!this.routelines.ContainsKey(waypoints[i]))
                {
                    GameObject line = this.drawLine(waypoints[i - 1].transform.position, waypoints[i].transform.position);
                    this.routelines[waypoints[i]] = line;
				}else{
					LineRenderer line = this.routelines[waypoints[i]].GetComponent<LineRenderer>();
					line.SetPosition(0, waypoints[i-1].transform.position);
					line.SetPosition(1, waypoints[i].transform.position);
				}
            }
        }
    }


//	private GameObject drawLine(Vector3 org, Vector3 dst){
//		GameObject line = (GameObject)Instantiate (routePoints, dst, new Quaternion(0,0,0,1));
//
//		LineRenderer lr = line.GetComponent<LineRenderer> ();
//		line.GetComponent<Renderer>().material.color=color;
//		lr.SetPosition (0, org);
//		lr.SetPosition (1, dst);
//		lr.material = new Material(Shader.Find("Particles/Additive"));
//		lr.SetColors(color, color);
//		line.layer = ResourceManager.LayerMainCamerea;
//
//		return line;
//	}

    //public 


	private GameObject drawLine(Vector3 org, Vector3 dst){
		GameObject[] tmpQueue = routePointsQueue.ToArray();
		//int length = tmpQueue.Length;
        LineRenderer lr = tmpQueue[tmpQueue.Length-1].GetComponent<LineRenderer>();
        //LineRenderer lr = tmpQueue[0].GetComponent<LineRenderer>();
        lr.SetPosition(0, org);
        lr.SetPosition (1, dst);
		lr.material = new Material(Shader.Find("Particles/Additive"));
		lr.SetColors(color, color);
		return tmpQueue[tmpQueue.Length - 1];
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
                while(routePointsQueue.Count !=0)
                { 
                    Destroy(routePointsQueue.Dequeue());
                }
				//this.routePointsQueue.Clear();
				this.Dieing();
			}
		}
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
		if (this.currentStatus != STATUS.LANDED) {
			Vector3 newpos = this.rb.transform.position;
			newpos.y -= Time.deltaTime;
			this.rb.transform.position = newpos;
		} else if (this.currentBattery <= 0 || this.currentStatus == STATUS.DEAD) {
			this.Dieing();
		} 
	}

	float takeOffHeight = 5f;
	float deltaHeight = 0f;
	private void TakeOffing(){
		if (deltaHeight < takeOffHeight) {
			Vector3 newpos = this.rb.transform.position;
			newpos.y += Time.deltaTime;
			deltaHeight+= Time.deltaTime;
			this.rb.transform.position = newpos;
		} else if (this.routePointsQueue.Count > 0 && !IsArrivedIn2D(this.routePointsQueue.Peek().transform.position)) {
			this.StartMove();
			this.deltaHeight = 0;
		} else if (this.routePointsQueue.Count == 0) {
			currentStatus = STATUS.IDLE;
			this.deltaHeight = 0;
		}
	}

	private void Recharging(){
		if (this.routePointsQueue.Count == 0 || IsArrivedIn2D (this.routePointsQueue.Peek().transform.position)) {
			if(this.currentStatus == STATUS.IDLE){
				this.currentStatus = STATUS.LANDING;
			} else if(this.currentStatus == STATUS.LANDED){
				this.currentStatus = STATUS.CHARGING;
				this.currentTask = TASK.NULL;
			}
		}
	}

	private void StartMoveInPath(){
		if (this.routePointsQueue.Count > 0 && this.currentStatus != STATUS.MOVING) {
			StartMove ();
		} else if(this.routePointsQueue.Count == 0){
			this.currentTask = TASK.NULL;
		}
	}

	private void Crashing(){
		this.fire.SetActive (true);
		this.rb.useGravity = true;
		this.rb.velocity = Vector3.zero;
		this.Dieing ();
		ScoreManager.score -= scoreValue;
	}

	public void Recharge(){
		StationCharger sc = this.player.stationManager.getNearestAvailabeCharger (transform.position);
		if (sc != null) {
			this.charger = sc;
			SpawnRoutePoints(sc.transform.position);
//			GameObject colon = (GameObject)Instantiate(routePoints,sc.transform.position, new Quaternion(0,0,0,1));
//			colon.GetComponent<Renderer>().material.color = color;
//			colon.layer = ResourceManager.LayerMainCamerea;
//			this.routePointsQueue.Enqueue(sc.transform.position);
//			this.routePointsQueue.Enqueue(colon);
			this.currentTask = TASK.RECHARGING;
			this.currentStatus = STATUS.TAKEOFF;
			sc.Occupy(this);
		} else {
			Debug.Log("No availble charger.");
		}
	}

	private bool IsArrivedIn2D(Vector3 pos){
		if(Mathf.Abs(pos.x - transform.position.x) < 0.5f && Mathf.Abs(pos.z - transform.position.z) < 0.5f){
			return true;
		}
		return false;
	}

	public void OnCollisionEnter(Collision collisionInfo){
		GameObject go = collisionInfo.gameObject;
		//crash
		if (go.GetComponent<Helicopter> () != null && !this.isDead ()) {
			this.Crashing ();
		} else if (this.currentStatus == STATUS.LANDING &&( go.name == "RenoDestroyed" || go.tag == "StationCharger")) {
			this.currentStatus = STATUS.LANDED;
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
//		GameObject colon = (GameObject)Instantiate(routePoints,point, new Quaternion(0,0,0,1));
//		colon.GetComponent<Renderer>().material.color = color;
//		colon.layer = ResourceManager.LayerMainCamerea;
//		this.routePointsQueue.Enqueue (point);
//		this.routePointsQueue.Enqueue (colon);
		SpawnRoutePoints(point);
	}

	private void clearDestination(){
		while (this.routePointsQueue.Count >0) {
			GameObject ball = this.routePointsQueue.Dequeue();
			Object.Destroy(this.routelines[ball]);
			Object.Destroy(ball);
		}
	}

	public Vector3 getCurrentDestination(){
		if (this.routePointsQueue.Count > 0) {
			return this.routePointsQueue.Peek ().transform.position;
		} else {
			return ResourceManager.InvalidPosition;
		}
	}

	private void malFunction(){
		Random random = new Random ();
		int randInt = Random.Range (1, ConfigManager.getInstance ().getSceneDroneCount ());
		if(randInt == 2) {
			this.malFunctionCamera ();
		} else if (randInt == 3){
			this.malFunctionSpeed();
		}
	}

	private void malFunctionCamera(){
		Blur blur = camera_front.GetComponent<Blur>();
		blur.enableFunc();
	}

	private void malFunctionSpeed(){
		this.maxSpeed /= 2;
	}


}
