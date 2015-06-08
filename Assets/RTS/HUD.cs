using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using RTS;

public class HUD : MonoBehaviour {
	public GUISkin resourceSkin, ordersSkin, selectBoxSkin;
	public GUISkin mouseCursorSkin;
	private const int SELECTION_NAME_HEIGHT = 15;
	private const int ORDERS_BAR_HEIGHT = 200, RESOURCE_BAR_HEIGHT = 40;
	private Player player;

	public bool dayNightToggle = true;

	public Texture2D activeCursor;
	public Texture2D selectCursor, leftCursor, rightCursor, upCursor, downCursor, moveCursor;

	private CursorState activeCursorState;

	private GameObject sun;

	// Use this for initialization
	void Start () {
		player = transform.root.GetComponent< Player >();
		sun = GameObject.FindGameObjectWithTag ("Sun");

		ResourceManager.StoreSelectBoxItems(selectBoxSkin);

		SetCursorState(CursorState.Select);
	}
	
	void OnGUI () {
		if(player && player.human) {
			DrawOrdersBar();
			DrawResourceBar();
			DrawMouseCursor();
			SwitchDayNight();
		}
	}

	void Update(){
		//Cursor.visible = false;
	}
	private void DrawResourceBar() {
		GUI.skin = resourceSkin;
		GUI.BeginGroup(new Rect(0,0,Screen.width,RESOURCE_BAR_HEIGHT));
		//GUI.Box(new Rect(0,0,Screen.width,RESOURCE_BAR_HEIGHT),"");
		dayNightToggle = GUI.Toggle(new Rect(5, 5, 100, 30), dayNightToggle, "Day/Night");
		GUI.EndGroup();
	}

	private void DrawOrdersBar() {
		GUI.skin = ordersSkin;
		GUI.BeginGroup(new Rect(0, Screen.height - ORDERS_BAR_HEIGHT, Screen.width, ORDERS_BAR_HEIGHT));
		GUI.Box(new Rect(0,0,Screen.width,ORDERS_BAR_HEIGHT),"");

		GUI.EndGroup();
		string selectionName = "";
		if(player.SelectedObject) {
			selectionName = player.SelectedObject.objectName;
		}
		if(!selectionName.Equals("")) {
			GUI.Label(new Rect(Screen.width/2,Screen.height - ORDERS_BAR_HEIGHT,Screen.width,ORDERS_BAR_HEIGHT), selectionName);
		}
	}

	public bool MouseInBounds() {
		//Screen coordinates start in the lower-left corner of the screen
		//not the top-left of the screen like the drawing coordinates do
		Vector3 mousePos = Input.mousePosition;
		bool insideWidth = mousePos.x >= 0 && mousePos.x <= Screen.width;
		//bool insideHeight = mousePos.y >= ORDERS_BAR_HEIGHT && mousePos.y <= Screen.height - RESOURCE_BAR_HEIGHT;
		bool insideHeight = mousePos.y >= 0 && mousePos.y <= Screen.height;
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
		Debug.Log ("SetNewState: "+newState);
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

}
