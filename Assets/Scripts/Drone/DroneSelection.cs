using UnityEngine;
using System.Collections;

public class DroneSelection : MonoBehaviour {

	public bool selected = false;
	public float speed = 5f;
	public float stopDistanceOffset = 0.5f;

	public GameObject unit;
	public GameObject selectionGlow = null;
	private GameObject glow = null;
	private Renderer rend;

	private Vector3 moveToDest = Vector3.zero;
	public float floorOffset = 1f;

	private bool selectedByClick = false;

	private Rigidbody rb;

	void Start(){
		rend = unit.GetComponent<Renderer> ();
		rb = this.GetComponent<Rigidbody> ();
	}

	private void Update () {
		if (rend.isVisible && Input.GetMouseButton (0)) {
			if(!selectedByClick){
				Vector3 camPos = Camera.main.WorldToScreenPoint(transform.position);
				camPos.y = CameraManagement.InvertMouseY(camPos.y);
				selected = CameraManagement.selection.Contains(camPos);
			}

			if (selected && glow==null) { 
				glow = (GameObject)GameObject.Instantiate(selectionGlow);
				glow.transform.parent = transform;
				//glow.transform.localPosition = new Vector3(0,-GetComponent<MeshFilter>().mesh.bounds.extents.y,0);
				glow.transform.localPosition = new Vector3(0,0,-10);
				glow.transform.localRotation = new Quaternion(0,180,0,0);

				//rend.material.color = Color.red;
			} else if(!selected && glow !=null){
				GameObject.Destroy(glow);
				glow = null;
				//rend.material.color = Color.white;
			}
		}


		if(selected && Input.GetMouseButtonUp(1)){
			Vector3 destination = CameraManagement.GetDestination();
			if(destination != Vector3.zero){
				moveToDest = destination;
				moveToDest.y += floorOffset;
			}
		}

		UpdateMove ();
	}

	private void UpdateMove(){
		if (moveToDest != Vector3.zero && transform.position != moveToDest) {
			Vector3 direction = (moveToDest - transform.position).normalized;
			direction.y = 0;
			rb.velocity = direction * speed;

			if (Vector3.Distance (transform.position, moveToDest) < stopDistanceOffset) {
				moveToDest = Vector3.zero;
			}
		} else {
			rb.velocity = Vector3.zero;
		}
	}

	private void OnMouseDown(){
		selectedByClick = true;
		selected = true;
	}

	private void OnMouseUp(){
		if (selectedByClick) {
			selected =true;
		}
		selectedByClick = false;
	}

}
