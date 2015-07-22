using UnityEngine;
using System.Collections;

namespace RTS {
	public static class ResourceManager {
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

		public static float MinCameraHeight { get { return 30; } }
		public static float MaxCameraHeight { get { return 2000; } }
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



	}
}