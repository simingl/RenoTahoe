using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CameraMain : MonoBehaviour
{
	private Player player;           // Reference to the player's transform.

	private Rect FULL_SCREEN_RECT = new Rect (0,0,1,1);
	private Rect PIP_SCREEN_RECT = new Rect (0,0,0.75f,1);

	void Awake ()
	{
		player = GameObject.FindGameObjectWithTag ("Player").GetComponent<Player>();
	}

	void Update(){
		if (player.isPIPActive()) {
			Camera.main.rect = PIP_SCREEN_RECT;
		} else {
			Camera.main.rect = FULL_SCREEN_RECT;
		}
	}
	


	

}