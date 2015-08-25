using UnityEngine;
using System.Collections;
using RTS;

public class Helicopter :  WorldObject {
	public float moveSpeed = 0.5f;
	public float rotateSpeed =1f;
	private GameObject mark;

	private float cycle = 20f;
	private float regenTime;
	private Vector3 destination;
	private Quaternion targetRotation;

	private int width = 100;
	private int height = 100;

	private bool rotating, moving;

	void Awake(){
		regenTime = cycle;
	}

	protected virtual void Start () {
		base.Start ();
		
		this._isSelectable = false;
		
		this.scoreValue = 200;
		
		mark = GameObject.CreatePrimitive (PrimitiveType.Cube);
		mark.layer = ResourceManager.LayerMiniMap;
		mark.GetComponent<Collider> ().enabled = false;
		
		mark.transform.parent = transform;
		mark.transform.localScale = Vector3.one * 2f;
		mark.transform.localPosition = new Vector3 (0, 5f, 0);
		mark.transform.rotation = gameObject.transform.rotation;
		mark.GetComponent<Renderer> ().material.color = Color.yellow;

		this.StartMove(generateRandomPosition (width, height));
	}

	public void setColor(Color color){
		mark.GetComponent<Renderer> ().material.color = color;
	}
	
	public void Mark(){
		mark.GetComponent<Renderer> ().material.color = Color.green;
		SetLayerRecursively (gameObject, gameObject.layer, ResourceManager.LayerEntitiesCommon);
		ScoreManager.score += this.scoreValue;
	}

	void Update(){
		if (regenTime <= 0) {
			regenTime = cycle;
			this.StartMove(this.generateRandomPosition (width, height));
		} else {
			regenTime -= Time.deltaTime;
		}

		if (rotating) {
			this.TurnToTarget();
		}else if (moving) {
			this.MakeMove();
		}

	}

	private void TurnToTarget() {
		transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotateSpeed);
		Quaternion inverseTargetRotation = new Quaternion(-targetRotation.x, -targetRotation.y, -targetRotation.z, -targetRotation.w);
		if(transform.rotation == targetRotation || transform.rotation == inverseTargetRotation) {
			this.rotating = false;
			this.moving = true;
		}
	}

	private void StartMove(Vector3 dest) {
		this.destination = dest;
		targetRotation = Quaternion.LookRotation (destination - transform.position);
		this.rotating = true;
		this.moving = false;
	}

	private void MakeMove() {
		transform.position = Vector3.MoveTowards(transform.position, destination, Time.deltaTime * moveSpeed);
		if (transform.position == destination) {
			this.StartMove(this.generateRandomPosition (width, height));
		}
	}

	private Vector3 generateRandomPosition(int w, int h){
		float x = Random.Range(-1*w, h);
		float z = Random.Range(-1*w, h);
		
		return new Vector3 (x,4,z);
	}
}
