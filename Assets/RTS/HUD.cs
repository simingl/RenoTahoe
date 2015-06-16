using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using RTS;

public class HUD : MonoBehaviour {
	public GUISkin resourceSkin, ordersSkin, selectBoxSkin, selectionBarSkin,selectBtnSkin;
	public GUISkin mouseCursorSkin;

	private const int RESOURCE_BAR_HEIGHT = 10;
	private const int LINE_HEIGHT = 20;

	private static int MINIMAP_WIDTH=(int)(0.2*Screen.width);
	private static int SELECTION_BAR_HEIGHT = (int)(0.2*Screen.height), SELECTION_BAR_WIDTH = (int)(0.5*Screen.width);
	private static int ORDERS_BAR_HEIGHT = (int)(0.3*Screen.height);
	private static int INFO_BAR_HEIGHT = SELECTION_BAR_HEIGHT, INFO_BAR_WIDHT = (int)(0.45*Screen.width) ;

	private static int SELECT_BAR_BTN_HEIGHT = 42, SELECT_BAR_BTN_WIDTH = 86;
	private static int ACTION_BTN_WIDTH = 80, ACTION_BTN_HEIGHT = 50;
	private static int MARGIN = 50;

	private Player player;

	public bool dayNightToggle = true;

	public Texture2D activeCursor;
	public Texture2D selectCursor, leftCursor, rightCursor, upCursor, downCursor, moveCursor;

	private CursorState activeCursorState;

	private GameObject sun;

	//For selection rendering
	public Texture2D selectionHighlight = null;
	public static Rect selection = new Rect(0,0,0,0);
	private Vector3 startClick = -Vector3.one;

	public Texture drone_2d;
	public Texture drone_2d_h;

	private Canvas canvas;

	public Button cellBtn;

	// Use this for initialization
	void Start () {
		player = transform.root.GetComponent< Player >();
		sun = GameObject.FindGameObjectWithTag ("Sun");

		ResourceManager.StoreSelectBoxItems(selectBoxSkin);

		SetCursorState(CursorState.Select);
		this.canvas = GameObject.FindObjectOfType<Canvas> ();

	}
	
	void OnGUI () {
		if(player && player.human) {
			DrawSelectionBar();
			DrawOrdersBar();
			DrawResourceBar();
			DrawInfoBar();
			//DrawMouseCursor();
			SwitchDayNight();
		}

		DrawMouseDragSelectionBox ();
	}

	void Update(){
		MouseDragSelection();
	}

	private void DrawResourceBar() {
		GUI.skin = resourceSkin;
		GUI.BeginGroup(new Rect(0,0,Screen.width,RESOURCE_BAR_HEIGHT));
		dayNightToggle = GUI.Toggle(new Rect(5, 5, 100, 30), dayNightToggle, "Day/Night");
		GUI.EndGroup();
	}

	Button btn = null;

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

