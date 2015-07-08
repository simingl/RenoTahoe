using UnityEngine;
using System.Collections;

public class MinimapManagement : MonoBehaviour {
	private Camera mainCam;
	private Camera minimapCam;

	public GUISkin skin;

	private Player player;

	// Use this for initialization
	void Start () {
		mainCam = Camera.main;
		minimapCam = this.GetComponent<Camera> ();
		player = GameObject.FindGameObjectWithTag ("Player").GetComponent<Player>();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButton (0) && player.hud.MouseInBoundsMinimap()) {
			Ray ray = minimapCam.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit)) {
				mainCam.transform.position = new Vector3(hit.point.x, mainCam.transform.position.y, hit.point.z);
			}
		}
	}

	void OnGUI(){
		GUI.skin = skin;
		GUI.Box (new Rect (minimapCam.pixelRect.x, (Screen.height - minimapCam.pixelRect.yMax), minimapCam.pixelWidth, minimapCam.pixelHeight), "");
	}
}
