using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BatteryBar : MonoBehaviour {
	private Rigidbody rb;
	public int startingBattery = 100;
	public int currentBattery = 100;
	public Image batteryBar;
	public float flashSpeed = 5f;
	public Color flashColor = new Color(1f,0f,0f,0.1f);

	void Awaik(){
		currentBattery = startingBattery;
	}
	// Use this for initialization
	void Start () {
		rb = this.GetComponent<Rigidbody> ();

	}
	
	// Update is called once per frame
	void Update () {

		Vector3 pos = Camera.main.WorldToScreenPoint (rb.transform.position);
		batteryBar.transform.position = new Vector3(pos.x,pos.y+100,0);
	}
}