				if(obj.currentlySelected){
					if(GUI.Button(new Rect(MINIMAP_WIDTH+ i*SELECT_BAR_BTN_WIDTH,Screen.height - SELECTION_BAR_HEIGHT  ,SELECT_BAR_BTN_WIDTH,SELECT_BAR_BTN_HEIGHT), drone_2d_h)){
						if(Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)){
							this.player.toggleSelectObject(obj);
						}else{
							this.player.setSelectedObject(obj);
							this.player.changePOV.switchCamera(CameraType.Camera_Third_View);
						}
					}
				}
				else{
					if(GUI.Button(new Rect(MINIMAP_WIDTH+ i*SELECT_BAR_BTN_WIDTH,Screen.height - SELECTION_BAR_HEIGHT  ,SELECT_BAR_BTN_WIDTH,SELECT_BAR_BTN_HEIGHT), drone_2d)){
						if(Input.GetKey(KeyCode.LeftControl)){
							this.player.toggleSelectObject(obj);
						}else{
							this.player.setSelectedObject(obj);
							this.player.changePOV.switchCamera(CameraType.Camera_Third_View);
						}
					}
				}
			}
		}
	}
	private void DrawInfoBar(){
		int offset = MINIMAP_WIDTH + SELECTION_BAR_WIDTH;

		GUI.color = Color.yellow;
		GUI.skin = ordersSkin;
		GUI.BeginGroup(new Rect(0, Screen.height - INFO_BAR_HEIGHT, Screen.width, INFO_BAR_HEIGHT));
		GUI.Box(new Rect(MINIMAP_WIDTH + SELECTION_BAR_WIDTH,0,INFO_BAR_WIDHT,INFO_BAR_HEIGHT),"");
		GUI.EndGroup();

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
		GUI.Box(new Rect(offset,0,Screen.width,ORDERS_BAR_HEIGHT),"");
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
		bool insideHeight = mousePos.y >= ORDERS_BAR_HEIGHT && mousePos.y <= Screen.height - RESOURCE_BAR_HEIGHT;
		//bool insideHeight = mousePos.y >= 0 && mousePos.y <= Screen.height;
		return insideWidth && insideHeight;
	}

	public Rect GetPlayingArea() {
		return new Rect(0, RESOURCE_BAR_HEIGHT, Screen.width - ORDERS_BAR_HEIGHT, Screen.height - RESOURCE_BAR_HEIGHT);
	}

	private void DrawMouseCursor() {
		//bool mouseOverHud = !MouseInBounds() && activeCursorState != CursorState.PanRight && activeCursorState != CursorState.PanUp;
		bool mouseOverHud = MouseInBounds();

		if(!mouseOverHud) {
			//Cursor.visible = true;
		} else {
			Cursor.visible = false;
		
			//Cursor.visible = false;
			GUI.skin = mouseCursorSkin;
			GUI.BeginGroup(new Rect(0,0,Screen.width,Screen.height));
			UpdateCursorAnimation();
			Rect cursorPosition = GetCursorDrawPosition();
			GUI.Label(cursorPosition, activeCursor);
			GUI.EndGroup();
		}
	}

	private void UpdateCursorAnimation() {
		//sequence animation for cursor (based on more than one image for the cursor)
		//change once per second, loops through array of images
		if (activeCursorState == CursorState.Move) {
			activeCursor = moveCursor;
		} else if (activeCursorState == CursorState.PanUp) {
			activeCursor = upCursor;
		} else if (activeCursorState == CursorState.PanDown) {
			activeCursor = downCursor;
		} else if (activeCursorState == CursorState.PanLeft) {
			activeCursor = leftCursor;
		} else if (activeCursorState == CursorState.PanRight) {
			activeCursor = rightCursor;
		}

	}
	private Rect GetCursorDrawPosition() {
		//set base position for custom cursor image
		float leftPos = Input.mousePosition.x;
		float topPos = Screen.height - Input.mousePosition.y; //screen draw coordinates are inverted
		//adjust position base on the type of cursor being shown
		if(activeCursorState == CursorState.PanRight) leftPos = Screen.width - activeCursor.width;
		else if(activeCursorState == CursorState.PanDown) topPos = Screen.height - activeCursor.height;
		else if(activeCursorState == CursorState.Move || activeCursorState == CursorState.Select || activeCursorState == CursorState.Harvest) {
			topPos -= activeCursor.height / 2;
			leftPos -= activeCursor.width / 2;
		}
		return new Rect(leftPos, topPos, activeCursor.width, activeCursor.height);
	}

	public void SetCursorState(CursorState newState) {
		activeCursorState = newState;
		switch(newState) {
		case CursorState.Select:
			activeCursor = selectCursor;
			break;
		case CursorState.Move:
			activeCursor = moveCursor;
			break;
		case CursorState.PanLeft:
			activeCursor = leftCursor;
			break;
		case CursorState.PanRight:
			activeCursor = rightCursor;
			break;
		case CursorState.PanUp:
			activeCursor = upCursor;
			break;
		case CursorState.PanDown:
			activeCursor = downCursor;
			break;
		default: break;
		}
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

	public static float InvertMouseY(float y){
		return Screen.height - y;
	}
}
