using UnityEngine;
using System.Collections;

namespace RTS {
	public enum CursorState { Select, Move, Attack, PanLeft, PanRight, PanUp, PanDown, Harvest, Hover }
	public enum WorldObjectType{Unit, Building}
	public enum CameraType {Camera_First_View, Camera_Third_View, Camera_Hover_View, Camera_Main}

	public class Tags{
		public static string CAM_FIRST_VIEW = "Camera_1st_view";
		public static string CAM_HOVER_VIEW = "Camera_hover_view";
		public static string CAM_Main = "MainCamera";
	}
}