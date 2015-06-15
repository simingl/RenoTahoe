using UnityEngine;
using System.Collections;
using RTS;

public class ChangePOV : MonoBehaviour {

	private Camera activeCamera;

	private Camera camMain;

	private Player player;

	private CameraType activeCameraType;
	private Vector3 camMainPosition;
	private Quaternion camMainRotation;
	private Quaternion inValidQuaternion = new Quaternion(0f, 0f, 0f, 1f);

	// Use this for initialization
	void Start () {
		player = GetComponent<Player> ();
		camMain = Camera.main;

		this.camMainPosition=Vector3.zero;
		this.camMainRotation=this.inValidQuaternion;
	}

	void Awake(){
	}

	public void switchCamera(CameraType type){
		if (type == CameraType.Camera_Main) {
			if (activeCamera != null) {
				this.activeCamera = null;
				this.camMain.transform.position = this.camMainPosition;
				this.camMain.transform.rotation = this.camMainRotation;
				this.camMainPosition = Vector3.zero;
				this.camMainRotation = this.inValidQuaternion;
			}
		} else {
			this.backupMainCameraPosition();
			this.activeCamera = this.getActiveCamera(type);
		}
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.F1)) {
			this.switchCamera(CameraType.Camera_First_View);
		} else if (Input.GetKeyDown (KeyCode.F2)) {
			this.switchCamera(CameraType.Camera_Third_View);
		} else if (Input.GetKeyDown (KeyCode.F3)) {
			this.switchCamera(CameraType.Camera_Hover_View);
		} else if (Input.GetKeyDown (KeyCode.F4)) {
			this.switchCamera(CameraType.Camera_Main);
		}

		if (this.activeCamera)
			this.copyCameraPosition (camMain, this.activeCamera);
	}

	private void copyCameraPosition(Camera main, Camera target){
		main.gameObject.transform.position = target.gameObject.transform.position;
		main.gameObject.transform.rotation = target.gameObject.transform.rotation;
	}

	private Camera getActiveCamera(CameraType ct){
		if (player.getSelectedObjects ().Count > 0) {
			WorldObject obj = player.getSelectedObjects () [0];

			Camera[] cameras = obj.gameObject.GetComponentsInChildren<Camera> ();
			foreach (Camera cam in cameras) {
				if (ct == CameraType.Camera_First_View && cam.tag == RTS.Tags.CAM_FIRST_VIEW) { 
					return cam;
				} else if (ct == CameraType.Camera_Third_View && cam.tag == RTS.Tags.CAM_THIRD_VIEW) { 
					return cam;
				} else if (ct == CameraType.Camera_Hover_View && cam.tag == RTS.Tags.CAM_HOVER_VIEW) {
					return cam;
				}
			}

		}
		return null;
	}

	private void backupMainCameraPosition(){
		if (camMainPosition == Vector3.zero) {
			this.camMainPosition = camMain.transform.position;
		}
		float angle = Quaternion.Angle(camMainRotation, inValidQuaternion);
		if (angle == 0) {
			this.camMainRotation = camMain.transform.rotation;
		}
	}
}
