using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StationManager : MonoBehaviour {
	private List<StationCharger> chargers = new List<StationCharger>();
	// Use this for initialization
	void Awake(){

	}
	void Start () {

		GameObject[] obs = GameObject.FindGameObjectsWithTag ("StationCharger");
		foreach (GameObject ob in obs) {
			StationCharger sc = ob.GetComponent<StationCharger>();
			chargers.Add(sc);
		}
	}
	
	// Update is called once per frame
	void Update () {

	}

	public StationCharger getNearestAvailabeCharger(Vector3 pos){
		float dist = 100000f;
		StationCharger charger = null;
		foreach (StationCharger ch in chargers) {
			if(!ch.IsOccupied()) {
				float d = Vector3.Distance(pos, ch.transform.position);
				if(d < dist){
					dist = d;
					charger = ch;
				}
			}
		}
		return charger;
	}
}
