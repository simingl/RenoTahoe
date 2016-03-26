using UnityEngine;
using System.Collections;
using RTS;
using System.Collections.Generic;

public class Helicopter :  WorldObject {
	public float speed = 0f;
	private float minSpeed = 0f;
	private float maxSpeed = 10f;
	public float moveSpeed, rotateSpeed;
	private float acceleration = 0.3f;
	private float turnSpeed = 0.3f;

	private GameObject mark;

	private float cycle = 20f;
	private float regenTime;
	private Vector3 destination;

	private GameObject fire;

	private GameObject[] waypoints;

	void Awake(){
		regenTime = cycle;
		fire = transform.FindChild ("fire").gameObject;
	}

	protected virtual void Start () {
		base.Start ();
		
		this._isSelectable = false;
		
		this.scoreValue = 5000;
		
		mark = GameObject.CreatePrimitive (PrimitiveType.Cube);
		mark.layer = ResourceManager.LayerMiniMap;
		mark.GetComponent<Collider> ().enabled = false;
		
		mark.transform.parent = transform;
		mark.transform.localScale = Vector3.one * 20f;
		mark.transform.localPosition = new Vector3 (0, 5f, 0);
		mark.transform.rotation = gameObject.transform.rotation;
		mark.GetComponent<Renderer> ().material.color = Color.yellow;

		waypoints = GameObject.FindGameObjectsWithTag ("WaypointAir");


		this.destination = this.getRandomWayPoint ();
		this.currentStatus = STATUS.TAKEOFF;
	}

	private Vector3 getRandomWayPoint(){
		int wp = (int)(Random.value * waypoints.Length);
		Vector3 point = waypoints [wp].transform.position;
		if (Vector3.Distance (point, transform.position) < 1) {
			return getRandomWayPoint ();
		} else {
			return point;
		}
	}
	void Update(){
		if (this.currentStatus == STATUS.CRASHING || this.currentStatus == STATUS.DEAD)
			return;



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
		case STATUS.LANDED:
			this.Rest();
			break;
		}
	}

	private void Landing(){
		if (this.currentStatus != STATUS.LANDED) {
			Vector3 newpos = transform.position;
			newpos.y -= Time.deltaTime;
			transform.position = newpos;
		} 
	}

	float takeOffHeight = 10f;
	float deltaHeight = 0f;
		
	private void TakeOffing(){
		if (deltaHeight < takeOffHeight) {
			Vector3 newpos = transform.position;
			newpos.y += Time.deltaTime;
			deltaHeight += Time.deltaTime;
			transform.position = newpos;
			this.currentStatus = STATUS.TAKEOFF;
		} else if (this.destination != ResourceManager.InvalidPosition && !IsArrivedIn2D(this.destination)) {
			destination.y = transform.position.y;
			this.currentStatus = STATUS.MOVING;
			deltaHeight =0;
		} 
	}

	private float maxRestTime = 5f;
	private float restTime = 0;
	private void Rest(){
		if (restTime < maxRestTime) {
			restTime += Time.deltaTime;
		} else {
			restTime = 0;
			this.destination = this.getRandomWayPoint();
			this.currentStatus = STATUS.TAKEOFF;
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
		if (go.GetComponent<Drone> () != null && !this.isDead ()) {
			this.Crashing ();
		} else if (this.currentStatus == STATUS.LANDING &&( go.name == "RenoDestroyed" || go.tag == "WaypointAir")) {
			this.currentStatus = STATUS.LANDED;
		}
	} 

	private void Crashing(){
			this.GetComponent<Rigidbody> ().useGravity = true;
			this.GetComponent<Rigidbody> ().velocity = Vector3.zero;
			this.GetComponent<Animator> ().enabled = false;
			fire.SetActive (true);
			this.destination = transform.position;
			this.currentStatus = STATUS.DEAD;

			ScoreManager.score -= this.scoreValue;
	}

	private void MakeRotateMove() {
		if (this.destination != ResourceManager.InvalidPosition) {
			float distance = Vector3.Distance (destination, transform.position);
			//heading
			Quaternion targetRotation = Quaternion.LookRotation (destination - transform.position);
			transform.rotation = Quaternion.RotateTowards (transform.rotation, targetRotation, this.turnSpeed);
			
			//speed
			if (distance / speed <= speed / acceleration) {
				speed -= acceleration * Time.deltaTime;
			} else {
				speed += acceleration * Time.deltaTime;
			}
			speed = Mathf.Clamp (speed, minSpeed, maxSpeed);
			transform.Translate (0, 0, speed * Time.deltaTime);

			if (this.IsArrivedIn2D (this.destination)) {
				speed = 0f;
				this.currentStatus = STATUS.LANDING;
			}
		}
	}

}
