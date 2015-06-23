using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CameraPIP : MonoBehaviour {
 	public GUISkin mySkin;

	private Camera cam;

	void Start(){
		cam = this.GetComponent<Camera>();
	}
	void OnGUI(){
		if (cam.depth != -1) {
			GUI.skin = mySkin;
			GUI.Box (new Rect (cam.pixelRect.x, (Screen.height - cam.pixelRect.yMax), cam.pixelWidth, cam.pixelHeight), "");
		}
	}
}
