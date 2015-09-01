using UnityEngine;
using System.Collections;

public class DronePhysics : MonoBehaviour {
	public float speed = 0.0f;
	public float acceleration = 1.0f;
	public float maxspeed = 2.0f;
	public float minspeed = -0.25f;
	public float heading = 0.0f;

	public float turnAcc = 0.0f;

	public float desiredHeading = 0.0f;
	public float desiredSpeed = 1.0f;

	private float elapsed = 0.0f;

	private float rudder = 0f;
	private float rudderDelta = 2.0f;
	private float maxRudder = 6.0f;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		elapsed += Time.deltaTime;

		//steering
		rudder += Input.GetAxis ("Horizontal") * rudderDelta * Time.deltaTime;
		rudder = Mathf.Clamp (rudder, -1*maxRudder, maxRudder);

		heading = (heading + rudder * Time.deltaTime * signedSqrt(speed)) % 360;

		transform.eulerAngles = new Vector3(transform.eulerAngles.x, heading, -1*rudder);

		// Sail
		speed += Input.GetAxis("Vertical") * acceleration * Time.deltaTime;
		if( speed > maxspeed ){
			speed = maxspeed;
		} else if ( speed < minspeed ){
			speed = minspeed;
		}
		
		transform.Translate(0, 0, speed * Time.deltaTime);

	}

	float signedSqrt(float x ){
		float r = Mathf.Sqrt(Mathf.Abs( x ));
		if( x < 0 ){
			return -r;
		} else {
			return r;
		}
	}
}
