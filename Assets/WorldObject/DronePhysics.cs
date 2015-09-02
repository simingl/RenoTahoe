using UnityEngine;
using System.Collections;
using RTS;

public class DronePhysics : MonoBehaviour {
	public float speed = 0.0f;
	public float acceleration = 1.0f;
	public float maxspeed = 2.0f;
	public float minspeed = -0.25f;
	public float heading = 0.0f;

	private float elapsed = 0.0f;

	private float rudder = 0f;
	private float rudderDelta = 30.0f;
	private float maxRudder = 30.0f;

	private Drone drone;

	private Vector3 destination;

	void Start () {
		drone = GetComponent<Drone> ();
		this.destination = ResourceManager.InvalidPosition;
	}
	
	void Update () {
		elapsed += Time.deltaTime;

		this.getCurrentDestination ();

		if (this.destination != ResourceManager.InvalidPosition) {

			float distance = Vector3.Distance(destination, transform.position);
			//heading
			Quaternion targetRotation = Quaternion.LookRotation (destination - transform.position);
			transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 0.25f);

			//speed
			float breakTime = speed / acceleration;
			if(distance <= 0.1f){
				speed = 0f;
			}else if (distance / speed <= breakTime){
				speed -= acceleration * Time.deltaTime;
			}else{
				speed += acceleration * Time.deltaTime;
			}
			speed = Mathf.Clamp (speed, minspeed, maxspeed);
			transform.Translate(0, 0, speed * Time.deltaTime);
		}


//		this.HandleKeyboardControl ();
		drone.CalculateBounds ();
	}

	private void HandleKeyboardControl(){
		//steering
		rudder += Input.GetAxis ("Horizontal") * rudderDelta * Time.deltaTime;
		rudder = Mathf.Clamp (rudder, -1*maxRudder, maxRudder);
		heading = (heading + rudder * Time.deltaTime * signedSqrt(speed)) % 360;
		transform.eulerAngles = new Vector3(transform.eulerAngles.x, heading, -1*rudder);
		
		// Sail
		speed += Input.GetAxis("Vertical") * acceleration * Time.deltaTime;
		speed = Mathf.Clamp (speed, minspeed, maxspeed);

		transform.Translate(0, Input.GetAxis("Jump")*0.01f*Time.deltaTime, speed * Time.deltaTime);
	}

	public void getCurrentDestination(){
		this.destination = drone.getCurrentDestination ();
	}

	private float signedSqrt(float x ){
		float r = Mathf.Sqrt(Mathf.Abs( x ));
		if( x < 0 ){
			return -r;
		} else {
			return r;
		}
	}
}
