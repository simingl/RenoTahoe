using UnityEngine;
using System.Collections;

public class MinimapManagement : MonoBehaviour {
	private Camera mainCam;
	private Camera minimapCam;

	public GUISkin skin;

	// Use this for initialization
	void Start () {
		mainCam = Camera.main;
		minimapCam = this.GetComponent<Camera> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButton (0)) {
			Ray ray = minimapCam.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit)) {
				mainCam.transform.position = new Vector3(hit.point.x, mainCam.transform.position.y, hit.point.z);
			}
		}
	}

	void OnGUI(){
		return;

		Ray mainray = mainCam.ScreenPointToRay (new Vector3(Screen.width/2, 0, Screen.height/2));

		float miniCamWidth = minimapCam.pixelWidth;
		float miniCamHeight = minimapCam.pixelHeight;

		GUI.skin = this.skin;
		float width = 10;
		float height = 10;
		float startX = mainray.origin.x + miniCamWidth/2 - width/2;
		float startY = Screen.height - mainray.origin.z - height/2 - miniCamHeight/2;

		GUI.Box(new Rect(startX,startY,width,height),"");
	}
}
