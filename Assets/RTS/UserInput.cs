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
				Rigidbody rb = wo.gameObject.GetComponent<Rigidbody> ();
				this.RotatingObject(rb, horizontal);
				this.MovingObject(rb,jump, vertical);
			}
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
		}

		if (jump != 0) {
			Vector3 newPos = rb.transform.up * jump * 0.1f;
			rb.transform.position += newPos;
		}
	}
	private void MoveCameraByMouse() {
		float xpos = Input.mousePosition.x;
		float ypos = Input.mousePosition.y;
		Vector3 movement = new Vector3(0,0,0);

		bool mouseScroll = false;
		
		//horizontal camera movement
		if(xpos >= 0 && xpos < ResourceManager.ScrollWidth) {
			movement.x -= ResourceManager.ScrollSpeed;
			player.hud.SetCursorState(CursorState.PanLeft);
			mouseScroll = true;
		} else if(xpos <= Screen.width && xpos > (Screen.width - ResourceManager.ScrollWidth)) {
			movement.x += ResourceManager.ScrollSpeed;
			player.hud.SetCursorState(CursorState.PanRight);
			mouseScroll = true;
		}

		if(!mouseScroll) {
			player.hud.SetCursorState(CursorState.Select);
		}

		//vertical camera movement
		if(ypos >= 0 && ypos < ResourceManager.ScrollWidth) {
			movement.z -= ResourceManager.ScrollSpeed;
			player.hud.SetCursorState(CursorState.PanDown);
			mouseScroll = true;
		} else if(ypos <= Screen.height && ypos > Screen.height - ResourceManager.ScrollWidth) {
			movement.z += ResourceManager.ScrollSpeed;
			player.hud.SetCursorState(CursorState.PanUp);
			mouseScroll = true;
		}

		//make sure movement is in the direction the camera is pointing
		//but ignore the vertical tilt of the camera to get sensible scrolling
		movement = Camera.main.transform.TransformDirection(movement);
		movement.y = 0;

		//away from ground movement
		movement.y -= ResourceManager.ScrollSpeed * Input.GetAxis("Mouse ScrollWheel");

		//calculate desired camera position based on received input
		Vector3 origin = Camera.main.transform.position;
		Vector3 destination = origin;
		destination.x += movement.x;
		destination.y += movement.y;
		destination.z += movement.z;

		//limit away from ground movement to be between a minimum and maximum distance
		if(destination.y > ResourceManager.MaxCameraHeight) {
			destination.y = ResourceManager.MaxCameraHeight;
		} else if(destination.y < ResourceManager.MinCameraHeight) {
			destination.y = ResourceManager.MinCameraHeight;
		}
		//if a change in position is detected perform the necessary update
		if(destination != origin) {
			Camera.main.transform.position = Vector3.MoveTowards(origin, destination, Time.deltaTime * ResourceManager.ScrollSpeed);
		}
	}
	
	private void RotateCamera() {
		
	}
	private void MouseActivity() {
		if (EventSystem.current.IsPointerOverGameObject ())
			return;

		if(Input.GetMouseButtonUp(0)) LeftMouseClick();
		else if(Input.GetMouseButtonDown(1)) RightMouseClick();

		this.MouseHover ();
	}

	private void LeftMouseClick() {
		if(player.hud.MouseInBounds()) {
			GameObject hitObject = FindHitObject();
			Vector3 hitPoint = FindHitPoint();
			if(hitObject && hitPoint != ResourceManager.InvalidPosition) {
				if(hitObject.name!="Ground") {
					WorldObject worldObject = hitObject.GetComponent< WorldObject >();
					if(worldObject) {
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

	private void MouseHover() {
		if(player.hud.MouseInBounds()) {
			GameObject hoverObject = FindHitObject();
			if(hoverObject) {
				//if(player.SelectedObject) player.SelectedObject.SetHoverState(hoverObject);
				//else 
				if(hoverObject.name != "Ground") {
					Player owner = hoverObject.transform.root.GetComponent< Player >();
					if(owner) {
						Drone unit = hoverObject.transform.parent.GetComponent< Drone >();
						Building building = hoverObject.transform.parent.GetComponent< Building >();
						if(owner.username == player.username && (unit || building)) player.hud.SetCursorState(CursorState.Select);
					}
				}
			}
		}
	}
}
