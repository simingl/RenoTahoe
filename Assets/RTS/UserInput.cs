using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RTS;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UserInput : MonoBehaviour {
	private Player player;
	private GameObject dayNightToggle;

	// Use this for initialization
	void Start () {
		player = transform.root.GetComponent< Player >();
		dayNightToggle = GameObject.FindGameObjectWithTag ("DayNightToggle");
	}
	
	// Update is called once per frame
	void Update () {
		if (player.human && Camera.main) {
			MoveCameraByMouse ();
			//RotateCamera ();
			MouseActivity();
			KeyboardActivity();
		}
	}

	void FixedUpdate(){
		float h = Input.GetAxis ("Horizontal");
		float v = Input.GetAxis ("Vertical");
		float j = Input.GetAxis ("Jump");

		MoveByInputAxis (h,j,v);
	}
	
	void MoveByInputAxis(float horizontal, float jump, float vertical){
		if (horizontal != 0f || vertical != 0f || jump != 0f) {
			if(player.getSelectedObjects().Count > 0){
				WorldObject wo = player.getSelectedObjects()[0];
				wo.CalculateBounds();
				if(wo is Drone) {
					((Drone)wo).StopMove();
				}
				Rigidbody rb = wo.gameObject.GetComponent<Rigidbody> ();
				this.RotatingObject(rb, horizontal);
				this.MovingObject(rb,jump, vertical);
			}
		}
	}

	void RotatingObject2(Rigidbody rb, float horizontal){
		Vector3 horizontalAxis = rb.transform.TransformDirection(Vector3.up);
		rb.transform.RotateAround(rb.transform.position, horizontalAxis, horizontal);
	}

	void MovingObject2(Rigidbody rb, float jump, float vertical){
		Vector3 newVelocity = rb.transform.forward * vertical;

		rb.velocity += newVelocity*0.1f;
		if (rb.gameObject.name == "drone") {
			Debug.Log (rb.velocity + ", " + rb.transform.forward);
		}
	}

	void RotatingObject(Rigidbody rb, float horizontal){
		Vector3 leftaxis = rb.transform.TransformDirection(Vector3.up);
		rb.transform.RotateAround(rb.transform.position, leftaxis, horizontal);
	}

	void MovingObject(Rigidbody rb, float jump, float vertical){
		if (vertical != 0) {
			Vector3 newPos = rb.transform.forward * vertical * 0.1f;
			rb.transform.position += newPos;

			newPos.x = Mathf.Clamp(rb.transform.position.x, ResourceManager.MaxEast,  ResourceManager.MaxWest);
			newPos.y = rb.transform.position.y;
			newPos.z = Mathf.Clamp(rb.transform.position.z, ResourceManager.MaxSouth,  ResourceManager.MaxNorth);
			rb.transform.position = newPos;
		}

		if (jump != 0) {
			Vector3 newPos = rb.transform.up * jump * 0.1f;
			rb.transform.position += newPos;

			newPos.x = rb.transform.position.x;
			newPos.y = Mathf.Clamp(rb.transform.position.y, ResourceManager.MaxBottom,  ResourceManager.MaxTop);
			newPos.z = rb.transform.position.z;
			rb.transform.position = newPos;
		}
	}
	private void MoveCameraByMouse() {
		float xpos = Input.mousePosition.x;
		float ypos = Input.mousePosition.y;
		Vector3 movement = new Vector3(0,0,0);

		//horizontal camera movement
		if(xpos >= 0 && xpos < ResourceManager.ScrollWidth) {
			movement.x -= ResourceManager.ScrollSpeed;
		} else if(xpos <= Screen.width && xpos > (Screen.width - ResourceManager.ScrollWidth)) {
			movement.x += ResourceManager.ScrollSpeed;
		}

		//vertical camera movement
		if(ypos >= 0 && ypos < ResourceManager.ScrollWidth) {
			movement.z -= ResourceManager.ScrollSpeed;
		} else if(ypos <= Screen.height && ypos > Screen.height - ResourceManager.ScrollWidth) {
			movement.z += ResourceManager.ScrollSpeed;
		}

		//away from ground movement
		Camera.main.orthographicSize -= ResourceManager.ScrollSpeed * Input.GetAxis("Mouse ScrollWheel");
		//movement += Camera.main.transform.forward * ResourceManager.ScrollSpeed * Input.GetAxis("Mouse ScrollWheel");;

		//calculate desired camera position based on received input
		Vector3 origin = Camera.main.transform.position;
		Vector3 destination = origin;
		destination.x += movement.x;
		destination.z += movement.z;

		//limit away from ground movement to be between a minimum and maximum distance
		destination.x = Mathf.Clamp(destination.x, ResourceManager.MinCameraWidth, ResourceManager.MaxCameraWidth);
		destination.z = Mathf.Clamp(destination.z, ResourceManager.MinCameraLength, ResourceManager.MaxCameraLength);

		//if a change in position is detected perform the necessary update
		if(destination != origin) {
			Camera.main.transform.position = Vector3.MoveTowards(origin, destination, Time.deltaTime * ResourceManager.ScrollSpeed*10);
		}
	}
	
	private void RotateCamera() {
		
	}
	private void MouseActivity() {
		if (EventSystem.current.IsPointerOverGameObject ())
			return;

		if(Input.GetMouseButtonUp(0)) LeftMouseClick();
		else if(Input.GetMouseButtonDown(1)) RightMouseClick();
	}

	private void KeyboardActivity() {
		if (Input.GetKey (KeyCode.LeftControl) && Input.GetKey (KeyCode.LeftShift)) {
			if(Input.GetMouseButtonUp(0)){
				Vector3 hitPoint = FindHitPoint();
				hitPoint.y = 4;
				this.player.sceneManager.CreateDrone(hitPoint);
			}
		}
	}

	private void LeftMouseClick() {
		if (Input.GetKey(KeyCode.LeftShift))
		{
			GameObject hitObject = FindHitObject();
			if(hitObject.tag == "Drone"){
				Drone drone = hitObject.GetComponent<Drone>();
				Camera cam = drone.getCameraFront();
				if(cam.depth !=Drone.PIP_DEPTH_ACTIVE){
					cam.rect = ResourceManager.getInstance().getAvailableCameraPosition(cam);
					cam.depth = Drone.PIP_DEPTH_ACTIVE;
				}
			}
		}else if(player.hud.MouseInBounds()) {
			GameObject hitObject = FindHitObject();
			Vector3 hitPoint = FindHitPoint();
			if(hitObject && hitPoint != ResourceManager.InvalidPosition) {
				if(hitObject.name!="Ground") {
					WorldObject worldObject = hitObject.GetComponent< WorldObject >();
					if(worldObject && worldObject.isSelectable()) {
						if(Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)){
							player.addSelectedObject(worldObject);
						}else{
							player.setSelectedObject(worldObject);
						}
					}
				}
			}
		}
	}
	private void RightMouseClick() {
		if(player.hud.MouseInBounds() && !Input.GetKey(KeyCode.LeftAlt) && player.getSelectedObjects().Count>0) {
			GameObject hitObject = FindHitObject();
			Vector3 hitPoint = FindHitPoint();

			if(player.getSelectedObjects().Count > 0){
				foreach(WorldObject obj in player.getSelectedObjects()){
					obj.MouseClick(hitObject, hitPoint, player);
				}
				this.player.audioManager.playUnitMoveToSound();
			}
		}
	}
	private GameObject FindHitObject() {
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if(Physics.Raycast(ray, out hit)) return hit.collider.gameObject;
		return null;
	}

	private Vector3 FindHitPoint() {
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if(Physics.Raycast(ray, out hit)) return hit.point;
		return ResourceManager.InvalidPosition;
	}
}
