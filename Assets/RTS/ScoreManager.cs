using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using RTS;

public class ScoreManager : MonoBehaviour {
	//private ConfigManager configManager;
	public static int score;

	Text text;

	void Awake(){
		text = GetComponent<Text> ();
		score = 0;
	}
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (ConfigManager.getInstance().getHUDShowScore ()) {
			text.text = "Score: " + score;
		} else {
			text.text = "";
		}
	}
}
