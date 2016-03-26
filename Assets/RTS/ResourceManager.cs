using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RTS {
	public class ResourceManager{
		private static ResourceManager instance = null;

		public static ResourceManager getInstance(){
			if (instance == null) {
				instance = new ResourceManager();
			}
			return instance;
		}

		private ResourceManager(){
			init ();
		}

		public static float ScrollSpeed { get { 
				if(Input.GetKey(KeyCode.LeftShift)){
					return 10;
				}else{
					return 1; 
				}
			} 
		}
		public static float RotateSpeed { get { return 100; } }
		public static int ScrollWidth { get { return 15; } }

		public static float MinCameraSize { get { return 10; } }
		public static float MaxCameraSize { get { return 35; } }

		public static float MinCameraWidth { get { return -500; } }
		public static float MaxCameraWidth { get { return 500; } }
		public static float MinCameraLength { get { return -500; } }
		public static float MaxCameraLength { get { return 500; } }
		public static float MaxBottom{get {return 2;}}
		public static float MaxTop{get {return 10;}}
		public static float MaxNorth{get {return 100;}}
		public static float MaxSouth{get {return -100;}}
		public static float MaxEast{get {return -100;}}
		public static float MaxWest{get {return 100;}}



		private static Vector3 invalidPosition = new Vector3(-99999, -99999, -99999);
		public static Vector3 InvalidPosition { get { return invalidPosition; } }

		private static Bounds invalidBounds = new Bounds(new Vector3(-99999, -99999, -99999), new Vector3(0, 0, 0));
		public static Bounds InvalidBounds { get { return invalidBounds; } }

		private static GUISkin selectBoxSkin;
		public static GUISkin SelectBoxSkin { get { return selectBoxSkin; } }
		
		public static void StoreSelectBoxItems(GUISkin skin) {
			selectBoxSkin = skin;
		}
        
        private Rect PIPCameraPosition  = new Rect (new Vector2 (0.8f, 0), new Vector2 (0.2f, 0.33f));
		public Rect getPIPCameraPosition(){
			return PIPCameraPosition;
		}

		private Rect[] camPos = new Rect[6];
		private Dictionary<Rect, Camera> cameras  = new Dictionary<Rect, Camera>();

		public Rect getAvailableCameraPosition(Camera cam){
			int vacuum = this.getVacuumCamPos ();
			if (vacuum != -1) {
				cameras [camPos [vacuum]] = cam;
				return camPos [vacuum];
			} else {
				Rect rect = camPos [camPos.Length -1];
				if (cameras [rect].rect != PIPCameraPosition) {
					cameras [rect] .depth = Drone.PIP_DEPTH_DEACTIVE;
				}else{
					cameras.Remove(rect);
				}

				cameras [rect] = cam;
				return rect;
			}
		}

		public void setCameraPosition(Camera cam, Rect rect){
			if (cameras.ContainsKey (rect)) {
				cameras.Remove(rect);
			}
			cameras [rect] = cam;
			cam.rect = rect;
		}

		private int getVacuumCamPos(){
			for (int i = 0; i<camPos.Length; i++) {
				if(!cameras.ContainsKey(camPos[i]) || cameras[camPos[i]].depth == Drone.PIP_DEPTH_DEACTIVE || cameras[camPos[i]].rect == this.PIPCameraPosition){
					return i;
				}
			}
			return -1;
		}
		private void init(){
			camPos[0] = new Rect (new Vector2 (0.8f, 0.80f), new Vector2 (0.1f, 0.15f));
			camPos[1] = new Rect (new Vector2 (0.9f, 0.80f), new Vector2 (0.1f, 0.15f));
			camPos[2] = new Rect (new Vector2 (0.8f, 0.65f), new Vector2 (0.1f, 0.15f));
			camPos[3] = new Rect (new Vector2 (0.9f, 0.65f), new Vector2 (0.1f, 0.15f));
			camPos[4] = new Rect (new Vector2 (0.8f, 0.50f), new Vector2 (0.1f, 0.15f));
			camPos[5] = new Rect (new Vector2 (0.9f, 0.50f), new Vector2 (0.1f, 0.15f));
		}

		//Layers
		public static int LayerMiniMap{get {return 8;}}
		public static int LayerPIPCamera{get {return 9;}}
		public static int LayerRoad{get {return 10;}}
		public static int LayerEntities{get {return 11;}}
		public static int LayerGround{get {return 12;}}
		public static int LayerEntitiesCommon{get {return 13;}}
		public static int LayerMainCamerea{get {return 14;}}

		public static int DroneBatteryLife {get {return 18*60;}}
		public static int DroneBatteryCharging {get {return 5*60;}}

		public static string TAG_MINIMAP_CAMERA {get {return "Camera_minimap";}}
	}
}