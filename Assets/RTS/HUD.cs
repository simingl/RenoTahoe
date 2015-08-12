using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using RTS;

public class HUD : MonoBehaviour {
	public GUISkin resourceSkin, ordersSkin, selectBoxSkin, selectionBarSkin,selectBtnSkin;

	public Texture drone_2d, drone_2d_h;

	public Texture drone_cam_front, drone_cam_down;
	
	public Button cellBtn;

	private const int RESOURCE_BAR_HEIGHT = 30;
	private const int LINE_HEIGHT = 20;

	private static int SELECT_BAR_BTN_HEIGHT = 30, SELECT_BAR_BTN_WIDTH = 60;
	private static int ACTION_BTN_WIDTH = 76, ACTION_BTN_HEIGHT = 30;
	private static int MARGIN = 50;

	private static int MINIMAP_WIDTH, MINIMAP_HEIGHT;
	private static int SELECTION_BAR_HEIGHT, SELECTION_BAR_WIDTH;
	private static int ORDERS_BAR_WIDTH, ORDERS_BAR_HEIGHT;
	private static int INFO_BAR_HEIGHT = SELECTION_BAR_HEIGHT, INFO_BAR_WIDHT ;

	private static int RESOURCE_DAYNIGHT_TOGGLE_WIDTH = 100;
	private static int RESOURCE_NAME_WIDTH = 100;
	private static int RESOURCE_LOCATION_WIDTH = 100;
	private static int RESOURCE_DRONE_HEIGHT_WIDTH = 100;
	private static int RESOURCE_DRONE_SPEED_WIDTH = 100;
	private static int RESOURCE_DRONE_ORIENT_WIDTH = 100;
	private static int RESOURCE_BATTERY_WIDTH = 100;
	private static int RESOURCE_CELL_WIDTH = 100;
	private static int RESOURCE_WATER_WIDTH = 100;

	private const int PIP_BTN_WIDTH = 30;
	private const int PIP_BTN_HEIGHT = 30;

	private const int ROW_MAX = 6;

	private Player player;

	public bool dayNightToggle = true;

	private CursorState activeCursorState;

	private GameObject sun;

	//For selection rendering
	public Texture2D selectionHighlight = null;
	public static Rect selection = new Rect(0,0,0,0);
	private Vector3 startClick = -Vector3.one;



	private Camera camera_minimap;

	void Start () {
		int WIDTH = Screen.width;
		int HEIGHT = Screen.height;

		MINIMAP_WIDTH=(int)(0.2*WIDTH);
		MINIMAP_HEIGHT = (int)(0.33 * HEIGHT);
		SELECTION_BAR_HEIGHT = (int)(0.16 * HEIGHT);
		SELECTION_BAR_WIDTH = (int)(0.29*WIDTH);
		ORDERS_BAR_WIDTH = (int)(0.131*WIDTH);
		ORDERS_BAR_HEIGHT = (int)(0.16*HEIGHT);
		INFO_BAR_HEIGHT = SELECTION_BAR_HEIGHT;
		INFO_BAR_WIDHT = (int)(0.18*WIDTH) ;

		player = transform.root.GetComponent< Player >();
		sun = GameObject.FindGameObjectWithTag ("Sun");

		RESOURCE_DAYNIGHT_TOGGLE_WIDTH = (int)(0.34*WIDTH);;
		RESOURCE_NAME_WIDTH = (int)(0.1*WIDTH);;
		RESOURCE_LOCATION_WIDTH = (int)(0.1*WIDTH);
		RESOURCE_DRONE_HEIGHT_WIDTH = (int)(0.1*WIDTH);	
		RESOURCE_DRONE_SPEED_WIDTH = (int)(0.1*WIDTH);;
		RESOURCE_DRONE_ORIENT_WIDTH = (int)(0.1*WIDTH);;
		RESOURCE_BATTERY_WIDTH = (int)(0.05*WIDTH);
		RESOURCE_CELL_WIDTH = (int)(0.05*WIDTH);
		RESOURCE_WATER_WIDTH = (int)(0.05*WIDTH);

		ResourceManager.StoreSelectBoxItems(selectBoxSkin);
	}
	
	void OnGUI () {
		if(player && player.human) {
			DrawSelectionBar();
			DrawOrdersBar();
			DrawResourceBar();
			DrawInfoBar();
			DrawPIPBar();
			SwitchDayNight();
			DrawMouseDragSelectionBox ();
		}
	}

