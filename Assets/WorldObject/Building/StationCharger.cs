using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RTS;

public class StationCharger : MonoBehaviour {
	public Drone drone;

	void Awake() {
		drone = null;
	}

	public void Occupy(Drone d){
		drone = d;
	}

	public bool IsOccupied (){
		return drone != null;
	}

	void Update(){
		if (drone != null && drone.currentTask != WorldObject.TASK.RECHARGING && drone.currentStatus != WorldObject.STATUS.CHARGING) {
			drone = null;
		}
	}

}
