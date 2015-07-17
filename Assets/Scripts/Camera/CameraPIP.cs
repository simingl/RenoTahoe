using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using RTS;

public class CameraPIP : MonoBehaviour {
 	public GUISkin mySkin;
	private Player player;
	private Camera cam;

	void Start(){
		cam = this.GetComponent<Camera>();
		player = GameObject.FindGameObjectWithTag ("Player").GetComponent<Player>();
	}

	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButton (0) && player.hud.MouseInBoundsPIP ()) {
			GameObject hitObject = FindHitObject ();
			if (hitObject) {
				if (hitObject.name != "Ground") {
					WorldObject worldObject = hitObject.GetComponent< WorldObject > ();
					if (worldObject) {
						worldObject.GetComponent<MapItem> ().setColor (Color.green);
						if (worldObject is NPC) {
							((NPC)worldObject).setColor (Color.green);
						} else if (worldObject is Vehicle) {
							((Vehicle)worldObject).setColor (Color.green);
						}
					} else {
						Debug.Log ("Hit nothing!");
					}
				}
			}
		} else if (Input.GetMouseButtonDown (1) && player.hud.MouseInBoundsPIP () && player.getSelectedObjects().Count > 0) {
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
		if (cam.depth != -1) {
			GUI.skin = mySkin;
			GUI.Box (new Rect (cam.pixelRect.x, (Screen.height - cam.pixelRect.yMax), cam.pixelWidth, cam.pixelHeight), "");
		}
	}

	private GameObject FindHitObject() {
		LayerMask entitylayerMask = (1 << 11);   //Entity layer, npc and vehicle
		LayerMask groundlayerMask = (1 << 12);    //ground layer, ground
		Ray ray = cam.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast (ray, out hit, 20f, entitylayerMask)) {
			return hit.collider.gameObject;
		} else if (Physics.Raycast (ray, out hit, 20f, groundlayerMask)) {
			return hit.collider.gameObject;
		}

		return null;
	}

	private Vector3 FindHitPoint() {
		Ray ray = cam.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if(Physics.Raycast(ray, out hit)) return hit.point;
		return ResourceManager.InvalidPosition;
	}
}
