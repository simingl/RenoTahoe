﻿using UnityEngine;
using System.Collections;
using RTS;

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
		if (player.human) {
			MoveCameraByMouse ();
			RotateCamera ();
			MouseActivity();
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
			//Camera.main.transform.position = Vector3.MoveTowards(origin, destination, Time.deltaTime * ResourceManager.ScrollSpeed);
		}
	}
	
	private void RotateCamera() {
		
	}
	private void MouseActivity() {
		if(Input.GetMouseButtonDown(0)) LeftMouseClick();
		else if(Input.GetMouseButtonDown(1)) RightMouseClick();

		this.MouseHover ();
	}

	private void LeftMouseClick() {

		if(player.hud.MouseInBounds()) {
			GameObject hitObject = FindHitObject();
			Vector3 hitPoint = FindHitPoint();
			if(hitObject && hitPoint != ResourceManager.InvalidPosition) {
				if(player.SelectedObject) player.SelectedObject.MouseClick(hitObject, hitPoint, player);
				else if(hitObject.name!="Ground") {
					//WorldObject worldObject = hitObject.transform.root.GetComponent< WorldObject >();
					WorldObject worldObject = hitObject.GetComponent< WorldObject >();
					if(worldObject) {
						//we already know the player has no selected object
						player.SelectedObject = worldObject;

						Rect area = player.hud.GetPlayingArea ();
						worldObject.SetSelection(true, area);
					}
				}
			}
		}
	}
	private void RightMouseClick() {
		player.hud.GetPlayingArea ();
		if(player.hud.MouseInBounds() && !Input.GetKey(KeyCode.LeftAlt) && player.SelectedObject) {
			if(player.SelectedObject.type == WorldObjectType.Building){
				Rect area = player.hud.GetPlayingArea ();
				player.SelectedObject.SetSelection(false, area);
				player.SelectedObject = null;
			}else if(player.SelectedObject.type == WorldObjectType.Unit){
				GameObject hitObject = FindHitObject();
				Vector3 hitPoint = FindHitPoint();
				player.SelectedObject.MouseClick(hitObject, hitPoint, player);
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
				if(player.SelectedObject) player.SelectedObject.SetHoverState(hoverObject);
				else if(hoverObject.name != "Ground") {
					Player owner = hoverObject.transform.root.GetComponent< Player >();
					if(owner) {
						Unit unit = hoverObject.transform.parent.GetComponent< Unit >();
						Building building = hoverObject.transform.parent.GetComponent< Building >();
						if(owner.username == player.username && (unit || building)) player.hud.SetCursorState(CursorState.Select);
					}
				}
			}
		}
	}
}
