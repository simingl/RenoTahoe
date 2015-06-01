using UnityEngine;
using System.Collections;

public class DroneController : MonoBehaviour {

	public float turnSmoothing = 15f;
	public float speedDampTime = 0.1f;
	public float speedUpTime   = 0.1f;
	public float speedUp = 3f;

	public Rigidbody player;
	
	void Awake(){
		player = GetComponent<Rigidbody> ();
	}
	
	void FixedUpdate(){
		float h = Input.GetAxis ("Horizontal");
		float v = Input.GetAxis ("Vertical");
		float j = Input.GetAxis ("Jump");

		MovementManagement (h,j,v);
	}
	
	void MovementManagement(float horizontal, float jump, float vertical){
		if (horizontal != 0f || vertical != 0f || jump != 0f) {
			this.Rotating(horizontal,vertical);

			this.Moving(horizontal,jump, vertical);
		}
	}
	
	void Rotating(float horizontal, float vertical){
		Vector3 targetDirection = new Vector3 (horizontal, 0f, vertical);
		Quaternion targetRotation = Quaternion.LookRotation (targetDirection, Vector3.up);
		Quaternion newRotation = Quaternion.Lerp (player.rotation, targetRotation, turnSmoothing*Time.deltaTime);
		player.MoveRotation (newRotation);
	}

	void Moving(float horizontal, float jump, float vertical){
		Vector3 oldPosition = player.position;
		Vector3 targetPosition = new Vector3(oldPosition.x+horizontal, oldPosition.y + jump, oldPosition.z+vertical);
		Vector3 newPosition = Vector3.Lerp (oldPosition, targetPosition, speedDampTime * Time.deltaTime);
		player.MovePosition (newPosition);
	}
}
