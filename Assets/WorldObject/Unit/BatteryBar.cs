using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BatteryBar : MonoBehaviour {

	public int startingBattery = 100;
	public int currentBattery;
	public Image batteryBar;
	public float flashSpeed = 5f;
	public Color flashColor = new Color(1f,0f,0f,0.1f);

	void Awaik(){
		currentBattery = startingBattery;
	}
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
