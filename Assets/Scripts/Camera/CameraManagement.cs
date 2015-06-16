﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraManagement : MonoBehaviour
{
	public float smooth = 1.5f;         // The relative speed at which the camera will catch up.
	
	private Transform player;           // Reference to the player's transform.
	private Vector3 relCameraPos;       // The relative position of the camera from the player.
	private float relCameraPosMag;      // The distance of the camera from the player.
	private Vector3 newPos;             // The position the camera is trying to reach.

	//For selection rendering
	public Texture2D selectionHighlight = null;
	public static Rect selection = new Rect(0,0,0,0);
	private Vector3 startClick = -Vector3.one;

	//For movement
	private static Vector3 moveToDestination = Vector3.zero;
	private static List<string> passables = new List<string>(){"Ground"};

	//For Zoom in and out
	public float zoomMaxY = 90f;
	public float zoomMinY = 30f;
	public float zoomSpeed = 15f;
	public float zoomTime = 0.25f;
	public Vector3 zoomDest = Vector3.zero;

	void Awake ()
	{
		player = GameObject.FindGameObjectWithTag("Player").transform;
		
		relCameraPos = transform.position - player.position;
		relCameraPosMag = relCameraPos.magnitude - 0.5f;
	}
	
	void Update () {
		CheckCamera ();
		ZoomCamera ();
		CleanUp ();
	}

	private void ZoomCamera(){
		float moveY = Input.GetAxis("Mouse ScrollWheel");
		transform.position += new Vector3 (0, moveY*zoomSpeed, 0);

		if (moveY != 0) {
			zoomDest = transform.position+new Vector3(0, moveY*zoomSpeed , 0);
		}

		if (zoomDest != Vector3.zero) {
			transform.position = Vector3.Lerp(transform.position, zoomDest, zoomTime);
			if(transform.position==zoomDest){
				zoomDest = Vector3.zero;
			}
		}

		//transform.position = Vector3.Lerp (transform.position, zoomDest, zoomTime);

		if (transform.position.y > zoomMaxY) {
			transform.position = new Vector3(transform.position.x, zoomMaxY, transform.position.z);
		}
		if (transform.position.y < zoomMinY) {
			transform.position = new Vector3(transform.position.x, zoomMinY, transform.position.z);
		}
	}
	void FixedUpdate ()
	{
		// The standard position of the camera is the relative position of the camera from the player.
		Vector3 standardPos = player.position + relCameraPos;
		
		// The abovePos is directly above the player at the same distance as the standard position.
		Vector3 abovePos = player.position + Vector3.up * relCameraPosMag;
		
		// An array of 5 points to check if the camera can see the player.
		Vector3[] checkPoints = new Vector3[5];
		
		// The first is the standard position of the camera.
		checkPoints[0] = standardPos;
		
		// The next three are 25%, 50% and 75% of the distance between the standard position and abovePos.
		checkPoints[1] = Vector3.Lerp(standardPos, abovePos, 0.25f);
		checkPoints[2] = Vector3.Lerp(standardPos, abovePos, 0.5f);
		checkPoints[3] = Vector3.Lerp(standardPos, abovePos, 0.75f);
		
		// The last is the abovePos.
		checkPoints[4] = abovePos;
		
		// Run through the check points...
		for(int i = 0; i < checkPoints.Length; i++)
		{
			// ... if the camera can see the player...
			if(ViewingPosCheck(checkPoints[i]))
				// ... break from the loop.
				break;
		}
		
		// Lerp the camera's position between it's current position and it's new position.
		transform.position = Vector3.Lerp(transform.position, abovePos, smooth * Time.deltaTime);
		//transform.position = newPos;
		// Make sure the camera is looking at the player.
		SmoothLookAt();
	}
	
	
	bool ViewingPosCheck (Vector3 checkPos)
	{
		RaycastHit hit;
		
		// If a raycast from the check position to the player hits something...
		if(Physics.Raycast(checkPos, player.position - checkPos, out hit, relCameraPosMag))
			// ... if it is not the player...
			if(hit.transform != player)
				// This position isn't appropriate.
				return false;
		
		// If we haven't hit anything or we've hit the player, this is an appropriate position.
		newPos = checkPos;
		return true;
	}
	
	
	void SmoothLookAt ()
	{
		return;
		// Create a vector from the camera towards the player.
		Vector3 relPlayerPosition = player.position - transform.position;
		
		// Create a rotation based on the relative position of the player being the forward vector.
		Quaternion lookAtRotation = Quaternion.LookRotation(relPlayerPosition, Vector3.up);
		
		// Lerp the camera's rotation between it's current rotation and the rotation that looks at the player.
		transform.rotation = Quaternion.Lerp(transform.rotation, lookAtRotation, smooth * Time.deltaTime);
	}

	//Render Selection
	private void CheckCamera(){
		if (Input.GetMouseButtonDown (0)) {
			startClick = Input.mousePosition;
		} else if(Input.GetMouseButtonUp(0)){
			startClick = -Vector3.one;
		}
		
		if (Input.GetMouseButton (0)) {
			selection = new Rect(startClick.x, InvertMouseY(startClick.y), Input.mousePosition.x - startClick.x, InvertMouseY(Input.mousePosition.y) - InvertMouseY(startClick.y));
			if(selection.width<0){
				selection.x += selection.width;
				selection.width = -selection.width;
			}
			if(selection.height<0){
				selection.y += selection.height;
				selection.height = -selection.height;
			}
		}
	}
	

	public static float InvertMouseY(float y){
		return Screen.height - y;
	}

	private void CleanUp(){
		if (!Input.GetMouseButtonUp (1)) {
			moveToDestination = Vector3.zero;
		}
	}

	public static Vector3 GetDestination(){
		if (moveToDestination == Vector3.zero) {
			RaycastHit hit;
			Ray r = Camera.main.ScreenPointToRay(Input.mousePosition);

			if(Physics.Raycast(r, out hit)){
				while(!passables.Contains(hit.transform.gameObject.tag)){
					if(!Physics.Raycast(hit.point+r.direction*0.1f, r.direction, out hit)){
						break;
					}
				}

			}

			if(hit.transform != null){
				moveToDestination = hit.point;
			}
		}
		return moveToDestination;
	}

}