	void Update(){
		MouseDragSelection();
	}



	private void DrawResourceBar() {
		GUI.skin = resourceSkin;
		GUI.BeginGroup(new Rect(0,0,Screen.width,RESOURCE_BAR_HEIGHT));

		int offset = 0;

		offset += RESOURCE_DAYNIGHT_TOGGLE_WIDTH;
		if (player.getSelectedObjects().Count >0) {
			GUI.color = new Color(1.0f, 1.0f, 1.0f, 0.9f);
			GUI.Box(new Rect(0,0,Screen.width, RESOURCE_BAR_HEIGHT),"");

			WorldObject obj = player.getSelectedObjects()[0];
			string name = obj.objectName; 
			string cellinfo = "Cell * ";
			string waterinfo = "Water * ";
			string location = "Location: (" + obj.transform.position.x.ToString("0.0") + ", " + obj.transform.position.z.ToString("0.0") + ")";
			string height = "Height: "+ obj.transform.position.y.ToString("0.0");
			string speed  = "Speed: "+ obj.speed.ToString("0.0");

			float angle = 0.0F;
			Vector3 axis = Vector3.zero;
			obj.transform.rotation.ToAngleAxis(out angle, out axis);
			string orient = "Orientation: " + angle.ToString("0.0")+"'";
			string battery = "";
			if (obj is Drone) {
				Drone unit = (Drone)obj;
				battery += "Battery: " + (int)(unit.currentBattery)+"%";
				cellinfo += unit.GetCellCount();
				waterinfo += unit.GetWaterCount();
			}
			GUI.DrawTexture(new Rect(offset, 5 ,40,20), this.drone_2d);
			offset +=40;
			GUI.Label(new Rect(offset,5 , Screen.width,ORDERS_BAR_HEIGHT), name);
			//DrawOutline(new Rect(offset,5 , Screen.width,ORDERS_BAR_HEIGHT), name, this.resourceSkin.GetStyle("large"), Color.white, Color.black);
			offset += RESOURCE_NAME_WIDTH;

			GUI.Label(new Rect(offset,5 , Screen.width,ORDERS_BAR_HEIGHT), location);
			//DrawOutline(new Rect(offset,5 , Screen.width,ORDERS_BAR_HEIGHT), location, this.resourceSkin.GetStyle("large"), Color.white, Color.black);
			offset += RESOURCE_LOCATION_WIDTH;

			GUI.Label(new Rect(offset,5 , Screen.width,ORDERS_BAR_HEIGHT), height);
			//DrawOutline(new Rect(offset,5 , Screen.width,ORDERS_BAR_HEIGHT), height, this.resourceSkin.GetStyle("large"), Color.white, Color.black);
			offset += RESOURCE_DRONE_HEIGHT_WIDTH;

			GUI.Label(new Rect(offset,5 , Screen.width,ORDERS_BAR_HEIGHT), speed);
			//DrawOutline(new Rect(offset,5 , Screen.width,ORDERS_BAR_HEIGHT), height, this.resourceSkin.GetStyle("large"), Color.white, Color.black);
			offset += RESOURCE_DRONE_SPEED_WIDTH;

			GUI.Label(new Rect(offset,5 , Screen.width,ORDERS_BAR_HEIGHT), orient);
			//DrawOutline(new Rect(offset,5 , Screen.width,ORDERS_BAR_HEIGHT), height, this.resourceSkin.GetStyle("large"), Color.white, Color.black);
			offset += RESOURCE_DRONE_ORIENT_WIDTH;

			GUI.Label(new Rect(offset,5 , Screen.width,ORDERS_BAR_HEIGHT), battery);
			//DrawOutline(new Rect(offset,5 , Screen.width,ORDERS_BAR_HEIGHT), battery, this.resourceSkin.GetStyle("large"), Color.yellow, Color.green);
			offset += RESOURCE_BATTERY_WIDTH;

			GUI.Label(new Rect(offset,5,Screen.width,ORDERS_BAR_HEIGHT), cellinfo);
			//DrawOutline(new Rect(offset,5,Screen.width,ORDERS_BAR_HEIGHT), cellinfo, this.resourceSkin.GetStyle("large"), Color.yellow, Color.green);
			offset += RESOURCE_CELL_WIDTH;
			GUI.Label(new Rect(offset,5,Screen.width,ORDERS_BAR_HEIGHT), waterinfo);
			//DrawOutline(new Rect(offset,5,Screen.width,ORDERS_BAR_HEIGHT), waterinfo, this.resourceSkin.GetStyle("large"), Color.yellow, Color.green);
		}
		dayNightToggle = GUI.Toggle(new Rect(5, 5, RESOURCE_DAYNIGHT_TOGGLE_WIDTH, RESOURCE_BAR_HEIGHT), dayNightToggle, "Day/Night");
		GUI.EndGroup();
	}

