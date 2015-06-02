using UnityEngine;
using System.Collections;

public class DroneSelection : MonoBehaviour {

	public bool selected = false;

	public GameObject unit;
	private Renderer rend;

	void Start(){
		rend = unit.GetComponent<Renderer> ();
	}

	private void Update () {
		if (rend.isVisible && Input.GetMouseButtonUp (0)) {
			Vector3 camPos = Camera.main.WorldToScreenPoint(transform.position);
			camPos.y = CameraManagement.InvertMouseY(camPos.y);
			selected = CameraManagement.selection.Contains(camPos);
		}

		if (selected) {
			rend.material.color = Color.red;
		} else {
			rend.material.color = Color.white;
		}
	}
}
