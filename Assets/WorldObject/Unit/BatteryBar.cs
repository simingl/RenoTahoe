using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BatteryBar : MonoBehaviour {
	public float startingBattery = 100.0f;
	public float currentBattery;

	public Slider batterySlider;
	public Image emptyImage;

	public float speed= 0.1f;

	void Awake(){
		currentBattery = startingBattery;
	}

	void Update(){
		emptyImage.color = Color.red;
		batterySlider.value = currentBattery;
	}

}
