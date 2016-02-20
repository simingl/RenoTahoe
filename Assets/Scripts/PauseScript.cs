using UnityEngine;
using System.Collections;

public class PauseScript : MonoBehaviour {

	public bool paused;
	float num;

	void Start () {
		paused = false;
		num = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.P))
		{
			paused = !paused;
		}
		if(paused)
		{
			Time.timeScale = 0;
		}
		else
		{
			Time.timeScale = 1;
		}
	}
}
