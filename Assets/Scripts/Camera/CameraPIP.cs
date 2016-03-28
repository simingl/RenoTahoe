using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using RTS;

public class CameraPIP : MonoBehaviour {
 	public GUISkin mySkin;
	private Player player;
	private Camera cam;
	private Drone drone;

	void Start(){
		cam = this.GetComponent<Camera>();
		drone = this.transform.parent.gameObject.GetComponent<Drone> ();
		player = GameObject.FindGameObjectWithTag ("Player").GetComponent<Player>();


	}

	void Update () {
        if (cam.tag == "Camera_1st_view")
        {
            cam.transform.GetChild(0).gameObject.GetComponent<MeshFilter>().mesh = CameraExtention.GenerateFrustumMesh(cam);
        }
        if (Input.GetMouseButton (0) && player.hud.MouseInBoundsPIP () && cam.depth == Drone.PIP_DEPTH_ACTIVE) {
			GameObject hitObject = FindHitObject ();
			if (hitObject) {
				if (hitObject.name != "Ground") {
					WorldObject worldObject = hitObject.GetComponent< WorldObject > ();
					if (worldObject) {
						worldObject.GetComponent<MapItem> ().setColor (Color.green);
						if (worldObject is NPC) {
							((NPC)worldObject).Mark ();
						} else if (worldObject is Vehicle) {
							((Vehicle)worldObject).Mark();
						}
					}
				}
			}
		} else if (Input.GetMouseButtonDown (1) && player.hud.MouseInBoundsPIP () && cam.depth == Drone.PIP_DEPTH_ACTIVE
		           && player.getSelectedObjects().Count > 0 && player.getSelectedObjects()[0] == drone) {

			GameObject hitObject = FindHitObject();
			Vector3 hitPoint = FindHitPoint();
			
			if(player.getSelectedObjects().Count > 0){
				foreach(WorldObject obj in player.getSelectedObjects()){
					obj.MouseClick(hitObject, hitPoint, player);
				}
				this.player.audioManager.playUnitMoveToSound();
			}
		}
	}

	void OnGUI(){
		if (cam.depth != Drone.PIP_DEPTH_DEACTIVE ) {
			GUI.skin = mySkin;
			GUI.Box (new Rect (cam.pixelRect.x, (Screen.height - cam.pixelRect.yMax), cam.pixelWidth, cam.pixelHeight), "");

			//draw drone icon on the top right of the camera
			if(cam.rect != ResourceManager.getInstance().getPIPCameraPosition()){
                if (GUI.Button(new Rect(cam.pixelRect.x + cam.pixelWidth - 20, (Screen.height - cam.pixelRect.yMax), 20, 20), "x"))
                {
                    cam.depth = Drone.PIP_DEPTH_DEACTIVE;
                }
            }

			//draw drone icon on the top right of the camera
			Color color = drone.color;
			Texture droneTexture = player.hud.drone_2d;
			GUI.color = color;
			GUI.DrawTexture(new Rect (cam.pixelRect.x + cam.pixelWidth-60, (Screen.height - cam.pixelRect.yMax), 40, 20), player.hud.drone_2d);

			//double click PIP camera to select the cooresponding drone
			Event e = Event.current;
			if (e.isMouse && e.type == EventType.MouseDown && e.clickCount == 2 &&  this.MouseInBoundsPIP())
			{
				if(player.getSelectedObjects().Count >0){
					Drone selectedDrone = (Drone)player.getSelectedObjects()[0];
					Camera dfcam = selectedDrone.getCameraFront();
					ResourceManager.getInstance().setCameraPosition(dfcam, cam.rect);
					//dfcam.rect = cam.rect;
				}
				player.setSelectedObject(drone);
				e.Use();
			}
		}
	}

	private GameObject FindHitObject() {
		LayerMask entitylayerMask = (1 << 11);   //Entity layer, npc and vehicle
		LayerMask groundlayerMask = (1 << 12);    //ground layer, ground
		Ray ray = this.cam.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast (ray, out hit, 20f, entitylayerMask)) {
			return hit.collider.gameObject;
		} else if (Physics.Raycast (ray, out hit, 20f, groundlayerMask)) {
			return hit.collider.gameObject;
		}

		return null;
	}

	private Vector3 FindHitPoint() {
		Ray ray = this.cam.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if(Physics.Raycast(ray, out hit)) return hit.point;
		return ResourceManager.InvalidPosition;
	}

	private bool MouseInBoundsPIP(){
		Vector3 mousePos = Input.mousePosition;
		bool insideWidth = mousePos.x >= cam.pixelRect.x && mousePos.x <= cam.pixelRect.x+cam.pixelWidth;
		bool insideHeight = mousePos.y >= cam.pixelRect.y && mousePos.y < cam.pixelRect.yMax;
		return insideWidth && insideHeight;
	}


}
