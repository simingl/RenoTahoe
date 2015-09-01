using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ExitDialog : MonoBehaviour {
	private GUIStyle boxStyle;
	private GUIStyle btnStysle;
	private Renderer rend;
	private bool GuiOn = false;

	private int DialogWidth;
	private int DialogHeight;
	// Use this for initialization
	void Start () {
		rend = this.GetComponent<Renderer> ();


		DialogWidth = 400;;
		DialogHeight = 300;
	}
	
	void OnMouseEnter(){
		rend.material.color = Color.green;
	}

	void OnMouseExit(){
		rend.material.color = Color.white;
	}

	void OnGUI () {
		return;
		if (Input.GetKey (KeyCode.Escape)) {
			this.GuiOn = true;
		}
		if (GuiOn) {
			boxStyle = new GUIStyle (GUI.skin.box);
			btnStysle = new GUIStyle(GUI.skin.button);
			boxStyle.fontSize = 24;
			boxStyle.padding.top = 30;
			boxStyle.fontStyle = FontStyle.Bold;
			btnStysle.fontSize = 30;

			GUI.Box(new Rect(Screen.width/2 - this.DialogWidth/2,Screen.height/2 - this.DialogHeight/2,this.DialogWidth,this.DialogHeight), "Are you sure you want to exit?", boxStyle);
			if (GUI.Button (new Rect (Screen.width/2 - this.DialogWidth/2 +100,Screen.height/2 - this.DialogHeight/2  + 130,200,50), "Yes", btnStysle)) {
				Application.Quit();
			}

			if (GUI.Button (new Rect (Screen.width/2 - this.DialogWidth/2 +100,Screen.height/2 - this.DialogHeight/2  + 200,200,50), "No", btnStysle)) {
				GuiOn=false;
			}
		}
	}

}