	private void DrawSelectionBar() {
		GUI.skin = this.selectionBarSkin;
		GUI.BeginGroup(new Rect(0, Screen.height - SELECTION_BAR_HEIGHT, Screen.width, SELECTION_BAR_HEIGHT));
		GUI.Box(new Rect(MINIMAP_WIDTH,0,SELECTION_BAR_WIDTH,SELECTION_BAR_HEIGHT),"");
		GUI.EndGroup();

		Drone[] allEntities = this.player.sceneManager.getAllDrones ();
		if(allEntities.Length>0) {
			for(int i =0;i<allEntities.Length;i++){
				GUI.skin = selectBtnSkin;
				Drone obj = allEntities[i];

				GUI.color = obj.color;

				int row = i/ROW_MAX;
				int col = i - row*ROW_MAX ;

				if(obj.isSelected()){
					if(GUI.Button(new Rect(MINIMAP_WIDTH+ col*SELECT_BAR_BTN_WIDTH,Screen.height - SELECTION_BAR_HEIGHT + row* SELECT_BAR_BTN_HEIGHT  ,SELECT_BAR_BTN_WIDTH,SELECT_BAR_BTN_HEIGHT), drone_2d_h)){
						if(Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)){
							this.player.toggleSelectObject(obj);
						}else{
							this.player.setSelectedObject(obj);
							obj.centerMainCamera();
							//this.player.changePOV.switchCamera(CameraType.Camera_Third_View);
						}
					}
				}
				else{
					if(GUI.Button(new Rect(MINIMAP_WIDTH+ col*SELECT_BAR_BTN_WIDTH,Screen.height - SELECTION_BAR_HEIGHT + row* SELECT_BAR_BTN_HEIGHT ,SELECT_BAR_BTN_WIDTH,SELECT_BAR_BTN_HEIGHT), drone_2d)){
						if(Input.GetKey(KeyCode.LeftControl)){
							this.player.toggleSelectObject(obj);
						}else{
							this.player.setSelectedObject(obj);
							obj.centerMainCamera();
							//this.player.changePOV.switchCamera(CameraType.Camera_Third_View);
						}
					}
				}
			}
		}
	}
	private void DrawInfoBar(){
		int offset = MINIMAP_WIDTH + SELECTION_BAR_WIDTH;

		//GUI.color = Color.yellow;
		GUI.skin = ordersSkin;
		GUI.BeginGroup(new Rect(0, Screen.height - INFO_BAR_HEIGHT, Screen.width, INFO_BAR_HEIGHT));
		GUI.Box(new Rect(MINIMAP_WIDTH + SELECTION_BAR_WIDTH,0,INFO_BAR_WIDHT,INFO_BAR_HEIGHT),"");
		GUI.EndGroup();

		return;

		List<WorldObject> objs = player.getSelectedObjects();
		for (int i =0; i<objs.Count; i++) {
			WorldObject obj = objs [i];
			string text = obj.objectName; 
			string cellinfo = "Cell * ";
			string waterinfo = "Water * ";
			text += "  - loc: (" + obj.transform.position.x + ", " + obj.transform.position.z + "), H: " + obj.transform.position.y;
			if (obj is Drone) {
				Drone unit = (Drone)obj;
				text += " - battery: " + unit.currentBattery;
				cellinfo += unit.GetCellCount();
				waterinfo += unit.GetWaterCount();
			}
			GUI.DrawTexture(new Rect(offset,Screen.height - INFO_BAR_HEIGHT + i*LINE_HEIGHT*2 ,40,20), this.drone_2d);
			GUI.Label(new Rect(offset+MARGIN,Screen.height - INFO_BAR_HEIGHT + i*LINE_HEIGHT*2 ,Screen.width,ORDERS_BAR_HEIGHT), text);

			GUI.Label(new Rect(offset+MARGIN,Screen.height - INFO_BAR_HEIGHT + LINE_HEIGHT  + i*LINE_HEIGHT*2,Screen.width,ORDERS_BAR_HEIGHT), cellinfo);
			GUI.Label(new Rect(offset+MARGIN+MARGIN,Screen.height - INFO_BAR_HEIGHT + LINE_HEIGHT  + i*LINE_HEIGHT*2,Screen.width,ORDERS_BAR_HEIGHT), waterinfo);
		}
	}

	private void DrawOrdersBar() {
		int offset = MINIMAP_WIDTH + SELECTION_BAR_WIDTH + INFO_BAR_WIDHT;

		GUI.color = Color.white;
		GUI.skin = ordersSkin;
		GUI.BeginGroup(new Rect(0, Screen.height - ORDERS_BAR_HEIGHT, Screen.width, ORDERS_BAR_HEIGHT));

		int orderWidth = Screen.width;
		if (player.isPIPActive ()) {
			orderWidth = ORDERS_BAR_WIDTH;
		}

		GUI.Box(new Rect(offset,0,orderWidth,ORDERS_BAR_HEIGHT),"");
		GUI.EndGroup();

		if(player.getSelectedObjects().Count>0) {
			List<WorldObject> objs = player.getSelectedObjects();
			for(int i =0;i<1;i++){
				WorldObject obj = objs[i];
				string text = obj.objectName; 
				text += "  - loc: ("+obj.transform.position.x +", "+obj.transform.position.z+"), H: " + obj.transform.position.y;
				if(obj is Drone){
					Drone unit = (Drone)obj;
					text += " - battery: "+unit.currentBattery;
				}
				if(GUI.Button (new Rect(offset+5,Screen.height - ORDERS_BAR_HEIGHT + i*LINE_HEIGHT +5,ACTION_BTN_WIDTH,ACTION_BTN_HEIGHT), "Load Cell")){
					Drone unit = (Drone)obj;
					unit.LoadCell();
				}
				if(GUI.Button (new Rect(offset + ACTION_BTN_WIDTH+5+5,Screen.height - ORDERS_BAR_HEIGHT + i*LINE_HEIGHT +5,ACTION_BTN_WIDTH,ACTION_BTN_HEIGHT), "Drop Cell")){
					Drone unit = (Drone)obj;
					unit.DropCell();
				}

				if(GUI.Button (new Rect(offset+5,Screen.height - ORDERS_BAR_HEIGHT + i*LINE_HEIGHT +5+ACTION_BTN_HEIGHT,ACTION_BTN_WIDTH,ACTION_BTN_HEIGHT), "Load Water")){
					Drone unit = (Drone)obj;
					unit.LoadWater();
				}
				if(GUI.Button (new Rect(offset + ACTION_BTN_WIDTH+5+5,Screen.height - ORDERS_BAR_HEIGHT + i*LINE_HEIGHT +5+ACTION_BTN_HEIGHT,ACTION_BTN_WIDTH,ACTION_BTN_HEIGHT), "Drop Water")){
					Drone unit = (Drone)obj;
					unit.DropWater();
				}
			}
		}
	}

	public bool MouseInBounds() {
		//Screen coordinates start in the lower-left corner of the screen
		//not the top-left of the screen like the drawing coordinates do
		Vector3 mousePos = Input.mousePosition;
		bool insideWidth = mousePos.x >= 0 && mousePos.x <= Screen.width;
		bool insideHeight = mousePos.y >= SELECTION_BAR_HEIGHT && mousePos.y <= Screen.height - RESOURCE_BAR_HEIGHT;
		//bool insideHeight = mousePos.y >= 0 && mousePos.y <= Screen.height;
		bool insideMinimap = this.MouseInBoundsMinimap ();
		bool insideOrderBar = this.MouseInBoundsOrderBar ();
		bool insidePIPCamera = this.MouseInBoundsPIP ();
		bool inBounds = insideWidth && insideHeight && !insideMinimap && !insideOrderBar && !insidePIPCamera;

		return inBounds;
	}

	public bool MouseInBoundsMinimap(){
		Vector3 mousePos = Input.mousePosition;
		bool insideWidth = mousePos.x >= 0 && mousePos.x <= MINIMAP_WIDTH;
		bool insideHeight = mousePos.y >= 0 && mousePos.y < MINIMAP_HEIGHT;
		return insideWidth && insideHeight;
	}

	private bool MouseInBoundsOrderBar(){
		Vector3 mousePos = Input.mousePosition;
		bool insideWidth = (mousePos.x >= MINIMAP_WIDTH+ SELECTION_BAR_WIDTH + INFO_BAR_WIDHT) && mousePos.x <= Screen.width;
		bool insideHeight = mousePos.y >= 0 && mousePos.y <ORDERS_BAR_HEIGHT;
		return insideWidth && insideHeight;
	}

	public bool MouseInBoundsPIP(){
		Vector3 mousePos = Input.mousePosition;
		bool insideWidth = (mousePos.x >= MINIMAP_WIDTH+ SELECTION_BAR_WIDTH + INFO_BAR_WIDHT+ORDERS_BAR_WIDTH) && mousePos.x <= Screen.width;
		bool insideHeight = mousePos.y >= 0 && mousePos.y < MINIMAP_HEIGHT;
		return insideWidth && insideHeight;
	}


	public Rect GetPlayingArea() {
		return new Rect(0, RESOURCE_BAR_HEIGHT, Screen.width - ORDERS_BAR_HEIGHT, Screen.height - RESOURCE_BAR_HEIGHT);
	}

	private void SwitchDayNight(){
		sun.SetActive(this.dayNightToggle);
	}

	//Render Selection
	private void DrawMouseDragSelectionBox(){
		if (startClick != -Vector3.one) {
			GUI.color = new Color(1,1,1,0.5f);
			GUI.DrawTexture(selection,selectionHighlight);
		}
	}

	private void MouseDragSelection(){
		if (Input.GetMouseButtonDown (0)) {
			startClick = Input.mousePosition;
		} else if(Input.GetMouseButtonUp(0)){
			startClick = -Vector3.one;
		}
		
		if (Input.GetMouseButton (0)) {
			selection = new Rect(startClick.x, InvertMouseY(startClick.y), Input.mousePosition.x - startClick.x, InvertMouseY(Input.mousePosition.y) - InvertMouseY(startClick.y));
			if(selection.width<0){
				selection.x += selection.width;
				selection.width = -selection.width;
			}
			if(selection.height<0){
				selection.y += selection.height;
				selection.height = -selection.height;
			}
		}
	}

	private void DrawPIPBar(){
		if (player.getSelectedObjects ().Count > 0) {
			WorldObject wo = player.getSelectedObjects () [0];
			if (wo is Drone) {
				Drone drone = (Drone)wo;
				int offset_w = MINIMAP_WIDTH + ORDERS_BAR_WIDTH + INFO_BAR_WIDHT + SELECTION_BAR_WIDTH + 3;
				int offset_h = Screen.height - MINIMAP_HEIGHT;

				if (drone.getCameraFront ().depth == Drone.PIP_DEPTH_ACTIVE) {
					if (GUI.Button (new Rect (offset_w, offset_h, PIP_BTN_WIDTH, PIP_BTN_HEIGHT), drone_cam_front)) {
						drone.togglePIPCamera ();
					}
				} else if (drone.getCameraDown ().depth == Drone.PIP_DEPTH_ACTIVE) {
					if (GUI.Button (new Rect (offset_w, offset_h, PIP_BTN_WIDTH, PIP_BTN_HEIGHT), drone_cam_down)) {
						drone.togglePIPCamera ();
					}
				}
			}
		}
	}

	public static float InvertMouseY(float y){
		return Screen.height - y;
	}

	public static void DrawOutline(Rect position, string text, GUIStyle style, Color outColor, Color inColor){
		GUIStyle backupStyle = style;
		style.normal.textColor = outColor;
		position.x--;
		GUI.Label(position, text, style);
		position.x +=2;
		GUI.Label(position, text, style);
		position.x--;
		position.y--;
		GUI.Label(position, text, style);
		position.y +=2;
		GUI.Label(position, text, style);
		position.y--;
		style.normal.textColor = inColor;
		GUI.Label(position, text, style);
		style = backupStyle;
	}

}
